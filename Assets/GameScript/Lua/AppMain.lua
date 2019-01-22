--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com

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

