--[[
    此脚本用来导入游戏具体功能模块生成的Wrap映射对象
]]
ByteArray = NetCore.ByteArray
ByteArrayQueue = NetCore.ByteArrayQueue

--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;

resMgr = LuaHelper.GetResManager ();
panelMgr = LuaHelper.GetPanelManager ();
soundMgr = LuaHelper.GetSoundManager ();
networkMgr = LuaHelper.GetNetManager ();

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;