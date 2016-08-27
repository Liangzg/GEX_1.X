require "AppConst"
require "Localization/Localization"
LocalizationTest = {}

function LocalizationTest:OnStart()

   Localization:InitLocalization()

end

function LocalizationTest:OnGUI()

    local function printText(key ,value)
        print("key= ".. key .."   value = " .. value)
    end

    if GUILayout.Button("GetValueByKey" , GUILayout.Height(30)) then
        local text = Localization:GetText("LTCommon_AppName")
        printText("LTCommon_AppName",text)

        text = Localization:GetText("LTCommon_HeHe")
        printText("LTCommon_HeHe",text)

        text = Localization:GetText("LTUser_HaHa")
        printText("LTUser_HaHa",text)

        text = Localization:ReplaceByKey("LTCommon_DongWang",2,text,text)
        printText("LTCommon_DongWang",text)
    end

end