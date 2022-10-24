--[[
	--@brief 协程管理类
	--除了CoroutineMgr:Start和CoroutineMgr:Stop之外，其他的协程方法只允许在协程函数的内部使用
--]]
local CoroutineMgr = RequireSingleton("CoroutineMgr")

--[[
	--@brief 协程函数的开启
	--@param fun 协程函数
	--@return 返回协程对象
--]]
function CoroutineMgr:Start(fun)
	if not fun then
		CS_LogError("coroutine fun is nil, start coroutine fail!")
		return
	end
	return coroutine.start(fun)
end

--[[
	--@brief 协程函数的挂起
--]]
function CoroutineMgr:Step()
	coroutine.step()
end

--[[
	--@brief 协程函数的延时
	--@param time 秒
--]]
function CoroutineMgr:Wait(time)
	coroutine.wait(time)
end

--[[
	--@brief 协程函数的结束
	--@param obj 协程对象
--]]
function CoroutineMgr:Stop(obj)
	if not obj then
		CS_LogError("coroutine is nil, stop coroutine fail!")
		return
	end
	coroutine.stop(fun)
end

--[[
	--@brief 协程下载
	--@param url 网址
--]]
function CoroutineMgr:WWW(url)
	coroutine.www(url)
end