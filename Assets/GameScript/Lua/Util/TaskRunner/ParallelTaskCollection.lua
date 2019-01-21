
--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc :	并行的任务集合
]]
local iskindof = iskindof

local ParallelTaskCollection = class("ParallelTaskCollection" , TaskCollection)

function ParallelTaskCollection:ctor( )
	ParallelTaskCollection.super.ctor(self)

	self._listOfStacks = List.new()
end

function ParallelTaskCollection:process( )
	-- body
end


function ParallelTaskCollection:getEnumerator( )
	return Enumerator.new(0 , 0 , handler(self , self.enumerator))	
end


function ParallelTaskCollection:enumerator( )
	self._isRunning = true
	self._listOfStacks:clear()

	for task in self._taskQueue:enumerator() do
		local stack = Stack.new()
		stack:push(task)
		self._listOfStacks:add(stack)
	end

	local startSize = self._listOfStacks:size()
	local size = startSize
	local rmList = List.new()

	while self._listOfStacks:size() > 0 do

		for stack in self._listOfStacks:enumerator() do
			if stack:size() > 0 then
				local task = stack:peek()			
				if type(task) == "function" then
					self._process = (size - startSize) / size
					startSize = startSize - 1

					local item = stack:pop()
					--print("args:" .. print_lua_table(self._args) .. ", arg:" .. print_lua_table(self._args[task]))
					item(unpack(self._args[task]))
				elseif iskindof(task , "Enumerator") then
					if not task:moveNext() then
						self._process = (size - startSize) / size
						startSize = startSize - 1
						stack:pop()
					end
				elseif iskindof(task , "AsyncTask") then
						stack:push(task)
				else
						stack:pop()
						stack:push(task:getEnumerator())
				end
			else
				rmList:add(stack)
			end
		end

		for stack in rmList:enumerator() do
			self._listOfStacks:remove(stack)
		end
		rmList:clear()

		coroutine.step()

	end

	self._isRunning = false
end





return ParallelTaskCollection