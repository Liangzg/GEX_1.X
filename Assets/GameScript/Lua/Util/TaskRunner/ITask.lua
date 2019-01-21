

IATask = {}
IATask.isDone = false
IATask.process = 0


function IATask:onComplete( action )	end


ITask = {}
implement(ITask , IATask)

function ITask:execute()	end


--print(print_lua_table(ITask))