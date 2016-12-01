--[[
	 Author： LiangZG
	Email :  game.liangzg@foxmail.com
]]

require "Common.functions"

--[[
	Desc:
]]
APITest = {}
local this = APITest


function this.Awake()
	-- body
end



function this.Start( )
	-- body

	TimeService.ins:adjustServerTime(os.time())
end


function this.OnGUI(  )
	if GUILayout.Button("StringBuffer" , GUILayout.Height(30)) then
		this.testStringBuffer()
	end
	if GUILayout.Button("testList" , GUILayout.Height(30)) then
		this.testList()
	end	
	
	--this.schedulerAPI()

	--this.timerGUI()

	this.arrayGUI()
end


function this.OnDestroy( ... )
	-- body
end


function this.testStringBuffer()
	
	local stringBuffer = require "Text.stringBuffer"

	local sb = stringBuffer.new()
	sb:append("test")
	sb:append(" , next ")
	sb:appendFormat("%d_%d" , 10 , 1000)
	sb:appendLine("appendLine")
	sb:appendLine("line2")
	print(sb:toString())

	print("remove--->" .. sb:size())
	sb:remove("test")
	print("size:" .. sb:size() .. " ,stringBuffer: " .. sb:toString())

	print("clear------>")
	sb:clear()
	print("size:" .. sb:size() .. " ,stringBuffer: " .. sb:toString())
end


function this.testList()
	local list = require "Collections.list"
	print("list:" .. tostring(list))

	local mList = list.new()
	mList:add(1)
	mList:add(2)
	mList:add(3)

	print("size:" .. mList:size() .. " , allValue:" .. mList:toString())

	for item in mList:values() do
		print(item)
	end

	print("next for :")
	for item in mList:values() do
		print(item)
	end

	print(" 1：" .. mList:get(1))
	print(" 2：" .. mList:get(2))
	print(" 3：" .. mList:get(3))

	print("remove index:2")
	mList:removeAt(2)	
	print("size:" .. mList:size() .. " , allValue:" .. mList:toString())
	
end


function this.schedulerAPI( )
	if GUILayout.Button(" add globalInterval" , GUILayout.Height(30)) then

		local index = 0
		this.updateFunc = function ( ... )
			print("log:" .. index)

			index = index + 1
		end
		
		Scheduler.ins:startInterval(this.updateFunc , 0.5)
	end		

	if GUILayout.Button("remove globalInterval" , GUILayout.Height(30)) then
		Scheduler.ins:remove(this.updateFunc)
	end	

	if GUILayout.Button(" start once" , GUILayout.Height(30)) then
		Scheduler.ins:start(function (  )
			-- body
			print(" do once ")
		end , true)
	end

	if GUILayout.Button(" start delay" , GUILayout.Height(30)) then
		print(" start delay ")
		Scheduler.ins:startDelay(function (  )
			-- body
			print(" delay func")

		end , 1 , true)
	end
end



function this.timerGUI( )
	if GUILayout.Button("start time second" , GUILayout.Height(30)) then
		local timer = Timer.new()
		local curTime = TimeService.ins:totalMillisecondNow() / 1000
		timer:setBeginTime(curTime , 10 , eTimeUnit.Second)
		local index = 0
		timer.changeFunc = function ( ... )
			index = index + 1

			print("time:" .. index)
		end
		timer:start()
	end

	if GUILayout.Button("start time milliSecond" , GUILayout.Height(30)) then
		local timer = Timer.new()
		local curTime = TimeService.ins:totalMillisecondNow() / 1000
		timer:setBeginTime(curTime , 1 , eTimeUnit.MilliSecond)
		local index = 0
		timer.changeFunc = function ( ... )
			index = index + 1

			print("time:" .. index)
		end
		timer:start()
	end

	if GUILayout.Button("start time custom" , GUILayout.Height(30)) then
		local timer = Timer.new()
		local curTime = TimeService.ins:totalMillisecondNow() / 1000
		timer:setBeginTime(curTime , 10 , 1.2)
		local index = 0
		timer.changeFunc = function ( ... )
			index = index + 1

			print("custom time:" .. index)
		end
		timer:start()
	end	
end



function this.arrayGUI(  )
	if GUILayout.Button(" Test Array " , GUILayout.Height(30)) then
		require "Collection.ArrayTest"
	end	

	if GUILayout.Button(" Test Queue " , GUILayout.Height(30)) then
		require "Collection.QueueTest"
	end	
end