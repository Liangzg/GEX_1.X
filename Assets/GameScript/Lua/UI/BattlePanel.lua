-- @Author: game.liangzg@foxmai.com
-- @Last Modified time: 2017-01-01 00:00:00
-- @Desc: 战斗面板

BattlePanel = {}
local this = BattlePanel
local curUiId = 0

--- 由LuaBehaviour自动调用
function BattlePanel.Awake(gameObject)
	
	this.widgets = {
		{field="root", path ="", src=LuaCanvas},
		{field="btn_skill",path="btn_skill",src=LuaButton, onClick = this._onClickSkill },
		{field="btn_battle",path="btn_battle",src=LuaButton, onClick = this._onClickBattle },

	}
	LuaUIHelper.bind(this.gameObject , BattlePanel )
end

function BattlePanel.Show( func )
    UIManager:Show("Prefab/GUI/BattlePanel", nil)
	this.closeFunc = func
end

--- 由LuaBehaviour自动调用
function BattlePanel.OnInit(basePage)
	this.basePage = basePage

	this.basePage:SetPage(EPageType.Normal , EShowMode.HideOther , ECollider.Normal)

	--每次显示自动修改UI中所有Panel的depth
	--LuaUIHelper.addUIDepth(this.gameObject , BattlePanel)
	this._registeEvents(this.event)

end

-- 注册界面事件监听
function BattlePanel._registeEvents(event)
    
end


function BattlePanel._onClickSkill()
	SkillPanel.Show()
end

function BattlePanel._onClickBattle()
	print("goto battle scene!")
end


--- 关闭界面
		function BattlePanel._onClickClose( )
		--panelMgr:ClosePanel("BattlePanel")
	UIManager.Hide(this.basePage.assetPath)
	if this.closeFunc ~= nil then
		this.closeFunc()
		this.closeFunc = nil
	end
end

--- 由LuaBehaviour自动调用
function BattlePanel.OnClose()
    --LuaUIHelper.removeUIDepth(this.gameObject)  --还原全局深度
end

--- 由LuaBehaviour自动调用--
function BattlePanel.OnDestroy()
	this.gameObject = nil
	this.transform = nil
	this.widgets = nil
end