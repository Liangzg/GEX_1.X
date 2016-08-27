GInterfaces = {}
 
function ClearAllInterface()
	for obj,interface in pairs(GInterfaces) do
	   for k, v in pairs(interface) do
		   interface[k] = nil
	   end
	end
	
	GInterfaces = {}
end

function DeclareInterface(obj,intfacename)
	obj._interfaces_ = obj._interfaces_ or {}
	obj._interfaces_[intfacename] = {}
end

function ListenInterface(obj,intfaceName,listener)
	assert(listener,"listener is nil")
	local _interfaces_ = obj._interfaces_
	assert(_interfaces_,"object have no declare any interface!")
	local interface = _interfaces_[intfaceName]
	assert(interface,"object have no declare this interface!")
	
	for key, var in pairs(interface) do
		if var == listener then
			assert(false,"listener already added!")
			return
		end
	end

	if table.empty(interface) then
		GInterfaces[obj] = interface
	end
	
	table.insert(interface,listener)
end

function RemoveInterface(obj,intfaceName,listener)
	assert(listener,"listener is nil")
	local _interfaces_ = obj._interfaces_
	assert(_interfaces_,"object have no declare any interface!")
	local interface = _interfaces_[intfaceName]
	assert(interface,"object have no declare this interface!")
	
	for key, var in pairs(interface) do
		if var == listener then
			interface[key] = nil
			
			if table.empty(interface) then
				GInterfaces[obj] = nil
			end
			
			return
		end
	end

	assert(false,"listener was not be added!")
end

function NotifyEvent(obj,intfaceName,eventName,...)
	local _interfaces_ = obj._interfaces_
	assert(_interfaces_,"object have no declare any interface!")
	local interface = _interfaces_[intfaceName]
	assert(interface,"object have no declare this interface!")

	for k,v in pairs(interface) do
		local fn = v[eventName]
		if fn then
			fn(v,...)
		end
	end
end
