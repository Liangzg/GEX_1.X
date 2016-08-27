--local NetREQ_ = NetREQ

NetProtoApi = {}
NetProto.NetModule[201] = NetProtoApi

NetProtoApi.st = {
	['ProtocolFileVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('content')	--content
		T:vstring('fileName')	--fileName
	end, 

	['CppProtoCodesResultObject'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('result')	--result
		T:array('value', NetProtoApi.st.ProtocolFileVO)	--value
	end, 

	['LuaProtoCodesResultObject'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('result')	--result
		T:array('value', NetProtoApi.st.ProtocolFileVO)	--value
	end, 

}

NetProtoApi.send = {
	['ModuleInfo'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['CmdInfo'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('module')	--
	end, 

	['VoStruce'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('className')	--类型全名
	end, 

	['CppProtoCodes'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('module')	--模块号
	end, 

	['LuaProtoCodes'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('module')	--模块号
	end, 

}

-- ***自动提示帮助***

NetProtoApi.ProtocolFileVO = {
	['content'] = nil,	--content
	['fileName'] = nil,	--fileName
}

NetProtoApi.CppProtoCodesResultObject = {
	['result'] = nil,	--result
	['value'] = nil,	--value
}

NetProtoApi.LuaProtoCodesResultObject = {
	['result'] = nil,	--result
	['value'] = nil,	--value
}


local T = NetProto.st

-- ***请求处理***
--模块信息
NetProtoApi.SendModuleInfo = {
	['new'] = NetProto.new,
	['_MOD_'] = 201,
	['_MED_'] = 1,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoApi.send.ModuleInfo, NetProtoApi.st.ModuleInfoList, fnRespond)
		end,
}

--命令信息
NetProtoApi.SendCmdInfo = {
	['new'] = NetProto.new,
	['_MOD_'] = 201,
	['_MED_'] = 2,
	['module'] = nil,	--
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoApi.send.CmdInfo, NetProtoApi.st.CmdInfoList, fnRespond)
		end,
}

--VO的结构
NetProtoApi.SendVoStruce = {
	['new'] = NetProto.new,
	['_MOD_'] = 201,
	['_MED_'] = 3,
	['className'] = nil,	--类型全名
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoApi.send.VoStruce, NetProtoApi.st.VoStruceMap, fnRespond)
		end,
}

--c++协议代码
NetProtoApi.SendCppProtoCodes = {
	['new'] = NetProto.new,
	['_MOD_'] = 201,
	['_MED_'] = 4,
	['module'] = nil,	--模块号
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoApi.send.CppProtoCodes, NetProtoApi.st.CppProtoCodesResultObject, fnRespond)
		end,
}

--lua协议代码
NetProtoApi.SendLuaProtoCodes = {
	['new'] = NetProto.new,
	['_MOD_'] = 201,
	['_MED_'] = 5,
	['module'] = nil,	--模块号
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoApi.send.LuaProtoCodes, NetProtoApi.st.LuaProtoCodesResultObject, fnRespond)
		end,
}


-- ***推送处理***
NetProtoApi.MesssagePush = {
}

DeclareInterface(NetProtoApi, 'msgPushInterface')

function NetProtoApi.listenMessagePush(listener)
	ListenInterface(NetProtoApi, 'msgPushInterface', listener)
end

function NetProtoApi.removeMessagePush(listener)
	RemoveInterface(NetProtoApi, 'msgPushInterface', listener)
end
