Localization = {}



--初始化
function Localization:InitLocalization()
    self.Keys = {}
    self.files = {}
    self.language = AppConst.Language.CHINESE

    self:SetLanguage(self.language)
    self:RegisterModule()
    self:LoadData()
end

--注册功能模块配置表
function Localization:RegisterModule()
    local function register_module(file)
        local path = "Localization/"..self.language.."/"..file
        table.insert(self.files,path)
    end

    --注册不同模块的配置表
    for key, var in ipairs(AppConst.LocalizationModule) do
        register_module(var)
    end

end

--设置语言
function Localization:SetLanguage(languageType)
    if self.language ~= languageType then 
        self.language = languageType 
        self:ClearKeys()
        self:RegisterModule()
        self:LoadData()
    end
end

--加载配置表
function Localization:LoadData()
    for key, var in pairs(self.files) do
        local flie = require(var)
        for key, var in pairs(flie) do
            self.Keys[key] = var
        end
    end
end

--根据key获取value
function Localization:GetText(key)
    local value = self.Keys[key]
    assert(value,"not found value ,key = "..key)
    return self.Keys[key]
end

--清除数据
function Localization:ClearKeys()
    self.Keys = {}
end

--根据中的key获取对应的value，然后替换文本
function Localization:ReplaceByKey(key,count,...)
    local str = self:GetText(key)
    print(str)
    return self:ReplaceText(str,count,...)
end

--替换字符串
function Localization:ReplaceText(str,count,...)
    local args = {...}
    print(...)
    for key, var in ipairs(args) do
        if key > count then
            break
        end
        local match = "$"..key
        str = string.gsub(str,match,tostring(var))
    end
    
    return str
end