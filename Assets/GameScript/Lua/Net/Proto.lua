NetProto = {}
NetProto.NetModule = {}

function NetProto.readArray(T,readFunc)
    local arrayCount = T._v_:readVarint16() 
    local o = {}

    for i=1,arrayCount do
        local o2 = {}
        local T2 = NetProto.TE.create(NetProto.RT,T._v_,o2)
        local key = '_array_'..i
        readFunc(T2,key)
        
        if o2[key] ~= nil then
            table.insert(o,o2[key])
        else
            table.insert(o,o2)
        end   
    end
    
    return o
end

function NetProto.writeArray(T,s,writeFunc)

    local len = 0
    local array = T._o_[s]
    assert(array,"REQ param is nil!")

    for k,v in pairs(array) do
        len = len + 1
    end

    T._v_:writeVarint16(len)

    local T2 = NetProto.TE.create(NetProto.WT,T._v_,array)
    for key, var in pairs(array) do
        writeFunc(T2,key)
    end
end

function NetProto.readStruct(T,s,st)
    local o = {}
    local T2 = NetProto.TE.create(NetProto.RT,T._v_,o)
    st(T2)
    T._o_[s] = o
end
      
NetProto.RT = {
    ['result'] = function(self,s)
     local c = self._v_:readVarint8()
     local rst = ( c > 0) 
     self._o_['_result_'] = rst 
     return rst 
     end,
     
    ['string']    = function(self,s) self._o_[s] = self._v_:readString()      end,
    ['int8']      = function(self,s) self._o_[s] = self._v_:readInt8()        end,
    ['int16']     = function(self,s) self._o_[s] = self._v_:readInt16()       end,
    ['int32']     = function(self,s) self._o_[s] = self._v_:readInt32()       end,
    ['int64']     = function(self,s) self._o_[s] = self._v_:readInt64()       end,
    ['vstring']   = function(self,s) self._o_[s] = self._v_:readVarintString()end,
    ['vint8']     = function(self,s) self._o_[s] = self._v_:readVarint8()     end,
    ['vint16']    = function(self,s) self._o_[s] = self._v_:readVarint16()    end,
    ['vint32']    = function(self,s) self._o_[s] = self._v_:readVarint32()    end,
    ['vint64']    = function(self,s) self._o_[s] = self._v_:readVarint64()    end,
    ['array']     = function(self,s,st) self._o_[s] = NetProto.readArray(self,st)  end,
    ['struct']    = NetProto.readStruct,
}
NetProto.RT.__index = NetProto.RT

NetProto.WT = {
    ['result']    = function() return true end,
    ['string']    = function(self,s)assert(self._o_[s])  self._v_:writeString(self._o_[s])         end,
    ['int8']      = function(self,s)assert(self._o_[s])  self._v_:writeInt8(self._o_[s])           end,
    ['int16']     = function(self,s)assert(self._o_[s])  self._v_:writeInt16(self._o_[s])          end,
    ['int32']     = function(self,s)assert(self._o_[s])  self._v_:writeInt32(self._o_[s])          end,
    ['int64']     = function(self,s)assert(self._o_[s])  self._v_:writeInt64(self._o_[s])          end,
    ['vstring']   = function(self,s)assert(self._o_[s])  self._v_:writeVarintString(self._o_[s])   end,
    ['vint8']     = function(self,s)assert(self._o_[s])  self._v_:writeVarint8(self._o_[s])        end,
    ['vint16']    = function(self,s)assert(self._o_[s])  self._v_:writeVarint16(self._o_[s])       end,
    ['vint32']    = function(self,s)assert(self._o_[s])  self._v_:writeVarint32(self._o_[s])       end,
    ['vint64']    = function(self,s)assert(self._o_[s])  self._v_:writeVarint64(self._o_[s])       end,
    ['array']     = function(self,s,st) NetProto.writeArray(self,s,st)                             end
}
NetProto.WT.__index = NetProto.WT

NetProto.st = {
    ['result']   = function() return true end,
    ['int8']     = function(T) T:int8('int8')         end,
    ['int16']    = function(T) T:int16('int16')       end,
    ['int32']    = function(T) T:int32('int32')       end,
    ['int64']    = function(T) T:int64('int64')       end,
    ['string']   = function(T) T:string('string')     end,
    ['vint8']    = function(T) T:vint8('vint8')       end,
    ['vint16']   = function(T) T:vint16('vint16')     end,
    ['vint32']   = function(T) T:vint32('vint32')     end,
    ['vint64']   = function(T) T:vint64('vint64')     end,   
    ['vstring']  = function(T) T:vstring('vstring')   end,
    ['array']    = function(st) return function(T) T:array('array',st) end end,
    ['void']     = function(T) --[[-T:vstring(string.format(%d,#T._o_))]] end,
}

NetProto.TE = {}

function NetProto.TE.create(T,v,o)
    local o2 = {}
    setmetatable(o2,T)
    o2['_v_'] = v
    o2['_o_'] = o
    return o2
end

function NetProto.readStructArray(T,readFunc)
    local arrayCount = T._v_:readVarint16() 
    local o = {}

    for i=1,arrayCount do
        local o2 = {}
        local T2 = NetProto.TE.create(NetProto.RT,T._v_,o2)
        readFunc(T2)
        table.insert(o,o2)
    end
    return o
end

function NetProto.handleSendPackHead(packVarStrem,ModuleNumber,MethodNuber)
    --清空缓冲
    local packBuffer = packVarStrem:byteBuffer()
    packBuffer:clearBuffer()
    
    --写包头
    packVarStrem:writeInt8(ModuleNumber)
    packVarStrem:writeInt8(MethodNuber)
    packVarStrem:writeInt32(0)--预留六字节
    packVarStrem:writeInt16(0)
end

function NetProto.handleReceivePackHead(varStream,ModuleNumber,MethodNuber)
    local buffer = varStream:byteBuffer()

    -- 消耗 1字节包头
    local datatlen = buffer:dataSize()
    local sn = varStream:readInt8()
    
    --读包头
    local module = varStream:readVarint8()
    local method = varStream:readVarint8()
    
    --断言回应方法与之对应
    assert(module == ModuleNumber,'respond module number no match!')
    assert(method == MethodNuber,'respond module number no match!')
end

function NetProto.new(cls)
    local instance = {}
    cls.__index = cls
    rawset(instance,'class',cls)
    setmetatable(instance, cls)
    return instance
end

return nil
