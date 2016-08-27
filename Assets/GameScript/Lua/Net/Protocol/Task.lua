--local NetREQ_ = NetREQ

NetProtoTask = {}
NetProto.NetModule[11] = NetProtoTask

NetProtoTask.st = {
	['TaskUnitVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('doneNum')	--已做次数
		T:vint8('state')	--状态集合
		T:vint32('taskId')	--任务id
	end, 

	['TaskUnitsPushVO'] = function(T) 
		if not(T:result()) then return end 
		T:array('taskUnitVOs', NetProtoTask.st.TaskUnitVO)	--任务单元数组
		T:vint32('type')	--任务类型
	end, 

}

NetProtoTask.send = {
	['GetTaskUnitVOs'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('type')	--任务类型
	end, 

	['GetLastTaskUnitVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('type')	--任务类型
	end, 

	['GetAcceptableTaskIds'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('type')	--任务类型
	end, 

	['AcceptTask'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('taskId')	--任务ID
	end, 

	['AutoAcceptTasks'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('type')	--任务类型
	end, 

	['ReceiveRewards'] = function(T) 
		if not(T:result()) then return end 
		T:array('taskIds', T.vint32)	--任务ID数组
	end, 

	['ReceiveRewardAndAcceptNext'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('taskId')	--任务ID
	end, 

	['GetAppointTaskUnitVOs'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('type')	--任务类型
		T:vint32('gettingType')	--获取方式：1 全部，2 指定类型 3 排除指定类型
		T:array('secondTyps', T.vint32)	--第二类型数组
	end, 

	['PostSevenLoginTask'] = function(T) 
		if not(T:result()) then return end 
	end, 

}

-- ***自动提示帮助***

NetProtoTask.TaskUnitVO = {
	['doneNum'] = nil,	--已做次数
	['state'] = nil,	--状态集合
	['taskId'] = nil,	--任务id
}

NetProtoTask.TaskUnitsPushVO = {
	['taskUnitVOs'] = nil,	--任务单元数组
	['type'] = nil,	--任务类型
}


local T = NetProto.st

-- ***请求处理***
--列出所有任务
NetProtoTask.SendGetTaskUnitVOs = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 1,
	['type'] = nil,	--任务类型
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.GetTaskUnitVOs, NetProto.st.array(NetProtoTask.st.TaskUnitVO), fnRespond)
		end,
}

--获得最后一个任务
NetProtoTask.SendGetLastTaskUnitVO = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 2,
	['type'] = nil,	--任务类型
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.GetLastTaskUnitVO, NetProtoTask.st.TaskUnitVO, fnRespond)
		end,
}

--列出可接受任务
NetProtoTask.SendGetAcceptableTaskIds = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 3,
	['type'] = nil,	--任务类型
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.GetAcceptableTaskIds, NetProto.st.array(T.vint32), fnRespond)
		end,
}

--接受任务	--返回:状态码
NetProtoTask.SendAcceptTask = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 4,
	['taskId'] = nil,	--任务ID
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.AcceptTask, T.vint32, fnRespond)
		end,
}

--自动接受任务	--返回:状态码
NetProtoTask.SendAutoAcceptTasks = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 5,
	['type'] = nil,	--任务类型
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.AutoAcceptTasks, T.vint32, fnRespond)
		end,
}

--领取奖励	--返回:状态码
NetProtoTask.SendReceiveRewards = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 6,
	['taskIds'] = nil,	--任务ID数组
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.ReceiveRewards, T.vint32, fnRespond)
		end,
}

--领取奖励	--返回:状态码
NetProtoTask.SendReceiveRewardAndAcceptNext = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 7,
	['taskId'] = nil,	--任务ID
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.ReceiveRewardAndAcceptNext, T.vint32, fnRespond)
		end,
}

--列出指定任务
NetProtoTask.SendGetAppointTaskUnitVOs = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 8,
	['type'] = nil,	--任务类型
	['gettingType'] = nil,	--获取方式：1 全部，2 指定类型 3 排除指定类型
	['secondTyps'] = nil,	--第二类型数组
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.GetAppointTaskUnitVOs, NetProto.st.array(NetProtoTask.st.TaskUnitVO), fnRespond)
		end,
}

--触发七天登录任务	--返回:状态码
NetProtoTask.SendPostSevenLoginTask = {
	['new'] = NetProto.new,
	['_MOD_'] = 11,
	['_MED_'] = 9,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoTask.send.PostSevenLoginTask, T.vint32, fnRespond)
		end,
}


-- ***推送处理***
NetProtoTask.MesssagePush = {
	    [101] = {['msg']='msgPushTaskUnits' ,['st'] = NetProtoTask.st.TaskUnitsPushVO},	--推送任务单元
}

DeclareInterface(NetProtoTask, 'msgPushInterface')

function NetProtoTask.listenMessagePush(listener)
	ListenInterface(NetProtoTask, 'msgPushInterface', listener)
end

function NetProtoTask.removeMessagePush(listener)
	RemoveInterface(NetProtoTask, 'msgPushInterface', listener)
end
