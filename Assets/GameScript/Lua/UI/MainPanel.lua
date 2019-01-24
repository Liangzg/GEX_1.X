-- @Author: game.liangzg@foxmail.com
-- @Last Modified time: 2017-01-01 00:00:00
-- @Desc:主界面逻辑处理

MainPanel = {}
local this = MainPanel
local curUiId = 0

--- 由LuaBehaviour自动调用
function MainPanel.Awake(gameObject)

	this.widgets = {
		{field="root", path ="", src=LuaCanvas},
		{field="BtnRank",path="BtnRank",src=LuaButton, onClick = this._onClickRank },
		{field="BtnSkill",path="BtnSkill",src=LuaButton, onClick = this._onClickSkill },
		{field="BtnCombat",path="BtnCombat",src=LuaButton, onClick = this._onClickCombat },
		{field="BtnChat",path="BtnChat.BtnChat",src=LuaButton, onClick = this._onClickChat },

	}
	LuaUIHelper.bind(this.gameObject , MainPanel )
end

function MainPanel.Show( func )
    UIManager:Show("Prefab/GUI/MainPanel", nil)
	this.closeFunc = func
end

--- 由LuaBehaviour自动调用
function MainPanel.OnInit(basePage)
	this.basePage = basePage
	this.basePage:SetPage(EPageType.Normal , EShowMode.HideOther , ECollider.None)
	--每次显示自动修改UI中所有Panel的depth
	--LuaUIHelper.addUIDepth(this.gameObject , MainPanel)
	this._registeEvents(this.event)

end

-- 注册界面事件监听
function MainPanel._registeEvents(event)
	--ps:事件列表每次关闭时会自动清空，所以显示时每次都需要注册
	event:AddListener("gotoRank" , this.dispatchRankResultEvent)
	event:AddListener("chatmsg" , this.dispatchChatMsgEvent)
end

--捕获gotoRank事件
function MainPanel.dispatchRankResultEvent()
	print("Yes , I receive this msg")

	EventManager.SendEvent("updateTopToolbar" , "hehe , Global Event")
end

--捕获聊天事件
function MainPanel.dispatchChatMsgEvent(msg)
    
	print("msg ：" .. msg)
end

function MainPanel._onClickRank()
	print(" goto rank UI ")

	EventManager.SendEvent("gotoRank")
end

function MainPanel._coClickSkill()

	SkillPanel.Show()
end

function MainPanel._onClickCombat()

	BattlePanel.Show()
end


function MainPanel.gotoChat()
	print(" goto chat UI ")

	EventManager.SendEvent("chatmsg" , "oh~ have a msg !")
end

--- 关闭界面
function MainPanel._onClickClose( )
    UIManager.Hide(this.basePage.assetPath)
	if this.closeFunc ~= nil then
		this.closeFunc()
		this.closeFunc = nil
	end
end

--- 由LuaBehaviour自动调用
function MainPanel.OnClose()
    --LuaUIHelper.removeUIDepth(this.gameObject)  --还原全局深度
end

--- 由LuaBehaviour自动调用--
function MainPanel.OnDestroy()
	this.gameObject = nil
	this.transform = nil
	this.widgets = nil
end