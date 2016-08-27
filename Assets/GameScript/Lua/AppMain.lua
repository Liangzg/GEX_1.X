--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]

require "Common.functions"
local Fsm = require "Common.Fsm"
local UpdateState = require "GameState.UpdateState"
local LoginState = require "GameState.LoginState"
local MainState = require "GameState.MainState"

--[[
应用Main入口
]]
AppMain = {}

--主入口
function AppMain.Main()
    local machine = Fsm.new()
    AppMain.Fsm = machine
    
    machine:OnEnter(MainState.new())
end





--主应用释放
function AppMain.Destroy()

end

