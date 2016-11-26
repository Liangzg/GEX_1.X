--[[
	Author： LiangZG
	Email :  game.liangzg@foxmail.com
	Desc : stringBuffer 
]]


local stringBuffer = class("stringBuffer")
local m = stringBuffer

function m:ctor( )
	self.buf = {}
	self._size = 0
end

function m:append( str )
	table.insert(self.buf , str)
	self._size = self._size + 1
end


function m:appendFormat( format , ... )
	local str = string.format(format , ...)

	self:append(str)
end


function m:appendLine( str )
	self:append(str)
	table.insert(self.buf , "\n")
end

function m:remove( str )
	for i=self:size(),1,-1 do
		if self.buf[i] == str then
			table.remove(self.buf , i)
			self._size = self._size - 1
			return 
		end
	end
end


function m:toString()
	return table.concat(self.buf, "")
end

function m:clear()
	self.buf = {}
	self._size = 0
end

function m:size()
	return self._size
end

return stringBuffer