--[[
    常用模块初始化
]]
local UTIL_MODEL_NAME = ...

eUtil = import(".eUtil")
utils = import(".utils")
goHelper = import(".goHelper")

Scheduler = import (".Scheduler")
TimeService = import(".TimeService")
Timer = import(".Timer")

import(".EventManager")
import(".TaskRunner.init")

