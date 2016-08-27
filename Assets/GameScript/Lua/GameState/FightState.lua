--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]

local BaseState = require "Common.BaseState"

--[[
    战斗状态，玩法展示状态
]]

local FightState = class("FightState" , BaseState)

function FightState:OnEnter()
    
    print("FightState ---> OnEnter")

end


function FightState:OnExit()
    print("FightState  <---- OnExit")
end

return FightState