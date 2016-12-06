

local AsyncTask = class("AsyncTask")

function AsyncTask:ctor( task )
	self._task = task
end

function AsyncTask:current( )
	return self
end


function AsyncTask:moveNext()
	return self:execute()
end

function AsyncTask:execute()
	
	self._task:execute()

	while self._task.isDone == false do
		coroutine.step()
	end		
end


function AsyncTask:reset( )
	-- body
end


function AsyncTask:toString()
	return self._task:toString()
end

return AsyncTask