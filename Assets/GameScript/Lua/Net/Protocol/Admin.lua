--local NetREQ_ = NetREQ

NetProtoAdmin = {}
NetProto.NetModule[200] = NetProtoAdmin

NetProtoAdmin.st = {
}

NetProtoAdmin.send = {
	['GetServerState'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['Maintenance'] = function(T) 
		if not(T:result()) then return end 
	end, 

}

-- ***自动提示帮助***


local T = NetProto.st

-- ***请求处理***
--获取服务器动态配置状态（1已设置；0未设置）
NetProtoAdmin.SendGetServerState = {
	['new'] = NetProto.new,
	['_MOD_'] = 200,
	['_MED_'] = 23,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoAdmin.send.GetServerState, T.vint32, fnRespond)
		end,
}

--维护服务器
NetProtoAdmin.SendMaintenance = {
	['new'] = NetProto.new,
	['_MOD_'] = 200,
	['_MED_'] = 24,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoAdmin.send.Maintenance, T.vint32, fnRespond)
		end,
}


-- ***推送处理***
NetProtoAdmin.MesssagePush = {
}

DeclareInterface(NetProtoAdmin, 'msgPushInterface')

function NetProtoAdmin.listenMessagePush(listener)
	ListenInterface(NetProtoAdmin, 'msgPushInterface', listener)
end

function NetProtoAdmin.removeMessagePush(listener)
	RemoveInterface(NetProtoAdmin, 'msgPushInterface', listener)
end
