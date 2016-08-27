--local NetREQ_ = NetREQ

NetProtoUser = {}
NetProto.NetModule[1] = NetProtoUser

NetProtoUser.st = {
	['PlayerInfoVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('heroId')	--魔王Id
		T:vint64('id')	--角色ID
		T:vint32('level')	--角色等级
		T:vstring('name')	--角色名字
		T:vint32('qualityIdentity')	--魔王升品标识
		T:vint32('star')	--魔王星级
	end, 

	['AccountVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('accountName')	--账号名
		T:array('players', NetProtoUser.st.PlayerInfoVO)	--角色列表
		T:vint32('result')	--返回结果
		T:vint32('userType')	--用户类型:1游客;2QQ;3微信;4爱苹果;5快用苹果助手;6PP助手;7同步推;8初见;9爱思
		T:vstring('validKey')	--验证密匙
	end, 

	['CreatePlayerVO'] = function(T) 
		if not(T:result()) then return end 
		T:array('players', NetProtoUser.st.PlayerInfoVO)	--角色列表
		T:vint32('result')	--返回结果
	end, 

	['BindPlayerVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('bindTime')	--绑定时间
		T:vint32('result')	--返回结果
		T:vint32('timeZone')	--时区
	end, 

	['IkIvVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('key')	--整形键名
		T:vint32('value')	--整形键值
	end, 

	['IkLvVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('key')	--整形键名
		T:vint64('value')	--长整形键值
	end, 

	['IkSvVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('key')	--整形键名
		T:vstring('value')	--字符串型键值
	end, 

	['UnitId'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('id')	--id
		T:vint32('unitType')	--unitType
	end, 

	['AttributesVO'] = function(T) 
		if not(T:result()) then return end 
		T:array('ikIvVOs', NetProtoUser.st.IkIvVO)	--整形值属性VO数组
		T:array('ikLvVOs', NetProtoUser.st.IkLvVO)	--长整形值属性VO数组
		T:array('ikSvVOs', NetProtoUser.st.IkSvVO)	--字符串属性VO数组
		T:struct('unitId', NetProtoUser.st.UnitId)	--单位ID
	end, 

}

NetProtoUser.send = {
	['HeartBeat'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['VisitorLogin'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('userName')	--用户名
		T:vstring('password')	--密码
	end, 

	['Create'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('userName')	--用户名
		T:vstring('validKey')	--验证码
		T:vint32('heroId')	--魔王id
		T:vstring('name')	--角色名
	end, 

	['Select'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('username')	--用户名
		T:vstring('validKey')	--验证码
		T:vint64('selectPlayerId')	--所选的角色id
	end, 

	['Reconnect'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('username')	--用户名
		T:vstring('validKey')	--验证码
		T:vint64('selectPlayerId')	--所选的角色id
		T:vint64('firstBindTime')	--首次绑定时间
	end, 

	['LoginForOurpalm'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('tokenId')	--tokenId
		T:vstring('channel')	--channel
	end, 

}

-- ***自动提示帮助***

NetProtoUser.PlayerInfoVO = {
	['heroId'] = nil,	--魔王Id
	['id'] = nil,	--角色ID
	['level'] = nil,	--角色等级
	['name'] = nil,	--角色名字
	['qualityIdentity'] = nil,	--魔王升品标识
	['star'] = nil,	--魔王星级
}

NetProtoUser.AccountVO = {
	['accountName'] = nil,	--账号名
	['players'] = nil,	--角色列表
	['result'] = nil,	--返回结果
	['userType'] = nil,	--用户类型:1游客;2QQ;3微信;4爱苹果;5快用苹果助手;6PP助手;7同步推;8初见;9爱思
	['validKey'] = nil,	--验证密匙
}

NetProtoUser.CreatePlayerVO = {
	['players'] = nil,	--角色列表
	['result'] = nil,	--返回结果
}

NetProtoUser.BindPlayerVO = {
	['bindTime'] = nil,	--绑定时间
	['result'] = nil,	--返回结果
	['timeZone'] = nil,	--时区
}

NetProtoUser.IkIvVO = {
	['key'] = nil,	--整形键名
	['value'] = nil,	--整形键值
}

NetProtoUser.IkLvVO = {
	['key'] = nil,	--整形键名
	['value'] = nil,	--长整形键值
}

NetProtoUser.IkSvVO = {
	['key'] = nil,	--整形键名
	['value'] = nil,	--字符串型键值
}

NetProtoUser.UnitId = {
	['id'] = nil,	--id
	['unitType'] = nil,	--unitType
}

NetProtoUser.AttributesVO = {
	['ikIvVOs'] = nil,	--整形值属性VO数组
	['ikLvVOs'] = nil,	--长整形值属性VO数组
	['ikSvVOs'] = nil,	--字符串属性VO数组
	['unitId'] = nil,	--单位ID
}


local T = NetProto.st

-- ***请求处理***
--心跳	--返回:当前时间戳
NetProtoUser.SendHeartBeat = {
	['new'] = NetProto.new,
	['_MOD_'] = 1,
	['_MED_'] = 0,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoUser.send.HeartBeat, T.vint64, fnRespond)
		end,
}

--游客登陆
NetProtoUser.SendVisitorLogin = {
	['new'] = NetProto.new,
	['_MOD_'] = 1,
	['_MED_'] = 1,
	['userName'] = nil,	--用户名
	['password'] = nil,	--密码
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoUser.send.VisitorLogin, NetProtoUser.st.AccountVO, fnRespond)
		end,
}

--创建角色
NetProtoUser.SendCreate = {
	['new'] = NetProto.new,
	['_MOD_'] = 1,
	['_MED_'] = 2,
	['userName'] = nil,	--用户名
	['validKey'] = nil,	--验证码
	['heroId'] = nil,	--魔王id
	['name'] = nil,	--角色名
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoUser.send.Create, NetProtoUser.st.CreatePlayerVO, fnRespond)
		end,
}

--选择角色登陆	--返回:绑定信息
NetProtoUser.SendSelect = {
	['new'] = NetProto.new,
	['_MOD_'] = 1,
	['_MED_'] = 3,
	['username'] = nil,	--用户名
	['validKey'] = nil,	--验证码
	['selectPlayerId'] = nil,	--所选的角色id
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoUser.send.Select, NetProtoUser.st.BindPlayerVO, fnRespond)
		end,
}

--断线重连	--返回:绑定信息
NetProtoUser.SendReconnect = {
	['new'] = NetProto.new,
	['_MOD_'] = 1,
	['_MED_'] = 15,
	['username'] = nil,	--用户名
	['validKey'] = nil,	--验证码
	['selectPlayerId'] = nil,	--所选的角色id
	['firstBindTime'] = nil,	--首次绑定时间
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoUser.send.Reconnect, NetProtoUser.st.BindPlayerVO, fnRespond)
		end,
}

--掌趣登录
NetProtoUser.SendLoginForOurpalm = {
	['new'] = NetProto.new,
	['_MOD_'] = 1,
	['_MED_'] = 35,
	['tokenId'] = nil,	--tokenId
	['channel'] = nil,	--channel
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoUser.send.LoginForOurpalm, NetProtoUser.st.AccountVO, fnRespond)
		end,
}


-- ***推送处理***
NetProtoUser.MesssagePush = {
	    [100] = {['msg']='msgPushPlayerKickoff' ,['st'] = T.vint32},	--推送玩家下线,0:重登,1:封禁,2:登录超时,3:服务器维护
	    [101] = {['msg']='msgPushPlayerAttribute' ,['st'] = NetProtoUser.st.AttributesVO},	--推送角色的属性
	    [103] = {['msg']='msgPushVisitor' ,['st'] = T.vstring},	--推送访问者的名字
	    [104] = {['msg']='msgPushPackageFull' ,['st'] = T.vint32},	--推送背包已满，清理后从邮件领取物品信息
}

DeclareInterface(NetProtoUser, 'msgPushInterface')

function NetProtoUser.listenMessagePush(listener)
	ListenInterface(NetProtoUser, 'msgPushInterface', listener)
end

function NetProtoUser.removeMessagePush(listener)
	RemoveInterface(NetProtoUser, 'msgPushInterface', listener)
end
