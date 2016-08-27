--[[
Author： LiangZG
Email :  game.liangzg@foxmail.com
]]

require "Common.functions"

--[[
    资源加载测试
]]
LoaderTest = {}
local this = LoaderTest

--入口
function LoaderTest:Start()
   
    AssetLoader.isBundle = false
    
    LuaAssetLoader.OnInitPreload(nil)
end

--主应用释放
function LoaderTest:OnGUI()

    if GUILayout.Button("Clear" , GUILayout.Height(30)) then
         if  this.target ~= nil then
            GameObject.Destroy(this.target)
         end
    end
    
    --直接传递匿名luaFunction测试 ， 结果成功
    if GUILayout.Button("Anon LuaFunction Callback" , GUILayout.Height(30)) then
    
        LuaAssetLoader.LoadGameObject("GO1.prefab" , function(obj)
            this.target = obj
            if obj then
                print("game name is " .. obj.name)
            else
                print("game object is nil ~~~")
            end
        
         end)
    end

    --传递Function 字符名称 , 结果成功
    if GUILayout.Button("Function String Callback" , GUILayout.Height(30)) then
        LuaAssetLoader.LoadGameObject("GO1.prefab" , "LoaderTest.LoadFinish")
    end

    --传递luaFunction测试 ， 结果成功
    if GUILayout.Button("Function Callback" , GUILayout.Height(30)) then
        LuaAssetLoader.LoadGameObject("GO1.prefab" , LoaderTest.LoadFinish)
    end    
end


function LoaderTest.LoadFinish(obj)
    
    this.target = obj
            if obj then
                print("game name is " .. obj.name)
            else
                print("game object is nil ~~~")
            end

end