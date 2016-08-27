--[[
Auth:LiangZG
like Unity Brocast Event System in lua.
]]

local EventLib = require "eventlib"

local Event = class("Event")

function Event:ctor()
    self.events = {}
end

function Event:AddListener(event,handler)
	if not event or type(event) ~= "string" then
		error("event parameter in addlistener function has to be string, " .. type(event) .. " not right.")
	end
	if not handler or type(handler) ~= "function" then
		error("handler parameter in addlistener function has to be function, " .. type(handler) .. " not right")
	end

	if not self.events[event] then
		--create the Event with name
		self.events[event] = EventLib:new(event)
	end

	--conn this handler
	self.events[event]:connect(handler)

    --print("add event : " .. event)
end

--广播事件
function Event:Brocast(event,...)
	if not self.events[event] then
		--error("brocast " .. event .. " has no event.")
        return false
	else
		self.events[event]:fire(...)
	end

    return true
end

--删除指定指定
function Event:RemoveListener(event,handler)
	if not self.events[event] then
		--error("remove " .. event .. " has no event.")
        return 
	else
		self.events[event]:disconnect(handler)
	end
end


--清空事件列表
function Event:Clear()
    for k , v in pairs(self.events) do
        v:DisconnectAll()
    end    
end

return Event