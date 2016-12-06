
require "test"

test("push should return a table with a , b , c ,size is 3" , function ( )
	local stack = Stack.new()

	stack:push("a")
	stack:push("b")
	stack:push("c")

	assertEqual(stack:size() , 3)
end)


test("pop should return a string" , function ( )
	local stack = Stack.new()

	stack:push("a")
	stack:push("b")
	stack:push("c")

	assertEqual(stack:pop() , "c")
	assertEqual(stack:pop() , "b")
	assertEqual(stack:pop() , "a")
end)


test("pop should return a nil when queue is empty" , function ( )
	local stack = Stack.new()
	assertEqual(stack:pop() , nil)

	stack:push(1)

	assertEqual(stack:pop() , 1)
	assertEqual(stack:pop() , nil)
end)


-- test("return size is zero when push value is nil  " , function ( )
-- 	local stack = Stack.new()
-- 	stack:push()

-- 	assertEqual(stack:size() , 0)
-- end)

-- test("return size is zero when push value is nil  " , function ( )
-- 	local stack = Stack.new()
-- 	print("-----> set value nil----------- ")
-- 	queue["_size"] = nil
-- 	queue["_size"] = 999
-- 	print("-----> set value nil  , size:" .. tostring(stack:size()))
-- end)


test("push over init capactity will work !" , function ( )
	local stack = Stack.new(2)

	stack:push("a")
	stack:push("b")

	assertEqual(stack:pop() , "b")
	assertEqual(stack:pop() , "a")
	assertEqual(stack:pop() , nil)

	stack:push("c")
	stack:push("d")
	stack:push("e")
	stack:push("f")

	assertEqual(stack:pop() , "f")
	assertEqual(stack:pop() , "e")
	assertEqual(stack:pop() , "d")
	assertEqual(stack:pop() , "c")
	assertEqual(stack:pop() , nil)
end)



test("peek dont change queue index !" , function ( )
	local stack = Stack.new()

	stack:push("a")
	stack:push("b")

	assertEqual(stack:peek() , "b")
	assertEqual(stack:pop() , "b")

	assertEqual(stack:peek() , "a")
	assertEqual(stack:pop() , "a")
	assertEqual(stack:pop() , nil)
end)


test('contains should returns correct position of value in the table', function()
	local stack = Stack.new()
	stack:push("a")
	stack:push(1)

    assertEqual(stack:contains('a'), true)
    assertEqual(stack:contains(1), true)
end)

test('contains should returns false when the value is not in the table', function()
	local stack = Stack.new()
	stack:push("a")
	stack:push(1)

    assertEqual(stack:contains("b"), false)
    assertEqual(stack:contains(2), false)

    stack:clear()
    assertEqual(stack:contains(1), false)
end)

test('enumerator should returns all element ', function()
	local stack = Stack.new()
	stack:push("a")
	stack:push(1)
	stack:push("b")
	stack:push("false")

	local str = {}
	for v in stack:enumerator() do
		table.insert(str , v)
	end

	str = table.concat( str, ",")

	assertEqual(str , "false,b,1,a")
end)


test('enumerator should returns all element ', function()
	local stack = Stack.new()
	stack:push("a")
	stack:push(1)
	stack:push("b")
	stack:push("false")


	local str = {}
	local rator = stack:enumerator()
	
	table.insert(str , rator())
	table.insert(str , rator())
	table.insert(str , rator())
	table.insert(str , rator())

	str = table.concat( str, ",")
	print("allValue:" .. str)

	assertEqual(str , "false,b,1,a")
end)


test('getEnumerator should returns all element ', function()
	local stack = Stack.new()
	stack:push("a")
	stack:push(1)
	stack:push("b")
	stack:push("false")


	local str = {}
	local rator = stack:getEnumerator()	

	while rator:moveNext() do
		table.insert(str , rator:current())
	end
	str = table.concat( str, ",")
	print("allValue:" .. str)

	assertEqual(str , "false,b,1,a")
end)
