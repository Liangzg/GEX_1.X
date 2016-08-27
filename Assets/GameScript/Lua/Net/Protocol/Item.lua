--local NetREQ_ = NetREQ

NetProtoItem = {}
NetProto.NetModule[4] = NetProtoItem

NetProtoItem.st = {
	['GoodVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint8('backpack')	--装备/道具背包号
		T:vint32('baseId')	--装备/道具基础ID
		T:vint16('count')	--装备/道具的数量
		T:vint64('expiration')	--失效时间,0为永久不失效
		T:vint8('goodsType')	--物品类型. 0:道具;1:装备
		T:vint16('gridIndex')	--格子索引,从0开始
		T:vint64('id')	--装备/道具的主键ID
	end, 

	['EquipVO'] = function(T) 
		if not(T:result()) then return end 
		T:vstring('attributes')	--[装备]装备的基础属性:属性下标1_属性编号1_属性值1|属性下标2_属性编号2_属性值2|...
		T:vint64('dressingId')	--[装备]穿戴对象ID
		T:vint32('fightCapacity')	--[装备]战斗力
		T:struct('goodVO', NetProtoItem.st.GoodVO)	--物品VO
		T:vint8('newState')	--对于玩家来说是否是新装备（是否刚刚获得）0:否；1：是
		T:vint8('starLevel')	--[装备]装备的星级
	end, 

	['ItemVO'] = function(T) 
		if not(T:result()) then return end 
		T:struct('goodVO', NetProtoItem.st.GoodVO)	--物品VO
	end, 

	['BackpackVO'] = function(T) 
		if not(T:result()) then return end 
		T:array('equipVOs', NetProtoItem.st.EquipVO)	--装备VO数组
		T:array('itemVOs', NetProtoItem.st.ItemVO)	--道具VO数组
	end, 

	['GridsExpandVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint16('expandNum')	--能扩展的格子数量
		T:vint16('itemCostNum')	--消耗解锁晶的数量
		T:vint16('itemOwnNum')	--拥有解封晶的数量
	end, 

	['RewardVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('baseId')	--[道具和装备有效]物品基础id
		T:vint32('count')	--奖励物品的数量
		T:vint32('goodsType')	--奖励的类型(详看GoodsType)
		T:vint32('starLevel')	--[装备有效]强化等级
	end, 

	['UseItemResultVO'] = function(T) 
		if not(T:result()) then return end 
		T:vint32('result')	--状态码
		T:array('rewardVOs', NetProtoItem.st.RewardVO)	--道具使用后的所得
	end, 

	['BulkOperationEquipVO'] = function(T) 
		if not(T:result()) then return end 
		T:array('equipVOs', NetProtoItem.st.EquipVO)	--装备VO数组
		T:vint32('resultCode')	--操作结果
	end, 

}

NetProtoItem.send = {
	['ListPackageGoods'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['TakeGoodsFromShopBackpack'] = function(T) 
		if not(T:result()) then return end 
		T:vint8('goodType')	--物品类型;0:道具,1:装备
		T:vint64('id')	--物品唯一id
	end, 

	['ShowGridsExpand'] = function(T) 
		if not(T:result()) then return end 
	end, 

	['ExpandGrids'] = function(T) 
		if not(T:result()) then return end 
		T:vint16('expandSize')	--扩展的大小
	end, 

	['QueryUserItem'] = function(T) 
		if not(T:result()) then return end 
		T:array('userItemIds', T.vint64)	--道具唯一id数组
	end, 

	['UseItem'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('userItemId')	--道具唯一id
		T:vint32('count')	--数量
	end, 

	['SellGoods'] = function(T) 
		if not(T:result()) then return end 
		T:array('userItemIds', T.vint64)	--道具唯一id数组
		T:array('counts', T.vint32)	--道具数量数组(对应道具id数组)
	end, 

	['QueryUserEquip'] = function(T) 
		if not(T:result()) then return end 
		T:array('userEquipIds', T.vint64)	--装备唯一id数组
	end, 

	['DressEquip'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('userPetId')	--兽兽id,0为角色
		T:vint64('userEquipId')	--装备唯一id
	end, 

	['UndressEquip'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('userEquipId')	--装备唯一id
	end, 

	['ListFightUserEquips'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('queryPlayerId')	--查询的玩家id,查自己可传0
	end, 

	['UndressAllEquip'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('roleId')	--换装角色id，魔王为0
	end, 

	['RedressAllEquip'] = function(T) 
		if not(T:result()) then return end 
		T:vint64('roleId')	--换装角色id，魔王为0
	end, 

}

-- ***自动提示帮助***

NetProtoItem.GoodVO = {
	['backpack'] = nil,	--装备/道具背包号
	['baseId'] = nil,	--装备/道具基础ID
	['count'] = nil,	--装备/道具的数量
	['expiration'] = nil,	--失效时间,0为永久不失效
	['goodsType'] = nil,	--物品类型. 0:道具;1:装备
	['gridIndex'] = nil,	--格子索引,从0开始
	['id'] = nil,	--装备/道具的主键ID
}

NetProtoItem.EquipVO = {
	['attributes'] = nil,	--[装备]装备的基础属性:属性下标1_属性编号1_属性值1|属性下标2_属性编号2_属性值2|...
	['dressingId'] = nil,	--[装备]穿戴对象ID
	['fightCapacity'] = nil,	--[装备]战斗力
	['goodVO'] = nil,	--物品VO
	['newState'] = nil,	--对于玩家来说是否是新装备（是否刚刚获得）0:否；1：是
	['starLevel'] = nil,	--[装备]装备的星级
}

NetProtoItem.ItemVO = {
	['goodVO'] = nil,	--物品VO
}

NetProtoItem.BackpackVO = {
	['equipVOs'] = nil,	--装备VO数组
	['itemVOs'] = nil,	--道具VO数组
}

NetProtoItem.GridsExpandVO = {
	['expandNum'] = nil,	--能扩展的格子数量
	['itemCostNum'] = nil,	--消耗解锁晶的数量
	['itemOwnNum'] = nil,	--拥有解封晶的数量
}

NetProtoItem.RewardVO = {
	['baseId'] = nil,	--[道具和装备有效]物品基础id
	['count'] = nil,	--奖励物品的数量
	['goodsType'] = nil,	--奖励的类型(详看GoodsType)
	['starLevel'] = nil,	--[装备有效]强化等级
}

NetProtoItem.UseItemResultVO = {
	['result'] = nil,	--状态码
	['rewardVOs'] = nil,	--道具使用后的所得
}

NetProtoItem.BulkOperationEquipVO = {
	['equipVOs'] = nil,	--装备VO数组
	['resultCode'] = nil,	--操作结果
}


local T = NetProto.st

-- ***请求处理***
--列出背包物品列表
NetProtoItem.SendListPackageGoods = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 1,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.ListPackageGoods, NetProtoItem.st.BackpackVO, fnRespond)
		end,
}

--从商城背包提取物品	--返回:状态码
NetProtoItem.SendTakeGoodsFromShopBackpack = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 3,
	['goodType'] = nil,	--物品类型;0:道具,1:装备
	['id'] = nil,	--物品唯一id
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.TakeGoodsFromShopBackpack, T.vint32, fnRespond)
		end,
}

--查看格子扩展信息	--返回:扩展格子信息VO
NetProtoItem.SendShowGridsExpand = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 4,
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.ShowGridsExpand, NetProtoItem.st.GridsExpandVO, fnRespond)
		end,
}

--扩展格子	--返回:状态码
NetProtoItem.SendExpandGrids = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 5,
	['expandSize'] = nil,	--扩展的大小
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.ExpandGrids, T.vint32, fnRespond)
		end,
}

--查询用户道具列表
NetProtoItem.SendQueryUserItem = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 21,
	['userItemIds'] = nil,	--道具唯一id数组
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.QueryUserItem, NetProto.st.array(NetProtoItem.st.ItemVO), fnRespond)
		end,
}

--使用用户道具	--返回:使用道具返回结果
NetProtoItem.SendUseItem = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 22,
	['userItemId'] = nil,	--道具唯一id
	['count'] = nil,	--数量
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.UseItem, NetProtoItem.st.UseItemResultVO, fnRespond)
		end,
}

--出售物品	--返回:状态码
NetProtoItem.SendSellGoods = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 23,
	['userItemIds'] = nil,	--道具唯一id数组
	['counts'] = nil,	--道具数量数组(对应道具id数组)
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.SellGoods, T.vint32, fnRespond)
		end,
}

--查询用户装备列表
NetProtoItem.SendQueryUserEquip = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 61,
	['userEquipIds'] = nil,	--装备唯一id数组
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.QueryUserEquip, NetProto.st.array(NetProtoItem.st.EquipVO), fnRespond)
		end,
}

--上装	--返回:状态码
NetProtoItem.SendDressEquip = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 62,
	['userPetId'] = nil,	--兽兽id,0为角色
	['userEquipId'] = nil,	--装备唯一id
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.DressEquip, T.vint32, fnRespond)
		end,
}

--下装	--返回:状态码
NetProtoItem.SendUndressEquip = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 63,
	['userEquipId'] = nil,	--装备唯一id
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.UndressEquip, T.vint32, fnRespond)
		end,
}

--查询所有穿在身上的装备
NetProtoItem.SendListFightUserEquips = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 64,
	['queryPlayerId'] = nil,	--查询的玩家id,查自己可传0
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.ListFightUserEquips, NetProto.st.array(NetProtoItem.st.EquipVO), fnRespond)
		end,
}

--一键脱装
NetProtoItem.SendUndressAllEquip = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 65,
	['roleId'] = nil,	--换装角色id，魔王为0
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.UndressAllEquip, NetProtoItem.st.BulkOperationEquipVO, fnRespond)
		end,
}

--一键换装
NetProtoItem.SendRedressAllEquip = {
	['new'] = NetProto.new,
	['_MOD_'] = 4,
	['_MED_'] = 66,
	['roleId'] = nil,	--换装角色id，魔王为0
	['send'] = function (self, fnRespond)
			GameNet:sendRequest(self, NetProtoItem.send.RedressAllEquip, NetProtoItem.st.BulkOperationEquipVO, fnRespond)
		end,
}


-- ***推送处理***
NetProtoItem.MesssagePush = {
	    [101] = {['msg']='msgPushBackpackVO' ,['st'] = NetProtoItem.st.BackpackVO},	--推送背包VO
}

DeclareInterface(NetProtoItem, 'msgPushInterface')

function NetProtoItem.listenMessagePush(listener)
	ListenInterface(NetProtoItem, 'msgPushInterface', listener)
end

function NetProtoItem.removeMessagePush(listener)
	RemoveInterface(NetProtoItem, 'msgPushInterface', listener)
end
