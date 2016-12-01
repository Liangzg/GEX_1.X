
require "test"


test('is_array should returns false when object is not a table', function()
    assertEqual(Array.isArray('lua'), false)
end)

test('is_array should returns false when the table is working like a dictionary', function()
    assertEqual(Array.isArray({ language='lua' }), false)
end)

test('is_array should returns true when table is empty', function()
    assertEqual(Array.isArray(Array.new({})), true)
end)

test('is_array should returns true when table is empty', function()
    assertEqual(Array.isArray({}), true)
end)

test('is_array should returns true when table is working like an array', function()
    assertEqual(Array.isArray({ 'a', 'b', 'c', 'd' }), true)
end)

test('isEmpty should returns false when table has at least one item', function()

    assertEqual(Array.isEmpty(Array.new({ 'a' })), false)
end)

test('isEmpty should returns false when table does not have any item', function()
	local array = Array.new({})
	-- print("array:" .. array:length() .. ":" .. print_lua_table(array))
    assertEqual(Array.isEmpty(array), true)
end)

test('first should returns nil when table is working like a dictionary', function()
	local array = Array.new({language='lua'})
    assertEqual(array:first(), nil)
end)

test('first should returns first item from table', function()
	local array = Array.new({ 'a', 'b', 'c', 'd' })
    assertEqual(array:first(), 'a')
end)

test('last should returns nil when table is working like a dictionary', function()
	local array = Array.new({language='lua'})
    assertEqual(array:last(), nil)
end)

test('last should returns last item from table', function()
	local array = Array.new({ 'a', 'b', 'c', 'd' })
    assertEqual(array:last(), 'd')
end)

test('slice should returns a empty table when it does not have any element', function()
	local array = Array.new({})
	local sliceArr = Array.slice(array , 1, 2)
    assertEqual(sliceArr, nil)
end)

test('slice should returns a table with values between start index and end index', function()
    local array = Array.new({ 'lua', 'javascript', 'python', 'ruby', 'c' })
	local result = Array.slice(array , 2, 4)
    assertEqual(type(result), 'table')
    assertEqual(result:length(), 3)
    assertEqual(result[1], 'javascript')
    assertEqual(result[2], 'python')
    assertEqual(result[3], 'ruby')
end)

test('slice should returns a table with every values from start index until last index', function()
    local array = Array.new({ 'lua', 'javascript', 'python', 'ruby', 'c' })
	local result = Array.slice(array , 2)

    assertEqual(type(result), 'table')
    assertEqual(result:length(), 4)
    assertEqual(result[1], 'javascript')
    assertEqual(result[2], 'python')
    assertEqual(result[3], 'ruby')
    assertEqual(result[4], 'c')
end)

test('reverse should returns an inverted table', function()
	local array = Array.new({ 'lua', 'javascript', 'python'})
    local result = array:reverse()

    assertEqual(type(result), 'table')
    assertEqual(result:length(), 3)
    assertEqual(result[1], 'python')
    assertEqual(result[2], 'javascript')
    assertEqual(result[3], 'lua')
end)

-- test('map should returns a table with 2, 4, 6 values', function()
--     local result = array:map({ 1, 2, 3 }, function(value)
--         return value * 2
--     end)

--     assertEqual(type(result), 'table')
--     assertEqual(#result, 3)
--     assertEqual(result[1], 2)
--     assertEqual(result[2], 4)
--     assertEqual(result[3], 6)
-- end)

-- test('filter should returns a table with 10, 15, 20 values', function()
--     local result = array:filter({ 15, 10, 5, 3, 20 }, function(value)
--         return value >= 10
--     end)

--     assertEqual(type(result), 'table')
--     assertEqual(#result, 3)
--     assertEqual(result[1], 15)
--     assertEqual(result[2], 10)
--     assertEqual(result[3], 20)
-- end)

-- test('max should returns the biggest value from a table', function()
--     assertEqual(array:max({ 20, 22, 1, 3, 30, 42 }), 42)
-- end)

-- test('max should returns nil when table is empty', function()
--     assertEqual(array:max({}), nil)
-- end)

-- test('min should returns the smallest value from a table', function()
--     assertEqual(array:min({ 20, 22, 1, 3, 30, 42 }), 1)
-- end)

-- test('min should returns nil when table is empty', function()
--     assertEqual(array:min({}), nil)
-- end)

-- test('reduce should returns 90', function()
--     local result = array:reduce({ 20, 30, 40 }, function(memo, value)
--         return memo + value
--     end)

--     assertEqual(result, 90)
-- end)

-- test('reduce should returns 100', function()
--     local result = array:reduce({ 20, 30, 40 }, function(memo, value)
--         return memo + value
--     end, 10)

--     assertEqual(result, 100)
-- end)

test('indexOf should returns correct position of value in the table', function()
	local array = Array.new({ 20, 30, 40, 50 })
    assertEqual(array:indexOf(40), 3)
end)

test('indexOf should returns -1 when the value is not in the table', function()
	local array = Array.new({ 20, 30, 40})
    assertEqual(array:indexOf(50), -1)
end)

test("copy source array to dest array !" , function ()
    local a = Array.new({1,2,3})
    local b = Array.new("a" , 'b' , 'c')

    Array.copy(a , 1 , b , b:length()  + 1, a:length())

    assertEqual(b:length() , 6)
    assertEqual(b[4] , 1)
    assertEqual(b[5] , 2)
    assertEqual(b[6] , 3)

end)

test("copyTo should returns contain source array elements !" , function ()
    local a = Array.new({1,2,3})
    local b = Array.new("a" , 'b' , 'c')

    a:copyTo(b , 1)

    assertEqual(b:length() , 6)
    assertEqual(b[4] , 1)
    assertEqual(b[5] , 2)
    assertEqual(b[6] , 3)

end)


test("append should returns contain all array elements !" , function ()
    local a = {1,2,3}
    local b = Array.new({"a" , 'b' , 'c'})

    b:append(a)
    assertEqual(b:length() , 6)
    assertEqual(b[4] , 1)
    assertEqual(b[5] , 2)
    assertEqual(b[6] , 3)

    b:append('e' , 'f' , 7 , 8)

    assertEqual(b:length() , 10)
    assertEqual(b[7] , 'e')
    assertEqual(b[8] , 'f')
    assertEqual(b[9] , 7)
    assertEqual(b[10] , 8)
end)