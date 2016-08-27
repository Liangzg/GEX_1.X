--local NetREQ_ = NetREQ

NetProtoNotice = {}
NetProto.NetModule[20] = NetProtoNotice

NetProtoNotice.st = {
	['NoticeVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('endTime')	--截至时间
		T:vint64('id')	--公告id
		T:vint32('intervlTime')	--间隔时间
		T:vstring('message')	--公告内容
		T:vint32('priority')	--优先级
		T:vint64('startTime')	--开始时间
		T:vstring('title')	--公告标题
		T:vint32('type')	--公告类型：1 聊天栏公告；2 跑马灯公告；3 活动公告；4 更新公告
	end, 

}

NetProtoNotice.send = {
	['GetNotices'] = function(T) 
		if not(T:result()) then return end 
	end, 

}

-- ***自动提示帮助***

NetProtoNotice.NoticeVO = {
	['endTime'] = nil,	--截至时间
	['id'] = nil,	--公告id
	['intervlTime'] = nil,	--间隔时间
	['message'] = nil,	--公告内容
	['priority'] = nil,	--优先级
	['startTime'] = nil,	--开始时间
	['title'] = nil,	--公告标题
	['type'] = nil,	--公告类型：1 聊天栏公告；2 跑马灯公告；3 活动公告；4 更新公告
}


local T = NetProto.st

-- ***请求处理***
--获取公告
NetProtoNotice.SendGetNotices = {
	['new'] = NetProto.new,
	['_MOD_'] = 20,
	['_MED_'] = 1,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoNotice.send.GetNotices, NetProto.st.array(NetProtoNotice.st.NoticeVO), fnRespond)
		end,
}


-- ***推送处理***
NetProtoNotice.MesssagePush = {
	    [101] = {['msg']='msgPushAddToPlayers' ,['st'] = NetProtoNotice.st.NoticeVO},	--推送添加公告
	    [102] = {['msg']='msgPushDeleteToPlayers' ,['st'] = T.vint64},	--推送删除公告
	    [103] = {['msg']='msgPushUpdateToPlayers' ,['st'] = NetProtoNotice.st.NoticeVO},	--推送修改公告
}

DeclareInterface(NetProtoNotice, 'msgPushInterface')

function NetProtoNotice.listenMessagePush(listener)
	ListenInterface(NetProtoNotice, 'msgPushInterface', listener)
end

function NetProtoNotice.removeMessagePush(listener)
	RemoveInterface(NetProtoNotice, 'msgPushInterface', listener)
end
