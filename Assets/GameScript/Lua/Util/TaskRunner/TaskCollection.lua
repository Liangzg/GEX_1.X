

local TaskCollection = class("TaskCollection")


function TaskCollection:ctor( )
	self._taskQueue = Queue.new()
	self._args = {}
end


function TaskCollection:isRunning( )
	return self._isRunning or false
end


function TaskCollection:process()
	return 0
end

function TaskCollection:addAsyncTask( task , ...)
	
	self:add(AsyncTask.new(task) , unpack({...}))
end

function TaskCollection:add( asyncTask , ... )
	self._args[asyncTask] = {...}
	self._taskQueue:enqueue(asyncTask)
end

--@abstract 
function TaskCollection:getEnumerator( )
	return nil
end

return TaskCollection