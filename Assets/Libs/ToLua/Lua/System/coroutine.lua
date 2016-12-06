--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local create = coroutine.create
local running = coroutine.running
local resume = coroutine.resume
local yield = coroutine.yield
local error = error
local unpack = unpack
local debug = debug
local FrameTimer = FrameTimer
local CoTimer = CoTimer

local comap = {}
setmetatable(comap, {__mode = "kv"})

function coroutine.start(f, ...)	
	local co = create(f)
	
	if running() == nil then
		local flag, msg = resume(co, ...)
	
		if not flag then		
			msg = debug.traceback(co, msg)					
			error(msg)				
		end					
	else
		local args = {...}
		local timer = nil
		
		local action = function()												
			local flag, msg = resume(co, unpack(args))			
	
			if not flag then				
				timer:Stop()				
				msg = debug.traceback(co, msg)				
				error(msg)						
			end		
		end
			
		timer = FrameTimer.New(action, 0, 1)
		comap[co] = timer
		timer:Start()		
	end

	return co
end



function coroutine.wait(t, co, ...)
	local args = {...}
	co = co or running()		
	local timer = nil
		
	local action = function()				
		local flag, msg = resume(co, unpack(args))
		
		if not flag then	
			timer:Stop()			
			msg = debug.traceback(co, msg)							
			error(msg)			
			return
		end
	end
	
	timer = CoTimer.New(action, t, 1)
	comap[co] = timer	
	timer:Start()
	return yield()
end

function coroutine.step(t, co, ...)
	local args = {...}
	co = co or running()		
	local timer = nil
	
	local action = function()						
		local flag, msg = resume(co, unpack(args))
	
		if not flag then							
			timer:Stop()					
			msg = debug.traceback(co, msg)					
			error(msg)
			return	
		end		
	end
				
	timer = FrameTimer.New(action, t or 1, 1)
	comap[co] = timer
	timer:Start()
	return yield()
end

--用于多层协同嵌套
--eg:
-- function onA()
-- 	print("a start")
-- 	coroutine.next(onB)
-- 	print("a end")
-- end

-- function onB()
-- 	print("b start")
-- 	coroutine.next(onC)
-- 	print("b end")
-- end

-- function onC()
-- 	print("c")
-- end

-- coroutine.start(onA)

-- --->output:
-- a start
-- b start
-- c
-- b end
-- a end

function coroutine.next( f , ... )
	local curCo = running()
	assert(curCo)
	
	if comap[curCo] then		
		comap[curCo]:Stop()
	end

	local timer = nil
	local co = nil	
	local action = function()	
		local state = coroutine.status(co)											
		if state ~= "dead" then	return end
		
		timer:Stop()
		local flag, msg = resume(curCo)			

		if not flag then				
			msg = debug.traceback(curCo, msg)				
			error(msg)						
		end		
	end
		
	timer = FrameTimer.New(action, 1, -1)
	comap[curCo] = timer
	timer:Start()	

	co = coroutine.start(f , unpack({...}))

	return yield()
end

function coroutine.www(www, co)			
	co = co or running()			
	local timer = nil			
			
	local action = function()				
		if not www.isDone then		
			return		
		end		
				
		timer:Stop()		
		local flag, msg = resume(co)		
			
		if not flag then						
			msg = debug.traceback(co, msg)						
			error(msg)			
			return			
		end				
	end		
					
	timer = FrameTimer.New(action, 1, -1)	
	comap[co] = timer	
 	timer:Start()
 	return yield()
end



function coroutine.stop(co)
 	local timer = comap[co]

 	if timer ~= nil then
 		comap[co] = nil
 		timer:Stop() 		
 	end
end
