-- @Author: game.liangzg@foxmail.com
-- @Last Modified time: 2017-01-01 00:00:00
-- @Desc: 提示

NoticePanel = {}
local this = NoticePanel
local curUiId = 0

--- 由LuaBehaviour自动调用
function NoticePanel.Awake(gameObject)
	
	this.widgets = {
		{field="root", path ="", src=LuaCanvas},
		{field="btn_confim",path="content.btn_confim",src=LuaButton, onClick = this._onClickClose },

	}
	LuaUIHelper.bind(this.gameObject , NoticePanel )
end

function NoticePanel.Show( func )
    UIManager:Show("Prefab/GUI/Common/NoticePanel", nil)
	this.closeFunc = func
end

--- 由LuaBehaviour自动调用
function NoticePanel.OnInit(basePage)
	this.basePage = basePage	
	this.basePage:SetPage(EPageType.PopUp , EShowMode.None , ECollider.Normal)

	--每次显示自动修改UI中所有Panel的depth
	--LuaUIHelper.addUIDepth(this.gameObject , NoticePanel)
	this._registeEvents(this.event)

end

-- 注册界面事件监听
function NoticePanel._registeEvents(event)
    
end

--- 关闭界面
function NoticePanel._onClickClose( )
	UIManager:Hide(this.basePage.assetPath)
	if this.closeFunc ~= nil then
		this.closeFunc()
		this.closeFunc = nil
	end
end

--- 由LuaBehaviour自动调用
function NoticePanel.OnClose()
    --LuaUIHelper.removeUIDepth(this.gameObject)  --还原全局深度
end

--- 由LuaBehaviour自动调用--
function NoticePanel.OnDestroy()
	this.gameObject = nil
	this.transform = nil
	this.widgets = nil
end