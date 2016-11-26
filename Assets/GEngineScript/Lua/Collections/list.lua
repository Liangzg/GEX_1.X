--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : 有序list 容器
]]
local list = class("list")
local m = list


function m:ctor( capactity )
	self.capactity = capactity
	self._list = {}
	self._listHash = {}
end


function m:add( item )
	if self._listHash[item]	then	return 	end

	self._listHash[item] = true
	table.insert(self._list , item)
end

-- function m:addRange( collection )
-- 	-- body
-- end


function m:remove( item	)
	if not self._listHash[item] then	return 		end

	self._listHash[item] = nil
	for i=self:size(),1,-1 do
		if self._list[i] == item then
			table.remove(self._list , i)
			return 
		end
	end
end

function m:removeAt( index )
	if index > self:size() or index < 1 then		return 	end

	local item = self._list[index]
	table.remove(self._list , index)
	self._listHash[item] = nil
end

-- function m:removeRange( ... )
-- 	-- body
-- end


function m:clear()
	self._listHash = {}
	self._list = {}
end


function m:size()
	return #self._list
end

function m:contains( item )
	return self._listHash[item]
end


-- function m:find( ... )
-- 	-- body
-- end


function m:insert( index , item )
	if index > self:size() or index < 1 then		return 	end

	table.insert(self._list , index , item)
	self._listHash[item] = true
end


-- function m:insertRange( ... )
-- 	-- body
-- end

-- function m:findIndex( ... )
-- 	-- body
-- end


function m:sort( compare )
	table.sort(self._list , compare)
end

function m:copyTo( ... )
	-- body
end


function m:tostring()
	local str = {}
	
	local size = self:size()
	for i=1,size do
		table.insert(str , self._list[i]:tostring())
	end	

	return table.concat( str, ", ")
end

local list