-- @Author: game.liangzg@foxmail.com
-- @Last Modified time: 2017-01-01 00:00:00
-- @Desc: 顶部工具栏

TopToolbarPanel = {}
local this = TopToolbarPanel
local curUiId = 0

--- 由LuaBehaviour自动调用
function TopToolbarPanel.Awake(gameObject)
	
	this.widgets = {
		{field="root", path ="", src=LuaCanvas},
		{field="BtnBack",path="BtnBack",src=LuaButton, onClick = this._onBackClick },
		{field="HelpInfo",path="HelpInfo",src=LuaButton, onClick = this._onClickHelpInfo },
		{field="barText",path="barText",src=LuaText},

	}
	LuaUIHelper.bind(this.gameObject , TopToolbarPanel )
end

function TopToolbarPanel.Show( func )
    UIManager:Show("Prefab/GUI/TopToolbarPanel", nil)
	this.closeFunc = func
end

--- 由LuaBehaviour自动调用
function TopToolbarPanel.OnInit(basePage)
	this.basePage = basePage
	this.basePage:SetFixPage(2200 , EShowMode.None , ECollider.None)
	--每次显示自动修改UI中所有Panel的depth
	--LuaUIHelper.addUIDepth(this.gameObject , TopToolbarPanel)
	this._registeEvents(this.event)

end

-- 注册界面事件监听
function TopToolbarPanel._registeEvents(event)
    
end

function TopToolbarPanel._onBackClick()
	print("Back Button Click")

	UIManager:AutoBackPage()
end


function TopToolbarPanel._onClickHelpInfo()
	NoticePanel.Show("Notice" , nil)
end

--- 关闭界面
function TopToolbarPanel._onClickClose( )
	--panelMgr:ClosePanel("TopToolbarPanel")
	UIManager.Hide(this.basePage.assetPath)
	if this.closeFunc ~= nil then
		this.closeFunc()
		this.closeFunc = nil
	end
end

--- 由LuaBehaviour自动调用
function TopToolbarPanel.OnClose()
    --LuaUIHelper.removeUIDepth(this.gameObject)  --还原全局深度
end

--- 由LuaBehaviour自动调用--
function TopToolbarPanel.OnDestroy()
	this.gameObject = nil
	this.transform = nil
	this.widgets = nil
end