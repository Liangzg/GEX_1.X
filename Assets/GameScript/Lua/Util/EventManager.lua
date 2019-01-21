--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

local Event = require "events"

--[[
    Desc:全局事件管理器
]]
EventManager = {}
local this = EventManager
--事件池
local eventPool = {}

function EventManager.AddEvent(luaTableName)
    local luatable = this._findTable(luaTableName)

    if (not luatable) or luatable["event"] then
      print("Cant find table or Event system is already !")   
      return
    end

    luatable.event = Event.new()   
    eventPool[luaTableName] = luatable.event
end

--查找对应的Table
function EventManager._findTable(tableName)
    local func = loadstring("return " .. tableName)
    return func()
end

--全局广播事件
function EventManager.SendEvent(eventName , ...)

    local args = {...}
    for k , v in pairs(eventPool) do   
        if v:Brocast(eventName , unpack(args))  then
            return 
        end
    end
        
end

--清理一份LuaTable内的事件
function EventManager.ClearEvent(luaTableName)
    
    local luatable = this._findTable(luaTableName)

    if luatable and luatable["event"] then
        --print(" -------> clear event sytem : " .. luaTableName)
        luatable.event:Clear()
    end
end

