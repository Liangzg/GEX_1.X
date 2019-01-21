--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

local Stack = class("Stack")

function Stack:ctor( initCapactity )
	self._capactity = initCapactity

	self._array = Array.new()
	self._size = 0
end


function Stack:push( item )
	if self._size == self._capactity then
		self._capactity = self._capactity * 2
		local newArr = Array.new()
		Array.copy(self._array , 1 , newArr , 1 , self._capactity)
		self._array = newArr
	end

	self._size = self._size + 1
	self._array:insert(self._size , item)
end


function Stack:pop( )
	if self._size == 0 then	return nil end

	local obj = self._array[self._size]
	self._array[self._size] = false
	self._size = self._size - 1
	return obj
end


function Stack:peek( )
	if self._size == 0 then	return nil end

	return self._array[self._size]
end


function Stack:contains( obj )
	return self._array:indexOf(obj) ~= -1 
end

function Stack:size( )
	return self._size
end

function Stack:clear( )
	self._array:clear()
	self._size = 0
end

function Stack:enumerator( )
	local i = self._size + 1
	return function ( )
		i = i - 1
		return self._array[i]
	end
end

function Stack:getEnumerator( )
	return Enumerator.new(self._size + 1, -1 , function ( i )
		return self._array[i]
	end)
end

function Stack:toString( )
	return self._array:toString()
end

return Stack
