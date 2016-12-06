
local SingleTaskTest = class("SingleTaskTest")


function  SingleTaskTest:Start () 

	--StartCoroutine(DoSomethingAsynchonously());
	
	--//TaskRunner.Instance.Run(new SingleTask(DoSomethingAsynchonously())); //use this if you are not in a monobehaviour
	TaskRunner.run(SingleTask.new(handler(self , self.DoSomethingAsynchonously)))
end


function SingleTaskTest:TestCoroutine(  )
	print("TestCoroutine")
	
	coroutine.start(handler(self , self.DoSomethingAsynchonously))
	--coroutine.create(self.DoSomethingAsynchonously)
end
	
function SingleTaskTest:DoSomethingAsynchonously()
		
		self.variableThatCouldHaveBeenUseful = false
		
		coroutine.next(handler(self , self.SomethingAsyncHappens))
		--coroutine.resume(curCo)
		
		self.variableThatCouldHaveBeenUseful = true
	
	    print("index is: " .. tostring(self.i))
end

	
function SingleTaskTest:SomethingAsyncHappens()
		
	    for  i = 1 , 3 do
	        self.i = i
	        print("i:" .. i)
	        coroutine.step()
	    end

	    coroutine.next(handler(self , self.SomethingAsyncHappensB))

	    print("SomethingAsyncHappens end")
end

function SingleTaskTest:SomethingAsyncHappensB()
		
	    for  i = 21 , 23 do
	        self.i = i
	        print("i:" .. i)
	        coroutine.step()
	    end

	    print("Happen B end")
end

test = SingleTaskTest.new()
test:Start()
--test:TestCoroutine()

return SingleTask