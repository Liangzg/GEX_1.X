--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

UIPageTest = {}
local this = UIPageTest

--Update挂点， UpdateBeat:Add(luaFunc)
local UpdateBeat = UpdateBeat

function UIPageTest.Awake()    

    AssetLoader.isBundle = false

end


function UIPageTest.Start()    

    LuaAssetLoader.OnInitPreload(function()
    
        print("Load Finish ---->")
        
    end)

end



function UIPageTest.OnGUI()
    
    if this.Tag then
        return 
    end

   if GUILayout.Button("Show" , GUILayout.Height(30)) then
           UIManager:Show("TopToolbar" , nil)
           UIManager:Show("MainUI" , nil)
           this.Tag = true
    end

    if GUILayout.Button("Show TopToolbar" , GUILayout.Height(30)) then
           UIManager:Show("Test/UI/TopToolbar" , nil)
           this.Tag = true
    end

    if GUILayout.Button("Show MainUI" , GUILayout.Height(30)) then
           UIManager:Show("Test/UI/MainUI" , nil)
           this.Tag = true
    end
end


function UIPageTest.OnDestroy()

end

