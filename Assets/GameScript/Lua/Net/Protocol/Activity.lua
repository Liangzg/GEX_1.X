--local NetREQ_ = NetREQ

NetProtoActivity = {}
NetProto.NetModule[29] = NetProtoActivity

NetProtoActivity.st = {
	['ActivityVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('content')	--活动内容
		T:vstring('des')	--描述
		T:vint64('endTime')	--结束时间
		T:vint64('id')	--活动id
		T:vstring('name')	--活动名称
		T:vstring('param')	--活动参数，有集体数据时才不为空
		T:vint32('resultCode')	--结果码
		T:vint64('startTime')	--开始时间
		T:vstring('subDes')	--子描述
		T:vstring('title')	--活动标签
		T:vint32('type')	--活动类型
		T:vstring('userData')	--用户数据
	end, 

	['PlayerActivityDataVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('param')	--活动参数，有集体数据时才不为空
		T:vint32('resultCode')	--结果码
		T:vstring('userData')	--用户数据
	end, 

}

NetProtoActivity.send = {
	['GetNowActivityIds'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['GetNowActivities'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['GetActivity'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('activityId')	--活动ID
	end, 

	['GetUserActivityData'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('activityId')	--活动ID
	end, 

	['ReceiveRewards'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('activityId')	--活动ID
		T:vint32('subId')	--子类ID
	end, 

}

-- ***自动提示帮助***

NetProtoActivity.ActivityVO = {
	['content'] = nil,	--活动内容
	['des'] = nil,	--描述
	['endTime'] = nil,	--结束时间
	['id'] = nil,	--活动id
	['name'] = nil,	--活动名称
	['param'] = nil,	--活动参数，有集体数据时才不为空
	['resultCode'] = nil,	--结果码
	['startTime'] = nil,	--开始时间
	['subDes'] = nil,	--子描述
	['title'] = nil,	--活动标签
	['type'] = nil,	--活动类型
	['userData'] = nil,	--用户数据
}

NetProtoActivity.PlayerActivityDataVO = {
	['param'] = nil,	--活动参数，有集体数据时才不为空
	['resultCode'] = nil,	--结果码
	['userData'] = nil,	--用户数据
}


local T = NetProto.st

-- ***请求处理***
--获取当前活动的id
NetProtoActivity.SendGetNowActivityIds = {
	['new'] = NetProto.new,
	['_MOD_'] = 29,
	['_MED_'] = 1,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoActivity.send.GetNowActivityIds, NetProto.st.array(T.vint64), fnRespond)
		end,
}

--获取当前活动
NetProtoActivity.SendGetNowActivities = {
	['new'] = NetProto.new,
	['_MOD_'] = 29,
	['_MED_'] = 2,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoActivity.send.GetNowActivities, NetProtoActivity.st.GetNowActivitiesList, fnRespond)
		end,
}

--获取活动
NetProtoActivity.SendGetActivity = {
	['new'] = NetProto.new,
	['_MOD_'] = 29,
	['_MED_'] = 3,
	['activityId'] = nil,	--活动ID
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoActivity.send.GetActivity, NetProtoActivity.st.ActivityVO, fnRespond)
		end,
}

--获取用户活动数据
NetProtoActivity.SendGetUserActivityData = {
	['new'] = NetProto.new,
	['_MOD_'] = 29,
	['_MED_'] = 4,
	['activityId'] = nil,	--活动ID
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoActivity.send.GetUserActivityData, NetProtoActivity.st.PlayerActivityDataVO, fnRespond)
		end,
}

--领取奖励
NetProtoActivity.SendReceiveRewards = {
	['new'] = NetProto.new,
	['_MOD_'] = 29,
	['_MED_'] = 5,
	['activityId'] = nil,	--活动ID
	['subId'] = nil,	--子类ID
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoActivity.send.ReceiveRewards, NetProtoActivity.st.PlayerActivityDataVO, fnRespond)
		end,
}


-- ***推送处理***
NetProtoActivity.MesssagePush = {
}

DeclareInterface(NetProtoActivity, 'msgPushInterface')

function NetProtoActivity.listenMessagePush(listener)
	ListenInterface(NetProtoActivity, 'msgPushInterface', listener)
end

function NetProtoActivity.removeMessagePush(listener)
	RemoveInterface(NetProtoActivity, 'msgPushInterface', listener)
end
