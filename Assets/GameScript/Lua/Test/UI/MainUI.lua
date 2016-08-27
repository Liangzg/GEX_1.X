--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

--[[
	Desc:主界面逻辑处理
]]

MainUI = {}
local this = MainUI

--[[
	初始化，资源实例后执行，仅执行一次
	LuaUIPage  basePage   依附GameObject结点的UIPage对象  
]]
function MainUI.OnAwake(basePage)
    basePage:SetPage(EPageType.Normal , EShowMode.HideOther , ECollider.None)
    --展示了Button点击事件绑定文法
    
    --rank
    this.btnRank = this.transform:FindChild("BtnRank").gameObject
    basePage:AddClick(this.btnRank , this.gotoRankUI)

    --skill
    basePage:AddClick("BtnSkill" , function()
        --goto skill
        UIManager:Show("UISkill" , nil)
    end)

    --combat
    basePage:AddClick("BtnCombat" , MainUI.gotoCombatUI)

    --chat
    basePage:AddClick("Btn/BtnChat" , MainUI.gotoChat)

end



function MainUI.gotoRankUI()
    print(" goto rank UI ")

    EventManager.SendEvent("gotoRank")
end


function MainUI.gotoCombatUI()

    UIManager:Show("UIBattle" , nil)
end


function MainUI.gotoChat()
    print(" goto chat UI ")

    EventManager.SendEvent("chatmsg" , "oh~ have a msg !")
end


--[[
	界面显示，每次界面显示时都将执行
	object pageData   界面显示时的传参（可空）
]]
function MainUI.OnShow(pageData)

    --ps:事件列表每次关闭时会自动清空，所以显示时每次都需要注册
    this.event:AddListener("gotoRank" , this.dispatchRankResultEvent)
    this.event:AddListener("chatmsg" , this.dispatchChatMsgEvent)
end

--捕获gotoRank事件
function MainUI.dispatchRankResultEvent()
    print("Yes , I receive this msg")

    EventManager.SendEvent("updateTopToolbar" , "hehe , Global Event")
end

--捕获聊天事件
function MainUI.dispatchChatMsgEvent(msg)
    
   print("msg ：" .. msg)
end



--[[
	界面隐藏，每次界面隐藏都将执行
]]
function MainUI.OnHide()


end

--[[
	界面销毁资源时调用，全生命周期，只会执行一次
]]
function MainUI.OnDestroy()

end

