local type = type
local ipairs = ipairs
local unpack = unpack
local string_len = string.len
local string_gsub = string.gsub
local string_sub = string.sub
local string_find = string.find
local table_insert = table.insert
local EventMgr = RequireSingleton("EventMgr")
local NetworkMgr = RequireSingleton("NetworkMgr")

--向C#发送数据
function NetworkMgr:Send(msgFunc, ...)
	--如果是table则转成json
	local arg = {...}
	local params = {}
	for i,v in ipairs(arg) do
		if type(v) == "table" then
			v = "_t" .. CS_Json.encode(v)
		end
		table_insert(params, v)
	end

    if string_find(msgFunc, "G_") == 1 then
        CS_LuaSend('K_Ts', msgFunc, unpack(params))
        return
    end
    CS_LuaSend(msgFunc, unpack(params))
end

--接收服务器消息
function Receive(eventType, ...)
	local arg = {...}
	local params = {}
	for i,v in ipairs(arg) do
		if type(v) == "string" then
			local first = string_sub(v, 0, 3)
			if first == "_t[" or first == "_t{" then
				v = string_sub(v, 3, string_len(v))
				v = CS_Json.decode(v)
			end
		end
		table_insert(params, v)
	end
	EventMgr:DisplayEvent(eventType, unpack(params))
end

--网络
CS_Connect = Network.Connect
CS_Closed = Network.Closed
CS_LuaSend = Network.LuaSend