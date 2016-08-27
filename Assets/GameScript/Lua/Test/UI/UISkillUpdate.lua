--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

--[[
	Desc:
]]

UISkillUpdate = {}
local this = UISkillUpdate

--[[
	初始化，资源实例后执行，仅执行一次
	LuaUIPage  basePage   依附GameObject结点的UIPage对象  
]]
function UISkillUpdate.OnAwake(basePage)
    basePage:SetPage(EPageType.PopUp , EShowMode.NoNeedBack , ECollider.Normal)

    basePage:AddClick("desc/btn_Confirm" , function()
        UIManager:Hide(basePage.Name)
    end)
end

--[[
	界面显示，每次界面显示时都将执行
	object pageData   界面显示时的传参（可空）
]]
function UISkillUpdate.OnShow(pageData)


end

--[[
	界面隐藏，每次界面隐藏都将执行
]]
function UISkillUpdate.OnHide()


end

--[[
	界面销毁资源时调用，全生命周期，只会执行一次
]]
function UISkillUpdate.OnDestroy()

end

