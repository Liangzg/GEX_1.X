--local NetREQ_ = NetREQ

NetProtoMail = {}
NetProto.NetModule[18] = NetProtoMail

NetProtoMail.st = {
	['RewardVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('baseId')	--[道具和装备有效]物品基础id
		T:vint32('count')	--奖励物品的数量
		T:vint32('goodsType')	--奖励的类型(详看GoodsType)
		T:vint32('starLevel')	--[装备有效]强化等级
	end, 

	['MailVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('content')	--内容(是否提供,依据来源)
		T:vint64('expirationTime')	--有效期满时间
		T:vint64('id')	--邮件主键id
		T:array('rewardVOs', NetProtoMail.st.RewardVO)	--附件内容
		T:vint64('sendTime')	--发送时间
		T:vint64('senderId')	--发送者id,来源为系统邮件时必为0
		T:vstring('senderName')	--发送者名字,来源为系统邮件时必为空
		T:vint8('state')	--状态组合(二进制按位与,定义于MailViewState);0x01:已阅读,0x02:已领奖
		T:vstring('theme')	--主题(是否提供,依据来源)
		T:vint32('themeId')	--主题ID(依据来源使用)
	end, 

}

NetProtoMail.send = {
	['MailList'] = function(T) 
		if not(T:result()) then return end 
		T:vint8('mailType')	--邮件类型：0系统邮件，1个人邮件
	end, 

	['QueryMails'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('divideMailId')	--分界ID(最新已加载邮件的ID)
		T:vint8('mailType')	--邮件类型：0系统邮件，1个人邮件
	end, 

	['SendMail'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('friendId')	--好友Id
		T:vint8('themeId')	--主题
		T:vstring('content')	--内容
	end, 

	['MarkMailRead'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('mailId')	--邮件主键id
		T:vint8('mailType')	--邮件类型：0系统邮件，1个人邮件
	end, 

	['ReceiveMailRewards'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('mailId')	--邮件主键id
		T:vint8('mailType')	--邮件类型：0系统邮件，1个人邮件
	end, 

	['ReceiveAllMailRewards'] = function(T) 
		if not(T:result()) then return end 
		T:vint8('mailType')	--邮件类型：0系统邮件，1个人邮件
	end, 

}

-- ***自动提示帮助***

NetProtoMail.RewardVO = {
	['baseId'] = nil,	--[道具和装备有效]物品基础id
	['count'] = nil,	--奖励物品的数量
	['goodsType'] = nil,	--奖励的类型(详看GoodsType)
	['starLevel'] = nil,	--[装备有效]强化等级
}

NetProtoMail.MailVO = {
	['content'] = nil,	--内容(是否提供,依据来源)
	['expirationTime'] = nil,	--有效期满时间
	['id'] = nil,	--邮件主键id
	['rewardVOs'] = nil,	--附件内容
	['sendTime'] = nil,	--发送时间
	['senderId'] = nil,	--发送者id,来源为系统邮件时必为0
	['senderName'] = nil,	--发送者名字,来源为系统邮件时必为空
	['state'] = nil,	--状态组合(二进制按位与,定义于MailViewState);0x01:已阅读,0x02:已领奖
	['theme'] = nil,	--主题(是否提供,依据来源)
	['themeId'] = nil,	--主题ID(依据来源使用)
}


local T = NetProto.st

-- ***请求处理***
--邮件列表
NetProtoMail.SendMailList = {
	['new'] = NetProto.new,
	['_MOD_'] = 18,
	['_MED_'] = 1,
	['mailType'] = nil,	--邮件类型：0系统邮件，1个人邮件
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoMail.send.MailList, NetProto.st.array(NetProtoMail.st.MailVO), fnRespond)
		end,
}

--查询刚收到的最新邮件
NetProtoMail.SendQueryMails = {
	['new'] = NetProto.new,
	['_MOD_'] = 18,
	['_MED_'] = 2,
	['divideMailId'] = nil,	--分界ID(最新已加载邮件的ID)
	['mailType'] = nil,	--邮件类型：0系统邮件，1个人邮件
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoMail.send.QueryMails, NetProto.st.array(NetProtoMail.st.MailVO), fnRespond)
		end,
}

--发送邮件	--返回:状态码
NetProtoMail.SendSendMail = {
	['new'] = NetProto.new,
	['_MOD_'] = 18,
	['_MED_'] = 4,
	['friendId'] = nil,	--好友Id
	['themeId'] = nil,	--主题
	['content'] = nil,	--内容
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoMail.send.SendMail, T.vint32, fnRespond)
		end,
}

--标记邮件已读取	--返回:状态码
NetProtoMail.SendMarkMailRead = {
	['new'] = NetProto.new,
	['_MOD_'] = 18,
	['_MED_'] = 5,
	['mailId'] = nil,	--邮件主键id
	['mailType'] = nil,	--邮件类型：0系统邮件，1个人邮件
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoMail.send.MarkMailRead, T.vint32, fnRespond)
		end,
}

--领取邮件附件	--返回:状态码
NetProtoMail.SendReceiveMailRewards = {
	['new'] = NetProto.new,
	['_MOD_'] = 18,
	['_MED_'] = 6,
	['mailId'] = nil,	--邮件主键id
	['mailType'] = nil,	--邮件类型：0系统邮件，1个人邮件
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoMail.send.ReceiveMailRewards, T.vint32, fnRespond)
		end,
}

--领取全部邮件附件	--返回:状态码
NetProtoMail.SendReceiveAllMailRewards = {
	['new'] = NetProto.new,
	['_MOD_'] = 18,
	['_MED_'] = 7,
	['mailType'] = nil,	--邮件类型：0系统邮件，1个人邮件
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoMail.send.ReceiveAllMailRewards, T.vint32, fnRespond)
		end,
}


-- ***推送处理***
NetProtoMail.MesssagePush = {
}

DeclareInterface(NetProtoMail, 'msgPushInterface')

function NetProtoMail.listenMessagePush(listener)
	ListenInterface(NetProtoMail, 'msgPushInterface', listener)
end

function NetProtoMail.removeMessagePush(listener)
	RemoveInterface(NetProtoMail, 'msgPushInterface', listener)
end
