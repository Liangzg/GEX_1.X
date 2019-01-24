--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc:副本界面逻辑
]]

UIBattle = {}
local this = UIBattle

--[[
	初始化，资源实例后执行，仅执行一次
	LuaUIPage  basePage   依附GameObject结点的UIPage对象  
]]
function UIBattle.OnAwake(basePage)
    basePage:SetPage(EPageType.Normal , EShowMode.HideOther , ECollider.Normal)

    basePage:AddClick("btn_skill" , function()
        UIManager:Show("UISkill" , nil)
    end)

    basePage:AddClick("btn_battle" , function()
        print("goto battle scene!")
    end)
end

--[[
	界面显示，每次界面显示时都将执行
	object pageData   界面显示时的传参（可空）
]]
function UIBattle.OnShow(pageData)


end

--[[
	界面隐藏，每次界面隐藏都将执行
]]
function UIBattle.OnHide()


end

--[[
	界面销毁资源时调用，全生命周期，只会执行一次
]]
function UIBattle.OnDestroy()

end

