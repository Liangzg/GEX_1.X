--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]

local BaseState = require "Common.BaseState"

--[[
    主场景状态，游戏登录后的场景状态
]]

local MainState = class("MainState" , BaseState)


function MainState:OnEnter()
    
    print("MainState ---> OnEnter")

    --local obj = AssetLoader.LoadGameObject()
end


function MainState:OnExit()
    print("MainState  <---- OnExit")
end

return MainState