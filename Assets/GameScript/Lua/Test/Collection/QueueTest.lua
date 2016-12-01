
require "test"

test("enqueue should return a table with a , b , c ,size is 3" , function ( )
	local queue = Queue.new()

	queue:enqueue("a")
	queue:enqueue("b")
	queue:enqueue("c")

	assertEqual(queue:size() , 3)
end)


test("dequeue should return a string" , function ( )
	local queue = Queue.new()

	queue:enqueue("a")
	queue:enqueue("b")
	queue:enqueue("c")

	assertEqual(queue:dequeue() , "a")
	assertEqual(queue:dequeue() , "b")
	assertEqual(queue:dequeue() , "c")
end)


test("dequeue should return a nil when queue is empty" , function ( )
	local queue = Queue.new()
	assertEqual(queue:dequeue() , nil)

	queue:enqueue(1)

	assertEqual(queue:dequeue() , 1)
	assertEqual(queue:dequeue() , nil)
end)


-- test("return size is zero when enqueue value is nil  " , function ( )
-- 	local queue = Queue.new()
-- 	queue:enqueue()

-- 	assertEqual(queue:size() , 0)
-- end)

-- test("return size is zero when enqueue value is nil  " , function ( )
-- 	local queue = Queue.new()
-- 	print("-----> set value nil----------- ")
-- 	queue["_size"] = nil
-- 	queue["_size"] = 999
-- 	print("-----> set value nil  , size:" .. tostring(queue:size()))
-- end)


test("enqueue over init capactity will work !" , function ( )
	local queue = Queue.new(2)

	queue:enqueue("a")
	queue:enqueue("b")

	assertEqual(queue:dequeue() , "a")
	assertEqual(queue:dequeue() , "b")
	assertEqual(queue:dequeue() , nil)

	queue:enqueue("c")
	queue:enqueue("d")
	queue:enqueue("e")
	queue:enqueue("f")
	queue:enqueue("g")
	queue:enqueue("h")
	assertEqual(queue:dequeue() , "c")
	assertEqual(queue:dequeue() , "d")
	assertEqual(queue:dequeue() , "e")
	assertEqual(queue:dequeue() , "f")
	assertEqual(queue:dequeue() , "g")	
	assertEqual(queue:dequeue() , "h")	
	assertEqual(queue:dequeue() , nil)
end)



test("peek dont change queue index !" , function ( )
	local queue = Queue.new()

	queue:enqueue("a")
	queue:enqueue("b")

	assertEqual(queue:peek() , "a")
	assertEqual(queue:dequeue() , "a")

	assertEqual(queue:peek() , "b")
	assertEqual(queue:dequeue() , "b")
	assertEqual(queue:dequeue() , nil)
end)


test('contains should returns correct position of value in the table', function()
	local queue = Queue.new()
	queue:enqueue("a")
	queue:enqueue(1)

    assertEqual(queue:contains('a'), true)
    assertEqual(queue:contains(1), true)
end)

test('contains should returns false when the value is not in the table', function()
	local queue = Queue.new()
	queue:enqueue("a")
	queue:enqueue(1)

    assertEqual(queue:contains("b"), false)
    assertEqual(queue:contains(2), false)

    queue:clear()
    assertEqual(queue:contains(1), false)
end)



