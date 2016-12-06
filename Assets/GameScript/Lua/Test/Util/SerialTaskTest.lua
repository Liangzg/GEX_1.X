
local SerialTasksTest = class("SerialTasksTest") 
	
	
function SerialTasksTest:Start () 
		
    local st = SerialTaskCollection.new()
    local st2 = SerialTaskCollection.new()
	
	st2:add(handler(self , self.Print) , "a")
	st2:add(handler(self ,self.DoSomethingAsynchonously) , 1)
	st2:add(handler(self ,self.Print) , "b")


	st:add(handler(self , self.Print) , 1)
	st:add(handler(self , self.Print) , 2)
	st:add(handler(self , self.DoSomethingAsynchonously) , 1)
	st:add(handler(self , self.Print) , 4)
	st:add(handler(self , self.DoSomethingAsynchonously) , 5)
	st:add(handler(self , self.Print) , 5)
	st:add(self.WWWTest)
	st:add(handler(self , self.Print) , 6)
	st:add(st2)
	st:add(handler(self , self.Print) , 7)


		
	TaskRunner.run(st:getEnumerator())
end
	
function SerialTasksTest:Print(i)
	
		Debugger.Log(i);
		coroutine.step()
	
end	

function SerialTasksTest:DoSomethingAsynchonously(time)
		--print("time:" .. print_lua_table(time))
		coroutine.wait(time);
		
		Debugger.Log("waited " .. time);
	
end	

function SerialTasksTest:WWWTest()
	
		local www = WWW("www.baidu.com");
		
		coroutine.www(www)
		
		Debugger.Log("www done:" .. www.text);
end	


local test = SerialTasksTest.new()
test:Start()

return SerialTasks