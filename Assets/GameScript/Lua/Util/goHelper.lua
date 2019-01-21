--  Author： LiangZG
--	Email :  game.liangzg@foxmail.com

-- GameObject 帮助工具集
local m = {}
local this = m

--交换一个组件对象，如果不存在，将添加对应的组件
function m.SwapComponent(gObj , typeCom )
	
	local comIns = gObj:GetComponent(typeCom)
	if not comIns then
		comIns = gObj:AddComponent(typeCom)
	end
	return comIns
end

--设置结点的图层
-- depath 是否遍历
function m.setLayer( root , layerName , depath )
	root.layer = LayerMask.NameToLayer(layerName)

	if not depath then 	return 	end

	local rootTrans = root.transform
	for i=1,rootTrans.childCount do
		this.setLayer(rootTrans:GetChild(i -1).gameObject , layerName , depath)
	end
end

return m
