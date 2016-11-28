--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : 有序Array容器 ， 列表的下标从1开始
]]

local array=class("array")

array.NUMERIC=16
array.length=0

function array:ctor(...)
	local args={...}
	self.length=_G.table.maxn(args)
	for i=1,self.length do
		self[i-1]=args[i]
	end
	if typeof(self[0])=="string" and _G.string.find(self[0],"#=")==1 then
		self.length=_G.tonumber(_G.string.sub(self[0],3))
		self[0]=nil
	end
end


function array:splice(...)
	local arr2=array.new()
	local args={...}
	local n1=args[1] or self.length
	if n1<0 then n1=self.length+n1 end
	n1=Math.max(n1,0)+1
	local n2=args[2] or self.length
	_G.table.remove(args,1)
	_G.table.remove(args,1)
	_G.table.insert(self,1,self[0])
	for i=1,n2 do
		arr2:push(self[n1])
		_G.table.remove(self,n1)
	end
	for i=#args,1,-1 do
		_G.table.insert(self,n1,args[i])
	end
	self.length=#self
	self[0]=self[1]
	_G.table.remove(self,1)
	return arr2
end

function array:push(...)
	self:splice(self.length,0,...)
	return self.length
end

function array:shift()
	local v=self[0]
	self:splice(0,1)
	return v
end

function array:unshift(...)
	self:splice(0,0,...)
	return self.length
end

function array:pop()
	local v=self[self.length-1]
	self:splice(self.length-1,1)
	return v
end

function array:slice(b,e)
	local arr=array.new()

	if b==nil then b=0 end
	if e==nil then e=self.length end
	
	if b<0 then b=Math.max(self.length+b,0) end
	if e<0 then e=Math.max(self.length+e,0) end
	local count=0
	for i=b,e-1 do
		if self[i]==nil then break end
		arr[i-b]=self[i]
		count=count+1
	end
	arr.length=count;
	return arr
end

function array:join(s)
	local str=""
	for i=1,self.length do
		str=str.._G.tostring(self[i-1])
		if i<self.length then str=str..s end
	end
	return str
end

function array:indexOf(v)
	for i=0,self.length-1 do
		if self[i]==v then return i end
	end
	return -1
end

function array:reverse()
	local t={}
	for i=0,self.length-1 do
		t[i]=self[i]
	end
	for i=0,self.length-1 do
		self[i]=t[self.length-1-i]
	end
end


function array:sort()
	local count = self.length -1
	while count > 0 do
		local k = 0
		for i=0,count-1 do 
			if self[i] > self[i + 1] then 
				local t = self[i]
				self[i] = self[i + 1]
				self[i + 1] = t
				k = i 
			end 
		end 
		count = k
	end
end


function array:sortOn(names,options)
	local count = self.length -1
	while count > 0 do
		local k = 0
		for i=0,count-1 do
			local v1=self[i][names]
			local v2=self[i+1][names]
			if options==16 then
				v1=_G.tonumber(v1) or 0
				v2=_G.tonumber(v2) or 0
			end
			if v1 > v2 then 
				local t = self[i]
				self[i] = self[i + 1]
				self[i + 1] = t
				k = i 
			end 
		end 
		count = k
	end
end


function array:concat(arr)
	local a= self:slice(0,self.length)
	local len = a.length
	for i=0,arr.length do
		a[len+i]=arr[i]
	end
	a.length = a.length+arr.length
	return a;
end


function array:toString()
	return self:join(",")
end
return array
