--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : 有序Array容器 ， 列表的下标从1开始
]]

local rawget = rawget

local Array = {}

Array.__index = function ( t , k )
	local var = rawget(Array, k)
	if var == nil then
		var = t._array_[k]
	end
	return var
end

Array.__newindex = function ( t , k ,v )			
	if nil == t._array_[k] then
		error(string.format("%s index out of range ." , k))
		return 
	end

	if nil == v then
		print("cant not remove element by use 'nil' . ")
		return 
	end

	rawset(t._array_ ,k, v)
end


-- Array.__call = function ( t , ... )
-- 	return Array.new( unpack{...})
-- end	

function Array.new( ... )	
	local newArray = { _array_ = {...} }
	setmetatable(newArray , Array)
	return newArray
end


function Array:removeAt( index )
	table.removeAt(self._array_ , index)
end


function Array:insert( index , item )
	index = Mathf.Min(index , #self._array_ + 1)
	table.insert(self._array_ , index , item)
end


function Array:length()
	return #self._array_
end

--对象是否是数组
function Array.isArray(obj)
	if type(obj) ~= "table" then	return false	end

	local i = 0
	for _ in pairs(obj) do
	 	i = i + 1
	 	if obj[i] == nil then	return false end
	end 
	return true
end

--是否为空，true为空
function Array.isEmpty( obj )
	return Array.isArray(obj) and obj:length() == 0
end

--切割数组,返回一个新的数组句柄，不影响原数组
-- @ [number] startIndex > 0 下标从1开始
function Array:slice( array , startIndex , endIndex )
	if Array.isEmpty(array) then	return nil end

	local newArr = Array.new()
	endIndex = endIndex or array:length()
	for i=startIndex,endIndex do
		newArr:insert(newArr:length() , array[i])
	end
	return newArr
end

--添加多个元素
function Array:append( ... )
	local elements = {...}
	for i=1,#elements do
		table.insert(self._array_ , #self._array_)
	end
end

function Array:indexOf( value )
	for i,v in ipairs(self._array_) do
		if v == value then	return i end
	end
	return -1
end


--反转
function Array:reverse()
	local newArr = Array.new()
	for i=self:length() , 1 , -1 do
		newArr:insert(newArr:length() , self._array_[i])
	end
	return newArr
end


function Array:first()
	return self._array_[1]
end

function Array:last( )
	return self._array_[#self._array_]
end

--删除第一个元素，并返回删除元素
function Array:shift( )
	local v = self._array_[1]
	self:removeAt(1)
	return v
end

--在第一个位置处插入指定元素，并返回当前数组大小
function Array:unshift( v )
	self:insert(1 , v)
	return self:length()
end


function Array.copy( srcArray , srcIndex , destArray , destIndex , length )
	local j = destIndex
	for i=srcIndex,srcIndex + length do		
		destArray:insert(j , srcArray[i])
		j = j + 1
	end
end

--将当前一维 Array 的所有元素复制到指定的一维 Array 中（从指定的目标 Array 索引开始)
function Array:copyTo( array , startIndex )
	for i=1,#self._array do
		array:insert(startIndex + i - 1, self._array_[i])
	end
end


function Array:toString( )	
	local str = {}
	for k,v in ipairs(self._array_) do
		table.insert(str , tostring(v))
	end
	return table.concat( str, ", ")
end





return Array