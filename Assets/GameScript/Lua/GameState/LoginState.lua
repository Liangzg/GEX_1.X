--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]

local BaseState = require "Common.BaseState"

--[[
    登录主状态，登录、选服、创号等功能
]]

local LoginState = class("LoginState" , BaseState)

function LoginState:OnEnter()
    
    print("LoginState ---> OnEnter")

end


function LoginState:OnExit()
    print("LoginState  <---- OnExit")
end

return LoginState