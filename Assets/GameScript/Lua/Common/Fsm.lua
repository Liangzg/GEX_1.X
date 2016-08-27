--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]


--[[
    有限状态机
]]

local Fsm = class("Fsm")

local UpdateBeat = UpdateBeat

function Fsm:ctor()
    self.state = nil
    
    self.globalState = nil
    
    UpdateBeat:Add(self.OnUpdate , self)
end

--[[
    状态切换
    FsmState nextState 下一个状态 
]]
function Fsm:OnEnter(nextState)

    if self.state ~= nil  then
        self.state:OnExit()
    end

    nextState:OnEnter()
    self.state = nextState
end


function Fsm:OnUpdate()

    if self.globalState ~= nil then
        self.globalState:OnUpdate()
    end

    if self.state ~= nil then
        self.state:OnUpdate()
    end

end


function Fsm:OnExit()
    
    if self.state ~= nil then
        self.state:OnExit()
    end
end

return Fsm
