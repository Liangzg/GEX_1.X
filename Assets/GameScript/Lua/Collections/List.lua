--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : 有序list 容器 ， 列表的下标从1开始
]]
local List = class("List")
local m = List


function m:ctor( capactity )
	self.capactity = capactity
	self._list = {}
	self._listHash = {}
end

--获取列表中的元素，index 从1开始
function m:get( index )
	if index > self:size() or index < 1 then return nil 	end

	return self._list[index]
end


function m:add( item )
	if self._listHash[item]	or self:isMaxCapactity() then	return 	end

	self._listHash[item] = true
	table.insert(self._list , item)
end


function m:isMaxCapactity( )
	return self.capactity and self:size() >= self.capactity or false
end

-- function m:addRange( collection )
-- 	-- body
-- end


function m:remove( item	)
	if not self._listHash[item] or self:isMaxCapactity() then	return 		end

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
	if index > self:size() or index < 1 or self:isMaxCapactity()  then		return 	end

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

--用于遍历
function m:enumerator()
	local index = 0
	return function ()
		index = index + 1
		return self._list[index]
	end
end


function m:toString()
	local str = {}
	
	local size = self:size()
	for i=1,size do
		table.insert(str , tostring(self._list[i]))
	end	

	return table.concat( str, ", ")
end

return List