--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc :	 串联的任务集合
]]
local iskindof = iskindof

local SerialTaskCollection = class("SerialTaskCollection" , TaskCollection)

function SerialTaskCollection:ctor( )
	SerialTaskCollection.super.ctor(self)


end

function SerialTaskCollection:process( )
	-- body
end


function SerialTaskCollection:getEnumerator( )
	return Enumerator.new(0 , 1 , handler(self , self.enumerator))	
end


function SerialTaskCollection:enumerator( )
	self._isRunning = true

	local startSize = self._taskQueue:size()

	while self._taskQueue:size() > 0 do

		local stack = Stack.new()

		stack:push(self._taskQueue:dequeue())

		while stack:size() > 0 do
			local task = stack:peek()			
			if type(task) == "function" then
				self._process = (startSize - self._taskQueue:size()) / startSize
				self._subProgress = 0

				local item = stack:pop()
				item(unpack(self._args[task]))
			elseif iskindof(task , "Enumerator") then
				if not task:moveNext() then
					stack:pop()
				end
			else
				if iskindof(task , "AsyncTask") then
					stack:push(task)
				else
					stack:push(task:getEnumerator())
				end

						-- if (ce is AsyncTask) //asyn
						-- 	_subProgress = (ce as AsyncTask).task.progress * (((float)(startingCount - (registeredEnumerators.Count)) / (float)startingCount) - progress);
						
						-- if (ce is EnumeratorWithProgress) //asyn
						-- 	_subProgress = (ce as EnumeratorWithProgress).progress * (((float)(startingCount - (registeredEnumerators.Count)) / (float)startingCount) - progress);

				--处理进度
			    if iskindof(task , "AsyncTask") then
			    	self._subProgress = task._progress * ((startSize - self._taskQueue:size()) / startSize - self._progress)
			    else

				end
			end

			coroutine.step()
		end
	end

	self._isRunning = false
end





return SerialTaskCollection