--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

--[[
	Desc:普通通知面板
]]

UINotice = {}
local this = UINotice

--[[
	初始化，资源实例后执行，仅执行一次
	LuaUIPage  basePage   依附GameObject结点的UIPage对象  
]]
function UINotice.OnAwake(basePage)
    basePage:SetPage(EPageType.PopUp , EShowMode.None , ECollider.Normal)

    --确认按钮关闭界面
    basePage:AddClick("content/btn_confim" , function()
        UIManager:Hide(basePage.Name)  
    end)
end

--[[
	界面显示，每次界面显示时都将执行
	object pageData   界面显示时的传参（可空）
]]
function UINotice.OnShow(pageData)


end

--[[
	界面隐藏，每次界面隐藏都将执行
]]
function UINotice.OnHide()


end

--[[
	界面销毁资源时调用，全生命周期，只会执行一次
]]
function UINotice.OnDestroy()

end

