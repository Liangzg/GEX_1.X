--[[
	调度结点信息结构体
]]
local SchedulerNode = class("SchedulerNode")

function SchedulerNode:ctor()
	self.delay = 0
	self.interval = 0
	self.listener = nil
	self.once = false
	self.elapseTime = 0
end



local UpdateBeat = UpdateBeat
local ilist = ilist

--[[--

 全局计时器、计划任务

该模块在框架初始化时不会自动载入

]]
local Scheduler = class("Scheduler")

function Scheduler:ctor()
	self.list = linkList:new()
	self.rmList = linkList:new()
	self.lock = false

	self.updateHandler = handler(self , self._update)
	UpdateBeat:Add(self.updateHandler)
end

--[[--

计划一个全局帧事件回调，并返回该计划的句柄。

全局帧事件在任何场景中都会执行，因此可以在整个应用程序范围内实现较为精确的全局计时器。

该函数返回的句柄用作 Scheduler.remove() 的参数，可以取消指定的计划。

@param function 回调函数
@param bool once 是否仅调用一次  

@return mixed scheduleNode句柄

]]
function Scheduler:start(listener , once)
	local node = SchedulerNode.new()
	node.listener = listener
	node.once = once

	self.list:push(node)

    return node
end

--[[--

计划一个以指定时间间隔执行的全局事件回调，并返回该计划的句柄。

@param function listener 回调函数
@param number interval 间隔时间

@return mixed scheduleNode句柄

]]
function Scheduler:startInterval(listener, interval)
	local node = SchedulerNode.new()
	node.listener = listener
	node.interval = interval


	self.list:push(node)

    return node
end

--[[--

取消一个全局计划
@param mixed scheduleNode句柄

]]
function Scheduler:remove(handle)
    for i, v in ilist(self.list) do							
		if v.listener == handle then
			if self.lock then
				self.rmList:push({listener = handle})		
			else
				self.list:remove(i)
			end
		end
	end	
end


function Scheduler:clear( )
	self.list = {}
	self.rmList = {}
end

--[[--

计划一个全局延时回调，并返回该计划的句柄。
会在等待指定时间后执行一次回调函数，然后自动取消该计划。

@param function listener 回调函数
@param number time 延迟时间
@param bool once 是否仅调用一次   

@return mixed scheduleNode句柄

]]
function Scheduler:startDelay(listener, time , once)
	local node = SchedulerNode.new()
	node.listener = listener
	node.delay = time or 0
	node.once = once or false

	self.list:push(node)

    return node
end


function Scheduler:_update( )
	local _list = self.list
	local _rmList = self.rmList
	self.lock = true

	local deltaTime = Time.deltaTime

	for i, n in ilist(_list) do	
		n.elapseTime = n.elapseTime + deltaTime

		if n.elapseTime >= n.interval + n.delay then
			n.elapseTime = 0
			n.delay = 0

			n.listener()

			if n.once then
				_rmList:push(n)
			end
		end
	end

	for _, v in ilist(_rmList) do					
		for i, item in ilist(_list) do							
			if v.listener == item.listener then
				_list:remove(i)
				break
			end 
		end
	end

	_rmList:clear()
	self.lock = false
end


function Scheduler:destroy( )
	UpdateBeat:Remove(self.updateHandler)
	
	self:clear()
end

Scheduler.ins = Scheduler.new()

return Scheduler
