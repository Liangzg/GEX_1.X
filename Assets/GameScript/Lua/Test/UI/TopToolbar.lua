--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

--[[
	Desc:顶部工具栏
]]

TopToolbar = {}
local this = TopToolbar

--[[
	初始化，资源实例后执行，仅执行一次
	GameObject  gObj   依附的GameObject结点  
]]
function TopToolbar.OnAwake(basePage)
    basePage:SetPage(EPageType.Fixed , EShowMode.None , ECollider.None)
    --以下这种方式，可行
    this.backBtn = this.transform:FindChild("BtnBack").gameObject

    --basePage:AddClick(this.backBtn:gameObject, this:BtnBackClick())   
     basePage:AddClick(this.backBtn , this.BtnBackClick)   

         --helpInfo
    basePage:AddClick("HelpInfo" , function()
        UIManager:Show("UINotice" , nil)
    end)

    --文本
    local barGObj = this.transform:Find("barText").gameObject
    this.barText = barGObj:GetComponent("Text")
end


function TopToolbar.BtnBackClick(gObj)
    print("Back Button Click")

    UIManager:AutoBackPage()
end

--[[
	界面显示，每次界面显示时都将执行
	object pageData   界面显示时的传参（可空）
]]
function TopToolbar.OnShow(pageData)

    this.event:AddListener("updateTopToolbar" , this.UpdateBarText)
    
end


function TopToolbar.UpdateBarText(msgTxt)

    print("topToolbar : " .. msgTxt) 

    this.barText.text = msgTxt
end

--[[
	界面隐藏，每次界面隐藏都将执行
]]
function TopToolbar.OnHide()


end

--[[
	界面销毁资源时调用，全生命周期，只会执行一次
]]
function TopToolbar.OnDestroy()

end

