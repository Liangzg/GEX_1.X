--[[
	Author： LiangZG
	Desc : 计时器
]]

local UpdateBeat = UpdateBeat

local Timer = class("Timer")
local m = Timer

function m:ctor(  )
	self:clear()
end

--设置计时器的开始时间和结束时间
--@param beginTime [number] 启始时间,单位秒
--@param totalTime [number] 计时量,单位秒
--@param timeUnit [eTimeUnit]  计时器单位间隔  
function m:setBeginTime( beginTime , totalTime , interval )
	self:setTime(beginTime , beginTime + totalTime , interval)
end

--设置计时器的开始时间和结束时间
--@param beginTime [number] 启始时间,单位秒
--@param endTime [number] 结束时间,单位秒
--@param timeUnit [eTimeUnit]  计时器单位间隔  
function m:setEndTime( endTime , totalTime , interval )
	self:setTime(endTime - totalTime , endTime , interval)
end

--设置计时器的开始时间和结束时间
--@param beginTime [number] 启始时间,单位秒
--@param totalTime [number] 计时量,单位秒
--@param timeUnit [eTimeUnit]  计时器单位间隔 
function m:setTime( beginTime , endTime , interval )
	self.beginTime = beginTime
	self.endTime = endTime
	self.totalTime = self.endTime - self.beginTime
	self.interval = interval
end

--是否忽略时间的缩放
--@param ignore bool  true表示忽略缩放（默认）
function m:ignoreTimeScale( ignore )
	self.ignoreScale = ignore
	return self
end

--启动计时器
-- delay 延迟时间
function m:start(delay)
	if self.runHandler then
		UpdateBeat:Remove(self.runHandler)
	end

	local curTime = TimeService.ins:totalMillisecondNow() / 1000
	self.elapseDeltaTime = Mathf.Max(0 , curTime - self.beginTime)
	self.lastElapseTime = self.elapseDeltaTime

	self.runHandler = handler(self , self._run)

	Scheduler.ins:startDelay(function ( )
		UpdateBeat:Add(self.runHandler)
	end , delay , true)
	
	self:_changeInvoke(self.elapseDeltaTime / self.totalTime)
end


function m:_run()
	if self.pause then	return 	end
	if self.elapseDeltaTime > self.totalTime then
		self:_changeInvoke(1)
		self:onDestroy()
		return 
	end

	self.elapseDeltaTime = self.elapseDeltaTime + self:_detalTime()

	if self.elapseDeltaTime - self.lastElapseTime >  self.interval then
		self.lastElapseTime = self.elapseDeltaTime
		self:_changeInvoke(self.elapseDeltaTime / self.totalTime)
	end
end


function m:_detalTime()
	if self.ignoreScale then
		return Time.unscaledDeltaTime
	end
	return Time.deltaTime
end


function m:pause( )
	self.pause = true
end

function m:resume( )
	self.pause = false
end

function m:_changeInvoke( process )
	if not self.changeFunc then	return 	end

	self.changeFunc(process)
end


function m:clear( )
	self.changeFunc = nil
	self.elapseDeltaTime = 0
	self.lastElapseTime = 0
	self.pause = false
	self.ignoreTimeScale = false
end

function m:onDestroy()
	if self.runHandler then
		UpdateBeat:Remove(self.runHandler)
		self.runHandler = nil
	end

	self:clear()
end


return Timer