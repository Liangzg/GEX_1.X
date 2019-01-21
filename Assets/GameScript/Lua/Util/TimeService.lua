--[[
    Author： LiangZG
    Desc : 服务器时间
]]

local TimeService = class("TimeService")
local m = TimeService
local orgintime = os.time({year=1970 , month=1 , day = 1 , hour = 8 , min = 0 , sec = 1})

function m:ctor()
    self.serverTimeZoneDiff = 0
    self.lastTimeval = 0
end

function m:adjustServerTime(time)
    self.serverTime  = time
    self.localTime   = os.time()
    self.lastTimeval = os.time()
end

function m:setServerTimeZoneDiff(zoneDiffTime)
    self.serverTimeZoneDiff = zoneDiffTime * 3600
end

--/** 获取服务器当前UTC时间 */
function m:getServerUtcTimeNow()
   local nowTimeval = os.time()
   local serverNowUtcTime = self.serverTime + os.difftime(nowTimeval,self.lastTimeval)
   return serverNowUtcTime
end

--获得当前服务器时间，累加指定毫秒数后的时间
function m:addTimeNow(hour , min , sec)
    return self:addTime(self:getServerUtcTimeNow() , hour , min , sec)
end

--获得一个累加的新time时间
function m:addTime(orginTime , hour , min , sec)   
    local curTime = os.date('*t' , orginTime) 
    curTime.hour = curTime.hour + hour
    curTime.min = curTime.min + (min or 0)
    curTime.sec = curTime.sec + (sec or 0)
    return os.time(curTime)
end

--获得当前服务器时间，累加指定毫秒数后的时间
function m:addMillisecondNow(millisecond)    
    return self:addMillisecond(self:getServerUtcTimeNow() , millisecond)
end

--获得指定时间，累加指定毫秒数后的时间
function m:addMillisecond(orginTime , millisecond)
    local sec = millisecond / 1000
    local hour = Mathf.ToInt(sec / 3600)
    local min = Mathf.ToInt((sec - hour * 3600) / 60)
    sec = sec % 60

    local curTime = os.date('*t' , orginTime) 
    curTime.hour = curTime.hour + hour
    curTime.min = curTime.min + min
    curTime.sec = curTime.sec + sec 
    return os.time(curTime)
end

--获取总的毫秒数，基于1970.1.1 0:0:0
function m:totalMillisecondNow()
    local curTime = self:getServerUtcTimeNow()
    return self:totalMillisecond(curTime)
end

--获取总的毫秒数，基于1970.1.1 0:0:0
function m:totalMillisecond(time)    
    return os.difftime(time , orgintime) * 1000
end

--获取相对于服务器当前时间的总秒数
function m:totalSecondServerNow( time )
    return os.difftime(time , self:getServerUtcTimeNow())
end

--计算两个时间的时间差，startTime 和 endTime 单位为秒
function m:totalSeconds(startTime , endTime)
    if startTime ~= "number" then
        startTime = convert2num(startTime)
    end
    if endTime ~= "number" then
        endTime = convert2num(endTime)
    end

    return os.difftime(endTime , startTime)
end

--/** 获取服务器当前时区时间 */
function m:getServerZoneTimeNow()
    local t = self:getServerUtcTimeNow()
    t = t + self.serverTimeZoneDiff
    return t
end

--/** 获取服务器当前UTC日期 */
function m:getServerUtcDateNow()
    local t = self:getServerUtcTimeNow()
    return os.date('*t',t)
end

--/** 获取服务器当前时区日期 */
function m:getServerZoneDateNow()
    local t = self:getServerUtcTimeNow()
    t = t + self.serverTimeZoneDiff
    return os.date('*t',t)
end

--/**转换服务器时间戳 */
function m:changeTimeStampToOs(time)
    local tm = os.date('*t',time)
    return tm
end

--/**转换时间戳 */
function m:changeTimeStampToStr(time)
    local tm = os.date('*t',time)
    local yearStr  = ''
    local dayStr   = ''
    local monthStr = ''
    local hourStr  = ''
    local minStr   = ''
    
    yearStr  = tostring(tm.year)
    monthStr = string.format("%2d" , tm.month)
    dayStr = string.format("%2d" , tm.day)
    hourStr = string.format("%2d" , tm.hour)
    minStr = string.format("%2d" , tm.min)
    secStr = string.format("%2d" , tm.sec)
    
    return yearStr.."."..monthStr.."."..dayStr.." "..hourStr..":"..minStr
end



--/**转换时间戳 */
function m:timeToStrOnly()
    local tm = os.date("*t", os.time());
    local yearStr  = ''
    local dayStr   = ''
    local monthStr = ''
    local hourStr  = ''
    local minStr   = ''
    local secStr   = ''
    yearStr  = tostring(tm.year)
    monthStr = string.format("%2d" , tm.month)
    dayStr = string.format("%2d" , tm.day)
    hourStr = string.format("%2d" , tm.hour)
    minStr = string.format("%2d" , tm.min)
    secStr = string.format("%2d" , tm.sec)

    return yearStr..monthStr..dayStr..hourStr..minStr..secStr
end

--格式化总秒数为时、分、钞的值
function m:formatHMS(seconds)
    local hour = Mathf.ToInt(seconds / 3600)
    local min = Mathf.ToInt((seconds % 3600) / 60)
    local sec = seconds % 60
    return hour , min , sec
end


TimeService.ins = TimeService.new()

return TimeService