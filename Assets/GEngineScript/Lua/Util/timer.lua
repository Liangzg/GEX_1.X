--[[
	Author： LiangZG
	Desc : 计时器
]]


local timeService = require "Util.timeService"

local UpdateBeat = UpdateBeat

local timer = class("timer")
local m = timer

function m:ctor(  )
	self:ignoreTimeScale(true)
	self:changes = list.new()
end

--设置计时器的开始时间和结束时间
--@param beginTime [number] 启始时间,单位秒
--@param totalTime [number] 计时量,单位秒
--@param timeUnit [eTimeUnit]  计时器单位 
function m:setBeginTime( beginTime , totalTime , timeUnit )
	self:setTime(beginTime , beginTime + totalTime , timeUnit)
end

--设置计时器的开始时间和结束时间
--@param beginTime [number] 启始时间,单位秒
--@param endTime [number] 结束时间,单位秒
--@param timeUnit [eTimeUnit]  计时器单位 
function m:setEndTime( endTime , totalTime , timeUnit )
	self:setTime(endTime - totalTime , endTime , timeUnit)
end

--设置计时器的开始时间和结束时间
--@param beginTime [number] 启始时间,单位秒
--@param totalTime [number] 计时量,单位秒
--@param timeUnit [eTimeUnit]  计时器单位 
function m:setTime( beginTime , endTime , timeUnit )
	self.beginTime = beginTime
	self.endTime = endTime
	self.totalTime = self.endTime - self.beginTime
	self.timeUnit = timeUnit
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

	self.delayTime = delayTime
	local curTime = timeService.ins:totalSecondServerNow()
	self.elapseDeltaTime = Mathf.Max(0 , curTime - self.beginTime)
	self.runHandler = handler(self , self._run)

	UpdateBeat:Add(self.runHandler)

end


function m:_run()
	if self.elapseDeltaTime > self.totalTime then
		self:onDestroy()
		return 
	end

	self.elapseDeltaTime = self.elapseDeltaTime + self:_detalTime()

	if self:executTime() then
		self.lastElapseTime = self.elapseDeltaTime
		self:_changeInvoke(self.elapseDeltaTime / self.totalTime)
	end
end


function m:_detalTime()
	if self.ignoreTimeScale then
		return Time.unscaledDeltaTime
	end
	return Time.deltaTime
end

function m:executTime()
	local deltaTime = self.elapseDeltaTime - self.lastElapseTime
	if self.timeUnit == eTimeUnit.MilliSecond then
		return   deltaTime >= 0.1 
	elseif self.timeUnit == eTimeUnit.Second then
		return deltaTime >= 1
	elseif self.timeUnit == eTimeUnit.Min then
		return deltaTime >= 60
	elseif self.timeUnit == eTimeUnit.Hour then
		return deltaTime >= 3600
	end

	error("cant find timeUnit:" .. tostring(self.timeUnit))
end


function m:_changeInvoke( process )
	
end


function m:clear( )
	-- body
end

function m:onDestroy()
	if self.runHandler then
		UpdateBeat:Remove(self.runHandler)
		self.runHandler = nil
	end


end




return timer