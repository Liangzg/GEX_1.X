-- @Author: game.liangzg@foxmail.com
-- @Last Modified time: 2017-01-01 00:00:00
-- @Desc: 技能面板

SkillPanel = {}
local this = SkillPanel
local curUiId = 0

--- 由LuaBehaviour自动调用
function SkillPanel.Awake(gameObject)

	this.widgets = {
		{field="root", path ="", src=LuaCanvas},
		{field="btn_upgrade",path="desc.btn_upgrade",src=LuaButton, onClick = this._onClickUpgrade },
		{field="btn_battle",path="desc.btn_battle",src=LuaButton, onClick = this._onClickBattle },

	}
	LuaUIHelper.bind(this.gameObject , SkillPanel )
end

function SkillPanel.Show( func )
    UIManager:Show("Prefab/GUI/Skill/SkillPanel", nil)
	this.closeFunc = func
end

--- 由LuaBehaviour自动调用
function SkillPanel.OnInit(basePage)
	this.basePage = basePage
	this.basePage:SetPage(EPageType.Normal , EShowMode.HideOther , ECollider.None)

	--每次显示自动修改UI中所有Panel的depth
	--LuaUIHelper.addUIDepth(this.gameObject , SkillPanel)
	this._registeEvents(this.event)

end

-- 注册界面事件监听
function SkillPanel._registeEvents(event)
    
end

---打开技能面板
function SkillPanel._onClickUpgrade()
	SkillUpdatePanel.Show()
end

--- 开始战斗
function SkillPanel._onClickBattle()
	BattlePanel.Show()
end

--- 关闭界面
function SkillPanel._onClickClose( )
    --panelMgr:ClosePanel("SkillPanel")
	if this.closeFunc ~= nil then
		this.closeFunc()
		this.closeFunc = nil
	end
end

--- 由LuaBehaviour自动调用
function SkillPanel.OnClose()
    --LuaUIHelper.removeUIDepth(this.gameObject)  --还原全局深度
end

--- 由LuaBehaviour自动调用--
function SkillPanel.OnDestroy()
	this.gameObject = nil
	this.transform = nil
	this.widgets = nil
end