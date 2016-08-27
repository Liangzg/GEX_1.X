local Dequeue = {}
Dequeue = class('Dequeue')

function Dequeue:ctor()  
    self.first = 0
    self.last  = -1
    self.data  = {}
end

function Dequeue:clone()
    local o = Dequeue:new()
    
    o.first = self.first
    o.last  = self.last
    
    for i=o.first,o.last do
    	o.data[i] = self.data[i]
    end
    
    return o
end

function Dequeue:endIndex()
    return self.last + 1
end

function Dequeue:removeAll()
    self.first = 0
    self.last  = -1
    self.data  = {}
end

function Dequeue:clear()
    self:removeAll()
end

function Dequeue:removeIndex(index)
    local currentIndex = index
    
    assert(self:inRange(currentIndex),'index is not in range')
    
    self.data[currentIndex] = nil
    
    while currentIndex < self.last do
        self.data[currentIndex] = self.data[currentIndex + 1]
        currentIndex = currentIndex + 1
    end
    
    self.data[currentIndex] = nil
    
    self.last = self.last - 1
end
 
function Dequeue:insert(o)
    if self:containObject(o) then
    	return
    end
    
    self:pushBack(o)
end
 
function Dequeue:remove(o)
    local enum = self:enum()

    while (not enum:enumEnd()) do
        local o2 = enum:get()
        if o2 == nil then break end    
        if o == o2 then 
            self:removeIndex(enum.enumIndex)
        else
            enum:toNext()
        end
    end
end

function Dequeue:removeList(list)
    for i, v in ipairs(list) do
    	self:remove(v)
    end
end

function Dequeue:removeFromTo(from,to)
    assert(self:inRange(from),"invalid 'from' index")
    
    local t = from
    local removeToEnd = not self:inRange(to)
    
    if removeToEnd then
        while from <= self.last do
            self.data[from] = nil
            from = from + 1
        end
        self.last = t - 1
    else
        local i = 0
        while from <= to do
            if (to + i) <= self.last then
             self.data[from] = self.data[to + i]
             i = i + 1
            else
                self.data[from] = nil
            end
            
            from = from + 1
        end
    
        self.last = t - 1 + i
    end
end

function Dequeue:front()
    return self.data[self.first]
end

function Dequeue:back()
    return self.data[self.last]
end

function Dequeue:pushFront(value)
    assert(value ~= nil,'value can not be nil') 
    local first= self.first-1  
    self.data[first] = value  
    self.first = first  
end  

function Dequeue:pushBack(value)
    assert(value ~= nil,'value can not be nil') 
    local last  = self.last + 1
    self.data[last]= value  
    self.last=last  
end  

function Dequeue:popFront()  
    local last  = self.last
    local first = self.first
    
    if(last<first) then  
        error("list is empty")  
    end
    
    local value= self.data[first]  
    self.data[first] = nil
    self.first = first + 1  
    return value  
end  

function Dequeue:popBack()  
    local last  = self.last
    local first = self.first
    if last<first then  
        error("the list is empty")  
    end  
    local value = self.data[last]  
    self.data[last]= nil  
    self.last= last-1  
    return value  
end 

function Dequeue:size()
    if self.last < self.first then
        return 0
    end
    
    return (self.last - self.first) + 1
end 

function Dequeue:empty()
    local last  = self.last
    local first = self.first
     
    if first > last then  
       return true
    end 
    
    return false
end

function Dequeue:travel(func)
    local last  = self.last
    local first = self.first

    if first < last then
      
        if not func(self.data[first]) then
            return
        end
        
        first = first + 1
    else
        func(nil)
    end 
end

local Enumtor = {}
Enumtor.__index = Enumtor

function Enumtor:enumNext()
    local last  = self.dq.last

    if self.enumIndex <= last then
        local rst = self.dq.data[self.enumIndex]
        self.enumIndex = self.enumIndex + 1
        return rst
    else
        return nil
    end 
end

function Enumtor:get()
    assert(not self:enumEnd(),"invalid enumrator.")
    return self.dq:at(self.enumIndex)
end

function Enumtor:toNext()
    self.enumIndex = self.enumIndex + 1
end

function Enumtor:EQ(o2)
    assert((self.dq == o2.dq),"invalid EQ.")

    if self.enumIndex ~= o2.enumIndex then
        return false
    end

    return true
end

function Enumtor:enumEnd()
    return (not self.dq:inRange(self.enumIndex))
end

function Enumtor:endIndex()
    return self.dq:endIndex()
end

function Enumtor:clone()
    local o = {}
    setmetatable(o,Enumtor)
    o.enumIndex = self.enumIndex
    o.dq        = self.dq
    return o
end

function Dequeue:enum()
    local o = {}
    setmetatable(o,Enumtor)
    o.enumIndex = self.first
    o.dq = self

    return o
end

function Dequeue:inRange(index)
    if self:empty() then
    	return false   
    end
    
    index = index + self.first
    
    if index < self.first or index > self.last then
       return false  
    end
    
    return true
end

function Dequeue:findObject(o)
    self:enum()

    while true do
        local rst = self:enumNext()
        if rst == nil then break end    
        if o == o2 then return self.enumIndex - 1 end
    end

    return self.last + 1
end

function Dequeue:containObject(o)
    self:enum()
    
    while true do 
        local o2 = self:enumNext()
        if o2 == nil then break end	
        if o == o2 then return true end
    end

    return false
end

function Dequeue:at(index)
    if self:inRange(index) == false then
        error("access dequeue out of rang.") 
    end
    
    index = index + self.first
    local o = self.data[index]
    assert(o ~= nil)
    return o
end

return Dequeue