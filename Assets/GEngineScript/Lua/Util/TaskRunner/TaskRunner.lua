
--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc :	任务执行器
]]

local TaskRunner = {}


function TaskRunner.run( task )

	coroutine.start(function (  )
		--print("current :" .. tostring(coroutine.running()))
		while task:moveNext() do
			local item = task:current()
			if item == "function" then
				item()
			end
		end
	end)
end

function TaskRunner.run2( task )
	local timer = nil
	local curTask = nil
	local action = function()				
		if not curTask.isDone then		
			return		
		end		
				
		if task:moveNext() then
			curTask = task:current()
 			coroutine.start(curTask)
		else		
			timer:Stop()	
		end							
	end		
					
	timer = FrameTimer.New(action, 1, -1)		
 	timer:Start()

 	curTask = task:current()
 	coroutine.start(curTask)
end




return TaskRunner