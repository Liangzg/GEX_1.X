require('Net/Socket')

NetClient = {}

NetClient.SokcetError = {
    ['eSendError']      = 0, -- /*发送数据错误 */
    ['eRecvError']      = 1, -- /*接受数据错误*/
    ['eTransError']     = 2, -- /*事务错误*/
}

--/*网络IO状态*/
NetClient.NetWorkState = {
    ['eDisConnected'] = 0, -- /*断开连接的*/
    ['eLoging']       = 1, -- /*登录中*/
    ['eReactiving']   = 2, -- /*激活中*/
    ['eWorking']      = 3  -- /*运行中*/
}

NetClient.KickCode = {
    ['eDUPLICATE_LOGIN']    = 0, --  /** 0 - 重复登录 */
    ['eBLOCK_LOGIN']        = 1, --  /** 1 - 玩家被封禁 */
    ['eLOGIN_TIMEOUT']      = 2, --  /** 2 - 登录超时 */
    ['eSERVER_CLOSED']      = 3, --  /** 3 - 服务器维护 */
}

NetClient.NetErrorType = {
    ['eNoError']         = 0,
    ['eConnectError']    = 6,     --/*网络连接错误                   -(有限次重试)*/

    ['eBuildNetError']   = 1,     --/*建立网络错误                   -(重登录)*/
    ['eLoginError']      = 7,     --/*登录错误                             -(重登录)*/
    ['eReactiveError']   = 8,     --/*激活错误                             -(重登录)*/
    ['eTransError']      = 4,     --/*事务传输错误                   -(重登录)*/

    ['eSendError']       = 2,     --/*发送事务数据错误         -(重激活)*/

    ['eRecvTimeout']     = 5,     --/*网络接收超时错误         -(重登录)*/

    ['eDisConnectError'] = 3,     --/*网络断开错误                   -(重激活)*/

    ['eKickError']       = 9      --/*服务器主动踢掉错误    -(重登录)*/
}

function NetClient:init()
    self.socket                = NetCore.Net.New()
    self.netPackProtocol       = NetCore.PackProtocol.New()
    self.netSendPackVStream    = NetCore.VarintStream.New()
    self.netRespPackVStream    = NetCore.VarintStream.New()
    self.sendPackQueue         = Dequeue.new()

    self.netWorkState     = NetClient.NetWorkState.eDisConnected
    self.netErrorType    = NetClient.NetErrorType.eNoError

    self.beatTimeCount   = 15.0
    self.lastBeatTime    = 0
    self.flag_heart_beat            = false
    self.flag_trans_timeout_tip     = false
    self.flag_trans_lock            = false
    self.flag_do_building     = false
    self.flag_ready_notify    = false
    self.netPacket            = nil
    self.fnNetError           = nil
--    self.serverId             = ''
end

function NetClient:setIp( ip,port )
    self.socket:setIp(ip,port)
end

function NetClient:setErrorCB(fn)
    self.fnNetError = fn
end

function NetClient:_notifyMessage(msg,...)
    if not self.msgListeners then
        return
    end

    for listener,l in pairs(self.msgListeners) do
        listener(msg,...)
    end
end

function NetClient:registerNetMessage(listener)
    if not self.msgListeners then
        self.msgListeners = {}
    end

    self.msgListeners[listener] = listener
end

function NetClient:removeNetMessage(listener)
    self.msgListeners[listener] = nil
end

function NetClient:sendRequest(req,tSend,tResp,fnRespond)
    -- 构造一个包
    local packet = {}
    packet['MOD']     = req['_MOD_']
    packet['MED']     = req['_MED_']
    packet.req        = req
    packet.tSend      = tSend
    packet.tResp      = tResp
    packet.fnRespond  = fnRespond
    --发送请求包
    --log("packet - ok")
    self:sendPacket(packet)
end

function NetClient:serializePacket(packet)
    -- 构建缓冲
    --log("sendRequest-ByteBuffer:new")
    local netPackBuffer  = NetCore.ByteArray.New()

    --log("attachBuffer")
    self.netSendPackVStream:attachBuffer(netPackBuffer)
    --log("attachBuffer - ok")

    -- 处理发送包头
    --log("hanlde head")
    log("req module = " ..packet.req['_MOD_'] .. " req mothed = " ..packet.req['_MED_']  )
    self.netSendPackVStream:writeInt8(packet.req['_MOD_'])
    self.netSendPackVStream:writeInt8(packet.req['_MED_'])
    self.netSendPackVStream:writeInt32(0)--预留六字节
    self.netSendPackVStream:writeInt16(0)
    --log("hanlde head - ok")

    -- 请求处理回调(请求序列化)
    local T = NetProto.TE.create(NetProto.WT,self.netSendPackVStream,packet.req)
    packet.tSend(T)

    -- 写签名缓冲
    netPackBuffer = self.netPackProtocol:signByteStream(netPackBuffer)

    self.netSendPackVStream:detachBuffer()

    packet.sendBuffer = netPackBuffer
end

---
--@param byteBuffer ByteBuffer
function NetClient.signByteStream(byteBuffer)
--local moduleNumber = byteBuffer:
end

--推送玩家下线,0:重登,1:封禁,2:登录超时,3:服务器维护
function NetClient:msgPushPlayerKickoff(code)
    self:onNetKickError(code.vint32)
end

function NetClient:startNetClient()
    self.socket:startNet()

    --self:resetNet()

--    self.socket:setErrorNotify(function(error)
--        self:onSocketError(error)
--    end)

    LateUpdateBeat:Add(function()self:onUpdate()end,nil)
end

function NetClient:shutdownNet()
    --self:freeWork()
    self.socket:shutdownNet()
    self.sendPackQueue:removeAll()

    LateUpdateBeat:Remove(function()self:onUpdate()end,nil)
    
    --self.socket:setErrorNotify(nil)
end

function NetClient:onSocketError(error)
    log("onSocketError#############################################")
    if  error == NetClient.SokcetError.eSendError then
        self:onNetSendError()
    elseif error == NetClient.SokcetError.eRecvError then
        self:onNetRecvError()
    elseif error == NetClient.SokcetError.eTransError then
        self:onNetTransError()
    end
end

function NetClient:reactiveGame()

    if not self.socket:isConnected() then
        self.netWorkState = NetClient.NetWorkState.eReactiving

        self:connectNetAsyc(
            function(rst)
                if rst == true then
                    local req = NetProtoUser.SendReconnect:new()
                    req.username       = modulePlayer.username
                    req.validKey       = modulePlayer.validKey
                    req.selectPlayerId = modulePlayer.role.id
                    req.firstBindTime  = modulePlayer.bindTime
                    req:send(
                        -- UserProtocol::BindPlayerVO vo
                        function(vo)
                            if vo.result then
                                --modulePlayer.bindTime = vo.bindTime
                                TimeService:adjustServerTime(math.floor(vo.bindTime / 1000))
                                TimeService:setServerTimeZoneDiff(vo.timeZone)

                                --登录成功(更新状态为-正常工作)
                                self.netWorkState = NetClient.NetWorkState.eWorking

                                self:hearBeatON()

                                self:_notifyMessage('reactive_game')
                            else
                                self:onNetReactiveError()
                            end
                        end)
                end
            end)
    else
        assert(false,"网络已连接中.")
    end
end

function NetClient:onUpdate()
    --self:handleHeartBeatTime()
    self:handleRevQueue()
    self:handleSendTimeOut()
    --log("network ok!")
end

function NetClient:sendNext()

    if self.netPacket then
        return
    end

    if self.sendPackQueue:empty() then
        return
    end

    local netPacket = self.sendPackQueue:popFront()
    self.netPacket = netPacket

    self:serializePacket(netPacket)

    self.socket:sendBytes(netPacket.sendBuffer)

    --log("ByteBuffer:releaseownership")
--    local rst = tolua.releaseownership(netPacket.sendBuffer)
--    assert(rst,"send WTF!")
    --log("send attach")
    
    local module = netPacket['MOD']
    local method = netPacket['MED']
    log("发送网络消息：Module:"..module.." Method:"..method)
    self.sendClock = os.clock()
    --self.socket:sendNext()
end

function NetClient:handleRevQueue()
    local revBuffer = self.socket:pickRevQueue()

    if nil == revBuffer then
        return
    end

    local varStream = self.netRespPackVStream
    --log("rev attach")
    varStream:attachBuffer(revBuffer)

    -- 处理响应包头 --读包头
    --local len = varStream:readInt32()
    local sn        = varStream:readInt8()
    local module    = varStream:readVarint8()
    local method    = varStream:readVarint8()

    if method < 100 then
        local sendPacket = self.netPacket
        self.netPacket = nil

        --断言回应方法与之对应
        assert(module == sendPacket['MOD'],'respond module number no match! .. module = ' ..module .. " ### sendPacket['MOD'] = "..sendPacket['MOD'] )
        assert(method == sendPacket['MED'],'respond module number no match!')

        local nowClock = os.clock()
        local diffTick =  nowClock- self.sendClock
        
        self:sendNext()

        local o = {}
        local T = NetProto.TE.create(NetProto.RT,varStream,o)
        sendPacket.tResp(T)-- 数据包回调

        if (o._result_ == nil) or (o._result_ == true) then
            if not(sendPacket.fnRespond == nil) then
                local startClock = os.clock()
                sendPacket.fnRespond(o)
                local nowClock = os.clock()
                local diffTick =  nowClock- startClock
                log("响应处理时间"..diffTick..".2f):Module:"..module.." Method:"..method)
            end
        else
            log("服务器返回了错误响应！")
        end

    else
        log("收到一个推送消息：Module:%d Method:%d",module,method)
        local startClock = os.clock()
        local netModule = NetProto.NetModule[module]
        local netProto  = netModule.MesssagePush[method]
        local msgName   = netProto.msg
        local st        = netProto.st
        local listeners = netModule.msgPushInterface
        local vo = {}
        local T = NetProto.TE.create(NetProto.RT,varStream,vo)
        st(T)
        NotifyEvent(netModule,'msgPushInterface',msgName,vo)

        local nowClock = os.clock()
        local diffTick =  nowClock- startClock
        log("处理时间(%.2f)",diffTick)
        --log("MakeFightActionOnce cost:%.2f",diffTick)
    end

    varStream:detachBuffer()

    if self.sendPackQueue:empty() and self.flag_trans_lock then
        self:freeTrans()
    end
end

function NetClient:sendPacket(netPacket)
    local mod = netPacket['MOD']
    local med = netPacket['MED']

    if self.flag_do_building and
        --如果是登录或者重连,则请求包放在队列前.
        mod == NetProtoUser.SendLogin._MOD_ and(
        med == NetProtoUser.SendLogin._MED_ or
        med == NetProtoUser.SendReconnect._MED_) then
        self.sendPackQueue:pushFront(netPacket)
        --self.socket:sendBytesFront(netPacket.sendBuffer)
        --log("sendBytesFront - ok")
    else
        self.sendPackQueue:pushBack(netPacket)
        --self.socket:sendBytes(netPacket.sendBuffer)
        --log("sendBytes - ok")
    end

    if mod == NetProtoUser.SendHeartBeat._MOD_ and
        med == NetProtoUser.SendHeartBeat._MED_ then
        self:sendNext()
        return --心跳不锁屏
    end

    -- 为什么 要返回?? (因为断网继续发请求会引起锁屏,无法点击重连按钮)
--    if not(self.socket:isWorking()) or
--        not(self.socket:isConnected()) then
--        return
--    end

--    if not self.flag_trans_lock then
--        self.flag_trans_lock = true
--        --锁屏
--        UI:lockInteraction()
--        self:TransTimeOutON()
--    end

    self:sendNext()
end

function NetClient:TransTimeOutON()
    self.flag_trans_timeout_on = true
    self.nTimeOutCount = 0
end

function NetClient:TransTimeOutOFF()
    self.flag_trans_timeout_on = false
    self.nTimeOutCount = 0
end

function NetClient:handleSendTimeOut(dt)
    if not self.flag_trans_timeout_on then
        return
    end

    self.nTimeOutCount = self.nTimeOutCount + Time.deltaTime
    --可以配表，暂时写死
    local sendTimeout = 0.25
    local transTimeOut = 25

    if not self.flag_trans_timeout_tip and
        self.nTimeOutCount >= sendTimeout then
        self:onTransTimeOutTip()
    elseif self.nTimeOutCount >= transTimeOut then
        self:onRecvTimeout()
    end
end

function NetClient:onTransTimeOutTip()
    self.flag_trans_timeout_tip = true
    UI:showNetWaitREQ()
end

function NetClient:hearBeatONTimeUpdate()
    self.lastBeatTime = os.clock()
end

function NetClient:hearBeatON()
    self.flag_heart_beat = true
end

function NetClient:handleHeartBeatTime()
    
--    if not(self.socket:isWorking()) or
--        not(self.socket:isConnected()) or
--        not self.flag_heart_beat then
--        return
--    end

    local now = os.clock()

    if (now - self.lastBeatTime) > self.beatTimeCount then
        self:hearBeatONTimeUpdate()

        if not (self.sendPackQueue:empty()) then
            return
        end

        self:heartBeatOFF()

        local req = NetProtoUser.SendHeartBeat:new()
        req:send(function(vo)
            self:hearBeatON()
            --log("hearBeatON!")
        end)

    end
end

function NetClient:heartBeatOFF()
    self.flag_heart_beat = false
end

function NetClient:freeTrans()
    if  self.flag_trans_lock == true then
        self.flag_trans_lock = false

        --请求事务发送中断-解锁屏
        UI:delayUnlockInteraction()

        self:TransTimeOutOFF()

        if self.flag_trans_timeout_tip == true then
            -- /*请求事务发送中断 - 解除请求提示*/
            UI:stopNetTip()
            self.flag_trans_timeout_tip = false
        end
    end
end

function NetClient:onRecvTimeout()
    self:freeWork()

    self:relogin('数据传输超时,\n请保持网络环境顺畅')

    self.netErrorType = NetClient.NetErrorType.eRecvTimeout
    self:notifyError()
end

function NetClient:onNetRecvError()
    log("onNetRecvError")

    --处理完所有的剩下的包.
    while not self.socket:isRecvQueueEmpty() do
        self:handleRevQueue()
    end

    --判断是否是KICK错误
    if self.kickCode then
        --已在KICK中处理.
        return
    end

    --判断是否传输错误
    if self.netPacket then
        local netPacket = self.netPacket
        log("NetRecvError:self.netPacket not nil.(it's a TransError)")
        log("ERROR - MOD:%d,MED:%d",netPacket['MOD'],netPacket['MED'])
        self:onNetTransError()
        return
    end

    --只是连接断开,自动重连.
    self:freeWork()

    --自动重连
    if self.netWorkState == NetClient.NetWorkState.eWorking then
        self:reactiveGame()
    end

    self.netErrorType = NetClient.NetErrorType.eReactiveError
    self:notifyError()
end

function NetClient:onNetSendError()
    log("onNetSendError")

    self:freeWork()

    self:relogin('数据发送失败,\n请保持网络环境顺畅')

    self.netErrorType = NetClient.NetErrorType.eSendError
    self:notifyError()
end

function NetClient:onNetTransError()
    log("onNetTransError")

    self:freeWork()

    self:relogin('数据传输错误,\n请保持网络环境顺畅')

    self.netErrorType = NetClient.NetErrorType.eTransError
    self:notifyError()
end

function NetClient:onNetConnectError()
    log("onNetConnectError")

    UI:stopNetTip()

    self:retry("网络无法连接,\n请检查网络是否正确")

    self.netErrorType = NetClient.NetErrorType.eConnectError
    self:notifyError()
end

function NetClient:onNetKickError(kickCode)
    log("onNetKickError")

    self:freeWork()

    self.kickCode = kickCode

    if kickCode == NetClient.KickCode.eDUPLICATE_LOGIN then
        self:relogin('当前账号已在其他设备上登录')
    elseif kickCode == NetClient.KickCode.eLOGIN_TIMEOUT then
        self:relogin('登陆过期')
    elseif kickCode == NetClient.KickCode.eBLOCK_LOGIN then
        self:relogin('账号已被锁定')
    elseif kickCode == NetClient.KickCode.eSERVER_CLOSED then
        self:relogin('服务器维护中')
    else
        self:relogin('网络连接错误')
    end

    self.netErrorType = NetClient.NetErrorType.eKickError
    self:notifyError()
end

function NetClient:onNetLoginError()
    log("onNetLoginError")

    self:freeWork()

    self:relogin('登录验证失败,\n请保持网络环境顺畅')

    self.netErrorType = NetClient.NetErrorType.eLoginError
    self:notifyError()
end

function NetClient:onNetReactiveError()
    log("onNetReactiveError")

    self:freeWork()

    self:relogin('验证失效')

    self.netErrorType = NetClient.NetErrorType.eLoginError
    self:notifyError()
end

function NetClient:connectNetAsyc(cb)

    if self.socket:isConnected() then
        cb(true)
    end

    --/* 建立网络 - 锁屏*/
    UI:showNetWaitForConnect()
    UI:lockInteraction()

    assert(not self.flag_do_building,"正在连接状态中,重复连接.")

    self.flag_do_building     = true

    self.socket:connectNetScript(function(rst)
        self.netPackProtocol:resetSN()

        UI:unlockInteraction()
        UI:stopNetTip()

        if not rst then
            -- /*连接失败 - 解锁屏*/
            self.flag_do_building  = false

            self:onNetConnectError()
        else
            self:startWork()
        end

        cb(rst)

        self.flag_do_building = false
    end)
end

function NetClient:startWork()
    if self.flag_start_work then
        assert(false)
        return
    end

    self.flag_start_work = true
    self.socket:working()
    NetProtoUser.listenMessagePush(self)
end

function NetClient:freeWork()
    if not self.flag_start_work then
        return
    end

    self.flag_start_work = false
    self:heartBeatOFF()
    self:freeTrans()
    UI:stopNetTip()

    NetProtoUser.removeMessagePush(self)
end

function NetClient:notifyError()
    if (self.fnNetError) and (not self.flag_ready_notify) then
        self.flag_ready_notify = true
        self.fnNetError(self.netErrorType)
    end
end

function NetClient:resetNet(cleanQueue)
    self.kickCode      = nil
    self.netErrorType  = NetClient.NetErrorType.eNoError
    self.netWorkState  = NetClient.NetWorkState.eDisConnected
    self.flag_ready_notify = nil
    self.flag_do_building  = nil
    self.netPacket         = nil
    self.socket:closeConnect()
    self.netPackProtocol:resetSN()

    if cleanQueue then
        self.socket:clearDataQueue()
    end
end

function NetClient:loginGame(lfn)
    
    if not self.socket:isConnected() then
        self.netWorkState = NetClient.NetWorkState.eLoging
        self:connectNetAsyc(
            function(rst)
                if rst == true then
                    self.socket:clearDataQueue()

                    lfn(function(result)
                        if result then
                            --登录成功(更新状态为-正常工作)
                            self.netWorkState = NetClient.NetWorkState.eWorking

                            self:hearBeatON()
                        else
                            self:onNetLoginError()
                        end
                    end)


                end
            end)
    else
        assert(false,"网络已连接中.")
    end
end

function NetClient:retry(msg)
    UI:forceUnlock()

    local board = require("Views/Common/NetTipBoard2Btn"):create()
    board:setText(msg)
    --board:setButtonText('重试')

    board:setClickCB(function(name)
        UI:freeForce()
        
        if name == "left" then
            UI:closeSysBoard()

            local netWorkState = self.netWorkState

            if netWorkState == NetClient.NetWorkState.eLoging then
                --GameNet:resetNet(true)
                --PSDK.User:loginGame()
                APP.restart()
            elseif netWorkState == NetClient.NetWorkState.eReactiving then
                GameNet:resetNet(false)
                GameNet:reactiveGame()
            else
                assert(false)
            end
        else
            APP.restart()
        end
    end)

    UI:pushSysBoard(board,false,nil,true)
end

function NetClient:relogin(msg)
    UI:forceUnlock()

    local board = require("Views/Common/NetTipBoard"):create()
    board:setText(msg)
    board:setButtonText("重新登录")
    board:setClickCB(function()
        UI:freeForce()
        UI:closeSysBoard()
        GlobalScheduler.delayCallOnce(function()
            APP.restart()
        end)
    end)
    moduleGuider:stopGuider()
    UI:pushSysBoard(board,false,nil,true)
end

function NetClient:onNetError(code)
    local error = code
    local NetErrorType = NetClient.NetErrorType
end