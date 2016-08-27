require "Common.functions"
require ("Net/init")
SocketTest = {}

function SocketTest:OnStart()
   
    NetClient:setIp("192.168.21.81",10102)
    GameNet:startNetClient()
    print("net test ##########################################################################")
    local req = NetProtoUser.SendVisitorLogin:new()
    req.userName = "abc"
    req.password = "123123"
    req:send(function(AccountVO)
          print(AccountVO.accountName)
        end)
end
