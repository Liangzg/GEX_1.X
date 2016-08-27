
NetSocket = {}
NetSocket = class("NetSocket",function()
    local o = core.SocketNet:new()
    return o
end)

NetSocket.moduleMsgPushMap = {} 

function NetSocket:registerModuleMsgPush(module_number,func)
    self.moduleMsgPushMap[module_number] = func
end
