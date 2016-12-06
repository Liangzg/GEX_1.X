
--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc :	单一任务
]]
local iskindof = iskindof
local Enumerator = require "Collections.Enumerator"

local SingleTask = class("SingleTask" , Enumerator)


function SingleTask:ctor( task )
	SingleTask.super.ctor(self)

	if iskindof(task , "TaskCollection") then
		self._enumerator = task
	else
		self._task = SerialTaskCollection.new()
		self._task:add(task)
		self._enumerator = self._task:getEnumerator()
	end
end


function SingleTask:current( )
	return self._enumerator:current()
end


function SingleTask:moveNext( )
	if not self._enumerator:moveNext() then
		if self.onComplete then
			self.onComplete()
		end
		return false
	end

	return true
end


function SingleTask:reset( )
	--self._enumerator:reset()
end


return SingleTask