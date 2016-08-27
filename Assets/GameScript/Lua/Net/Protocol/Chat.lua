--local NetREQ_ = NetREQ

NetProtoChat = {}
NetProto.NetModule[7] = NetProtoChat

NetProtoChat.st = {
	['SkIvVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('key')	--字符串型键名
		T:vint32('value')	--整形键值
	end, 

	['ChatInfo'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('channel')	--channel
		T:vstring('content')	--content
		T:vint64('playerId')	--playerId
		T:vstring('playerName')	--playerName
		T:vint64('targetId')	--targetId
		T:vstring('targetName')	--targetName
	end, 

}

NetProtoChat.send = {
	['CommonChat'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('content')	--聊天的信息
		T:vint32('channel')	--0系统,1世界,2帮派,3私聊
		T:vstring('targetName')	--私聊目标的名字(私聊需要)
	end, 

	['InputGMCode'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('content')	--命令内容
	end, 

	['Bgm'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('content')	--命令内容,用英文逗号','隔开
	end, 

	['WordChatOpen'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['WordChatClose'] = function(T) 
		if not(T:result()) then return end 
	end, 

}

-- ***自动提示帮助***

NetProtoChat.SkIvVO = {
	['key'] = nil,	--字符串型键名
	['value'] = nil,	--整形键值
}

NetProtoChat.ChatInfo = {
	['channel'] = nil,	--channel
	['content'] = nil,	--content
	['playerId'] = nil,	--playerId
	['playerName'] = nil,	--playerName
	['targetId'] = nil,	--targetId
	['targetName'] = nil,	--targetName
}


local T = NetProto.st

-- ***请求处理***
--角色聊天
NetProtoChat.SendCommonChat = {
	['new'] = NetProto.new,
	['_MOD_'] = 7,
	['_MED_'] = 1,
	['content'] = nil,	--聊天的信息
	['channel'] = nil,	--0系统,1世界,2帮派,3私聊
	['targetName'] = nil,	--私聊目标的名字(私聊需要)
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoChat.send.CommonChat, T.vint32, fnRespond)
		end,
}

--GM命令
NetProtoChat.SendInputGMCode = {
	['new'] = NetProto.new,
	['_MOD_'] = 7,
	['_MED_'] = 2,
	['content'] = nil,	--命令内容
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoChat.send.InputGMCode, T.vint32, fnRespond)
		end,
}

--批量GM命令	--返回:key=>命令内容,value=>返回结果
NetProtoChat.SendBgm = {
	['new'] = NetProto.new,
	['_MOD_'] = 7,
	['_MED_'] = 3,
	['content'] = nil,	--命令内容,用英文逗号','隔开
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoChat.send.Bgm, NetProto.st.array(NetProtoChat.st.SkIvVO), fnRespond)
		end,
}

--开启世界聊天	--返回:聊天信息数组
NetProtoChat.SendWordChatOpen = {
	['new'] = NetProto.new,
	['_MOD_'] = 7,
	['_MED_'] = 4,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoChat.send.WordChatOpen, NetProto.st.array(NetProtoChat.st.ChatInfo), fnRespond)
		end,
}

--关闭世界聊天
NetProtoChat.SendWordChatClose = {
	['new'] = NetProto.new,
	['_MOD_'] = 7,
	['_MED_'] = 5,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoChat.send.WordChatClose, T.vint32, fnRespond)
		end,
}


-- ***推送处理***
NetProtoChat.MesssagePush = {
	    [101] = {['msg']='msgPushToOnline' ,['st'] = NetProtoChat.st.ChatInfo},	--世界聊天(推送给在线的)
	    [102] = {['msg']='msgPushToPlayers' ,['st'] = NetProtoChat.st.ChatInfo},	--推送给指定玩家
}

DeclareInterface(NetProtoChat, 'msgPushInterface')

function NetProtoChat.listenMessagePush(listener)
	ListenInterface(NetProtoChat, 'msgPushInterface', listener)
end

function NetProtoChat.removeMessagePush(listener)
	RemoveInterface(NetProtoChat, 'msgPushInterface', listener)
end
