
--[[--
计算表格包含的字段数量
Lua table 的 "#" 操作只对依次排序的数值下标数组有效，table.nums() 则计算 table 中所有不为 nil 的值的个数。
@param table t 要检查的表格
@return integer
]]
function table.nums(t)
	local count = 0
	for k, v in pairs(t) do
		count = count + 1
	end
	return count
end

--[[--
返回指定表格中的所有键
~~~ lua
local hashtable = {a = 1, b = 2, c = 3}
local keys = table.keys(hashtable)
-- keys = {"a", "b", "c"}
~~~
@param table hashtable 要检查的表格
@return table
]]
function table.keys(hashtable)
	local keys = {}
	for k, v in pairs(hashtable) do
		keys[#keys + 1] = k
	end
	return keys
end

--[[--
返回指定表格中的所有值
~~~ lua
local hashtable = {a = 1, b = 2, c = 3}
local values = table.values(hashtable)
-- values = {1, 2, 3}
~~~
@param table hashtable 要检查的表格
@return table
]]
function table.values(hashtable)
	local values = {}
	for k, v in pairs(hashtable) do
		values[#values + 1] = v
	end
	return values
end

--[[--
将来源表格中所有键及其值复制到目标表格对象中，如果存在同名键，则覆盖其值
~~~ lua
local dest = {a = 1, b = 2}
local src  = {c = 3, d = 4}
table.merge(dest, src)
-- dest = {a = 1, b = 2, c = 3, d = 4}
~~~
@param table dest 目标表格
@param table src 来源表格
]]
function table.merge(dest, src)
	for k, v in pairs(src) do
		dest[k] = v
	end
end

--[[--
在目标表格的指定位置插入来源表格，如果没有指定位置则连接两个表格
~~~ lua
local dest = {1, 2, 3}
local src  = {4, 5, 6}
table.insertto(dest, src)
-- dest = {1, 2, 3, 4, 5, 6}
dest = {1, 2, 3}
table.insertto(dest, src, 5)
-- dest = {1, 2, 3, nil, 4, 5, 6}
~~~
@param table dest 目标表格
@param table src 来源表格
@param [integer begin] 插入位置
]]
function table.insertto(dest, src, begin)
	begin = checkint(begin)
	if begin <= 0 then
		begin = #dest + 1
	end

	local len = #src
	for i = 0, len - 1 do
		dest[i + begin] = src[i + 1]
	end
end

--[[
从表格中查找指定值，返回其索引，如果没找到返回 false
~~~ lua
local array = {"a", "b", "c"}
print(table.indexof(array, "b")) -- 输出 2
~~~
@param table array 表格
@param mixed value 要查找的值
@param [integer begin] 起始索引值
@return integer
]]
function table.indexof(array, value, begin)
	for i = begin or 1, #array do
		if array[i] == value then return i end
	end
	return false
end

--[[--
从表格中查找指定值，返回其 key，如果没找到返回 nil
~~~ lua
local hashtable = {name = "dualface", comp = "chukong"}
print(table.keyof(hashtable, "chukong")) -- 输出 comp
~~~
@param table hashtable 表格
@param mixed value 要查找的值
@return string 该值对应的 key
]]
function table.keyof(hashtable, value)
	for k, v in pairs(hashtable) do
		if v == value then return k end
	end
	return nil
end

--[[--
从表格中删除指定值，返回删除的值的个数
~~~ lua
local array = {"a", "b", "c", "c"}
print(table.removebyvalue(array, "c", true)) -- 输出 2
~~~
@param table array 表格
@param mixed value 要删除的值
@param [boolean removeall] 是否删除所有相同的值
@return integer
]]
function table.removebyvalue(array, value, removeall)
	local c, i, max = 0, 1, #array
	while i <= max do
		if array[i] == value then
			table.remove(array, i)
			c = c + 1
			i = i - 1
			max = max - 1
			if not removeall then break end
		end
		i = i + 1
	end
	return c
end

--[[--
对表格中每一个值执行一次指定的函数，并用函数返回值更新表格内容
~~~ lua
local t = {name = "dualface", comp = "chukong"}
table.map(t, function(v, k)
    -- 在每一个值前后添加括号
    return "[" .. v .. "]"
end)
-- 输出修改后的表格内容
for k, v in pairs(t) do
    print(k, v)
end
-- 输出
-- name [dualface]
-- comp [chukong]
~~~
fn 参数指定的函数具有两个参数，并且返回一个值。原型如下：
~~~ lua
function map_function(value, key)
    return value
end
~~~
@param table t 表格
@param function fn 函数
]]
function table.map(t, fn)
	for k, v in pairs(t) do
		t[k] = fn(v, k)
	end
end


--[[--
将参数里面的keylist合成一个deep map
~~~比如传入 'a','b','c','d'则会return 一个map {a={b={c='d'}}}
]]
function table.keymap(map, ...)
	map = map or {}
	if arg['n'] <= 1 then
		return map
	end

	local tmp = map
	for k,v in ipairs(arg) do
		if k >= arg['n'] - 1 then
			tmp[v] = arg[k+1]
			break
		end

		tmp[v] = tmp[v] or {}
		tmp = tmp[v]
	end
	return map
end

--[[--
对表格中每一个值执行一次指定的函数，但不改变表格内容
~~~ lua
local t = {name = "dualface", comp = "chukong"}
table.walk(t, function(v, k)
    -- 输出每一个值
    print(v)
end)
~~~
fn 参数指定的函数具有两个参数，没有返回值。原型如下：
~~~ lua
function map_function(value, key)
end
~~~
@param table t 表格
@param function fn 函数
]]
function table.walk(t, fn)
	for k,v in pairs(t) do
		fn(v, k)
	end
end

--[[--
对表格中每一个值执行一次指定的函数，如果该函数返回 false，则对应的值会从表格中删除
~~~ lua
local t = {name = "dualface", comp = "chukong"}
table.filter(t, function(v, k)
    return v ~= "dualface" -- 当值等于 dualface 时过滤掉该值
end)
-- 输出修改后的表格内容
for k, v in pairs(t) do
    print(k, v)
end
-- 输出
-- comp chukong
~~~
fn 参数指定的函数具有两个参数，并且返回一个 boolean 值。原型如下：
~~~ lua
function map_function(value, key)
    return true or false
end
~~~
@param table t 表格
@param function fn 函数
]]
function table.filter(t, fn)
	for k, v in pairs(t) do
		if not fn(v, k) then t[k] = nil end
	end
end

--[[--
遍历表格，确保其中的值唯一
~~~ lua
local t = {"a", "a", "b", "c"} -- 重复的 a 会被过滤掉
local n = table.unique(t)
for k, v in pairs(n) do
    print(v)
end
-- 输出
-- a
-- b
-- c
~~~
@param table t 表格
@return table 包含所有唯一值的新表格
]]
function table.unique(t)
	local check = {}
	local n = {}
	for k, v in pairs(t) do
		if not check[v] then
			n[k] = v
			check[v] = true
		end
	end
	return n
end

--[[--
指定表是否为空
@param table t 表格
@return 不为空返回true , 否则false
]]
function table.empty(t)
--  for k,v in pairs(tbl) do
--      return false
--  end
--  return true
	return next(t)==nil
end

--[[--
清空表数据
@param table t 表格
]]
function table.clear(t)
	if not t then return end
	for k,v in pairs(t) do
		t[k] = nil
	end
end

--[[--
两个表是否相同
@param table t1 表格
@param table t2 表格
@return 相同返回true , 否则false
]]
function table.equal(t1, t2)
	if not t1 or not t2 then
		return false
	end

	if t1 == t2 then
		return true
	end

	if table.nums(t1) ~= table.nums(t2) then
		return false
	end

	for k,v in pairs(t1) do
		if t2[k] ~= v then
			return false
		end
	end
	return true
end

--[[--
深度拷贝表
@param table src  源表
]]
function table.deepcopy(src)
	if type(src) ~= "table" then
		return src
	end
	local cache = {}
	local function clone_table(t, level)
		if not level then
			level = 0
		end

		if level > 100 then
			return t
		end

		local k, v
		local rel = {}
		for k, v in pairs(t) do
			if type(v) == "table" then
				if cache[v] then
					rel[k] = cache[v]
				else
					rel[k] = clone_table(v, level+1)
					cache[v] = rel[k]
				end
			else
				rel[k] = v
			end
		end
		setmetatable(rel, getmetatable(t))
		return rel
	end
	return clone_table(src)
end

--[[--
目标表中指定Key是否有值
@param table t 目标表
@param key 键值
@return 如果有值,返回对应的value,否则返回nil,
]]
function table.hasvalue(t, key)
	for k,v in pairs(t) do
		if v == key then
			return k
		end
	end
	return nil
end

--[[--
返回Array中的最大值
--注意:不是Hash-table
@param table arr 有序表(array)
]]
function table.max(arr)
	return math.max (unpack(arr))
end

--[[--
--返回Array中的最小值
--注意:不是Hash-table
@param table arr 有序表(array)
]]
function table.min(Array)
	return math.min (unpack(Array))
end

--[[--
从table中随机返回n个k,v的table
]]
function table.randoms(t)
	local Keys = table.keys(t)
	local n = #Keys
	local ret = {}
	for i=1, n do
		local R = math.random(1, #Keys)
		local key = Keys[R]
		local value = t[key]
		ret[key]=value
		table.remove(Keys, R)
	end
	return ret
end

--[[--
根据表中的某个key的值进行排序

@param table tbl 需要排序的表
@param Key key 指定键
@return 返回{k=key,v=data}的数组
]]
function table.sortbykey(tbl,key)
	local Keys = table.keys(tbl)
	local Size = #Keys
	for i=1, Size do
		for j=i, Size do
			if tbl[Keys[i]][key] and tbl[Keys[j]][key] then
				if tbl[Keys[i]][key] > tbl[Keys[j]][key] then
					--交换
					local Tmp = Keys[i]
					Keys[i] = Keys[j]
					Keys[j] = Tmp
				end
			end
		end
	end
	local ret = {}
	for i,key in ipairs(Keys) do
		table.insert(ret,{k=key,v=tbl[key]})
	end
	return ret
end


--[[--
根据表中的值进行排序

@param table tbl 需要排序的表
@return 排序完成的keys映射表
]]
function table.sortbyvalue(tbl)
	--传说中的选择排序
	local Keys = table.keys(tbl)
	local Size = #Keys
	for i=1, Size do
		for j=i, Size do
			if tbl[Keys[i]] > tbl[Keys[j]] then
				--交换
				local Tmp = Keys[i]
				Keys[i] = Keys[j]
				Keys[j] = Tmp
			end
		end
	end
	return Keys
end

--[[--
自定义排序

@param table arr 需要排序的有序(array)表
@param function func 排序方法
]]
function table.insertsort(arr, func)
	if #arr<=1 then return end
	for i = 2,#arr do
		local temp = arr[i]
		local j = i-1
		while j>=1 do
			local _is =false
			if func then
				_is =func(temp, arr[j])
			else
				_is = temp > arr[j]
			end

			if _is then
				arr[j+1] = arr[j]
				j = j-1
			else
				break
			end
		end
		arr[j+1] = temp
	end
end

--[[--
反转一个有序array
@param table Array 有序表
]]
function table.reverse(Array)
	local size = #Array
	for i=1, math.floor(size/2) do
		local tmp = Array[i]
		Array[i] = Array[size+1-i]
		Array[size+1-i] = tmp
	end
end


--Returns a new tbl that is n copies of the tbl.
function table.rep(Tbl, n)
	if n < 1 then
		return nil
	end
	local Tmp = {}
	for k=1, n do
		table.insert(Tmp, Tbl)
	end
end

--[[--
判断包含关系(array only)
]]
function table.contain(Big, Small)
	for _, Each in pairs(Small) do
		if not table.member_key(Big, Each) then
			return false, Each
		end
	end

	return true
end

--[[--
将array1与array2里面对应位置合成mapping的一个pair
]]
function table.mapping(array1, array2)
	local ret = {}
	for k,v in ipairs(array1) do
		ret[v] = array2[k]
	end
	return ret
end

--// table.binsert( table, value [, comp] )

-- LUA 5.x ADD-On for the table library
-- Inserts a given value through BinaryInsert into the table sorted by [,comp]
-- If comp is given, then it must be a function that receives two table elements,
-- and returns true when the first is less than the second or reverse
-- e.g.  comp = function( a, b ) return a > b end , will give a sorted table, with the biggest value on position 1
-- [, comp] behaves as in table.sort( table, value [, comp] )

-- This method is faster than a regular table.insert( table, value ) and a table.sort( table [, comp] )

function table.binsert( t, value, fcomp )

	-- Initialise Compare function
	fcomp = fcomp or function( a, b ) return a < b end

	--  Initialise Numbers
	local iStart, iEnd, iMid, iState =  1, table.getn( t ), 1, 0

	-- Get Insertposition
	while iStart <= iEnd do

		-- calculate middle
		iMid = math.floor( ( iStart + iEnd )/2 )

		-- compare
		if fcomp( value , t[iMid] ) then
			iEnd = iMid - 1
			iState = 0
		else
			iStart = iMid + 1
			iState = 1
		end
	end

	table.insert( t, ( iMid+iState ), value )
end

--// table.bfind( table, value [, compvalue] [, reverse] )

-- LUA 5.x ADD-On for the table library
-- Searches the table through BinarySearch for value, if the value is found it returns the index
--     and the value of the table where it was found
-- If compvalue is given then it must be a function that takes one value and returns a second value2
-- to be compared with the input value, e.g. compvalue = function( value ) return value[1] end
-- If reverse is given then the search assumes that the table is sorted with the biggest value on position 1

function table.bfind( t, value, fcompval, reverse )

	-- initialise Functions
	fcompval = fcompval or function( value ) return value end
	fcomp = function( a, b ) return a < b end
	if reverse then
		fcomp = function( a, b ) return a > b end
	end

	--  Initialise Numbers
	local iStart, iEnd, iMid =  1, table.getn( t ), 1

	-- Binary Search
	while (iStart <= iEnd) do

		-- calculate middle
		iMid = math.floor( ( iStart + iEnd )/2 )

		-- get compare value
		local value2 = fcompval( t[iMid] )

		if value == value2 then
			return iMid, t[iMid]
		end

		if fcomp( value , value2 ) then
			iEnd = iMid - 1
		else
			iStart = iMid + 1
		end
	end
end

--传入一个类似这样的table：{[1] = {Odds = 50}, [2] = {Odds = 50}}
--根据随机取到key返回
function table.get_key_by_odds(Tbl, FullOdds)
	if Tbl == nil then
		return nil
	end
	--兼容某些传入的是小数的情况
	local Ext=100
	if not FullOdds then
		FullOdds = 0
		for k,v in pairs(Tbl) do
			FullOdds = FullOdds + v.Odds*Ext
		end
	else
		FullOdds = FullOdds*Ext
	end

	--注意，nil和无参数是不一样的
	local Ran = math.random(FullOdds)

	local TotalRan = 0
	for key, subTbl in pairs(Tbl) do
		TotalRan = TotalRan + subTbl.Odds*Ext
		if Ran <= TotalRan then
			return key
		end
	end
end