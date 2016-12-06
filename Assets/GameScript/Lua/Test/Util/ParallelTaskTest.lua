

local ParallelTaskTest = class("ParallelTaskTest") 

function ParallelTaskTest:Start () 
	
	self.i = 0

	local pt = ParallelTaskCollection.new()
	local	st = SerialTaskCollection.new()
	
	st:add(handler(self , self.Print ) , "s1");
	st:add(handler(self , self.DoSomethingAsynchonously))
	st:add(handler(self , self.Print) , "s3")
	
	pt:add(handler(self , self.Print) ,"1")
	pt:add(handler(self , self.Print) ,"2")
	pt:add(handler(self , self.Print) ,"3")
	pt:add(handler(self , self.Print) ,"4")
	pt:add(handler(self , self.Print) ,"5")
	pt:add(st)
	pt:add(handler(self , self.Print) ,"6")
	pt:add(self.WWWTest)
	pt:add(handler(self , self.Print) ,"7")
	pt:add(handler(self , self.Print) ,"8")
		
	TaskRunner.run(pt:getEnumerator())
end

	
function ParallelTaskTest:Print(i)
	
		Debugger.Log(i);
		coroutine.step()
end
	
function ParallelTaskTest:DoSomethingAsynchonously()  --this can be awfully slow, I suppose it is synched with the frame rate
	
		for i = 0,  500 do
	        self.i = i
	        coroutine.step()
		end

		Debugger.Log("index " .. self.i);
end
	
function ParallelTaskTest:WWWTest()
	
		local www = WWW("www.baidu.com");
		
		coroutine.www(www)
		
		Debugger.Log("www done:" .. www.text);
end


local test = ParallelTaskTest.new()
test:Start()

return ParallelTaskTest