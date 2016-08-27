require('Net/Proto')
require('Net/Interface')
require "Net/NetClient"
Dequeue = require ('Net/Dequeue')
--��������
GameNet = NetClient
NetClient:init()
--NetClient:setIp("192.168.22.45",10102)


local function load_net_file(file)
    --table.insert(require_files,'Net/Protocol/'..file)
    require('Net/Protocol/'..file)
    --log("Net/Protocol/"..file)
end

load_net_file('Activity')
load_net_file('Admin')
load_net_file('Api')
load_net_file('Chat')
load_net_file('Item')
load_net_file('Mail')
load_net_file('Notice')
load_net_file('Task')
load_net_file('User')
