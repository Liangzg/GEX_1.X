--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]

local BaseState = require "Common.BaseState"

local LoginState = require "GameState.LoginState"
--[[
    更新资源主状态，用于启动时的资源更新
]]

local UpdateState = class("UpdateState" , BaseState)


function UpdateState:OnEnter()
    
    print("Update State ---> OnEnter")

end


function UpdateState:OnExit()
    print("Update State  <---- OnExit")
end

return UpdateState
