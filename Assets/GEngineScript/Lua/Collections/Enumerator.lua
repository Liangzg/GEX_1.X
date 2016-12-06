--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : 容器迭代器 ， 列表的下标从1开始
]]

local Enumerator = class("Enumerator")

function Enumerator:ctor( init, offset, func )
	self._pairs = func
	self._index = init
	self._nextOffset = offset
end

function Enumerator:current( )
	return self._current
end

function Enumerator:moveNext( )
	return self:enumerator()
end


function Enumerator:enumerator()
	self._index = self._index + self._nextOffset
	self._current = self._pairs(self._index)
	return self._current
end

return Enumerator