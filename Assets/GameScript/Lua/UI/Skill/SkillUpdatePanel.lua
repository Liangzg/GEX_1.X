-- @Author:
-- @Last Modified time: 2017-01-01 00:00:00
-- @Desc:

SkillUpdatePanel = {}
local this = SkillUpdatePanel
local curUiId = 0

--- 由LuaBehaviour自动调用
function SkillUpdatePanel.Awake(gameObject)
	
	this.widgets = {
		{field="root", path ="", src=LuaCanvas},
		{field="btn_Confirm",path="desc.btn_Confirm",src=LuaButton, onClick = this._onClickConfirm },

	}
	LuaUIHelper.bind(this.gameObject , SkillUpdatePanel )
end

function SkillUpdatePanel.Show( func )
    UIManager:Show("Prefab/GUI/Skill/SkillUpdatePanel", nil)
	this.closeFunc = func
end

--- 由LuaBehaviour自动调用
function SkillUpdatePanel.OnInit(basePage)
	this.basePage = basePage
	this.basePage:SetPage(EPageType.Normal , EShowMode.HideOther , ECollider.None)
	--每次显示自动修改UI中所有Panel的depth
	--LuaUIHelper.addUIDepth(this.gameObject , SkillUpdatePanel)
	this._registeEvents(this.event)

	

end

-- 注册界面事件监听
function SkillUpdatePanel._registeEvents(event)
    
end


function SkillUpdatePanel._onClickConfirm()
	
	UIManager:Hide(basePage.assetPath)

end

--- 关闭界面
function SkillUpdatePanel._onClickClose( )
    --panelMgr:ClosePanel("SkillUpdatePanel")
	if this.closeFunc ~= nil then
		this.closeFunc()
		this.closeFunc = nil
	end
end

--- 由LuaBehaviour自动调用
function SkillUpdatePanel.OnClose()
    --LuaUIHelper.removeUIDepth(this.gameObject)  --还原全局深度
end

--- 由LuaBehaviour自动调用--
function SkillUpdatePanel.OnDestroy()
	this.gameObject = nil
	this.transform = nil
	this.widgets = nil
end