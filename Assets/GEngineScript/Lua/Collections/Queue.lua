--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : 队列
]]
local rawset = rawset

local Queue = class("Queue")

function Queue:ctor( capactity , growFactor)
	self._capactity = capactity or 32
	self._growFactor = (growFactor or 2) * 100
	self._array = Array.new()

	self._head = 1
	self._tail = 1
	self._size = 0
end


function Queue:enqueue( item )
	if self._size == self._capactity then
		local capactity = self._array:length() * self._growFactor / 100
		if capactity < self._array:length() + 4 then
			capactity = self._array:length() + 4
		end
		self:setCapactity(capactity)
	end
	self._array:insert(self._tail , item)
	self._tail = self._tail + 1
	self._size = self._size + 1
end

function Queue:dequeue(	)
	if self._size == 0  then	return nil end

	local item = self._array[self._head]
	self._array[self._head] = false
	self._head = self._head + 1
	self._size = self._size - 1
	return item
end

function Queue:peek( )
	if self._size == 0  then	return nil end

	return self._array[self._head]
end


function Queue.isEmpty(queue)
	return Array.isEmpty(queue._array)
end


function Queue:isFull()
	return self._array:length() == self._capactity
end

function Queue:contains( obj )
	return self._array:indexOf(obj) ~= -1
end

function Queue:clear( )
	self._head = 1
	self._tail = 1
	self._size = 0
	self._array:clear()
end


function Queue:setCapactity( capactity )
	-- print("----------------size:" .. self._size .. " , capactity:" .. capactity)
	-- print("src:" .. self._array:toString() .. ",Array:" .. print_lua_table(self._array))

	local objArr = Array.new()
	if self._head < self._tail then
		Array.copy(self._array , self._head , objArr , 1 , self._size)
	else
		print("head:" .. self._head .. " length:" .. (self._capactity - self._head))
		Array.copy(self._array , self._head , objArr , 1 , self._capactity - self._head)
		Array.copy(self._array , 1 , objArr , self._capactity  - self._head , self._tail)
	end

	
	self._capactity = capactity
	self._array = objArr
	self._size = self._size == capactity and 0 or self._size
	self._head = 1
end

function Queue:size()
	return self._size
end

function Queue:toString( )
	return self._array:toString()
end

return Queue