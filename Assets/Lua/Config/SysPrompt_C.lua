--[[ X-系统提示信息.xlsm 标准
[n] : 提示ID
strPromptInfo : 提示内容
promptPos : 提示位置
dwtype : 提示框按钮
strParamType : 参数类型
IsLed : 走马灯是否在综合频道显示
]]
return {
[100001] = { strPromptInfo='请输入正确的角色名称！', promptPos=1, dwtype=1, strParamType='', IsLed=0, },
[100002] = { strPromptInfo='帐号或者密码错误！', promptPos=2, dwtype=1, strParamType='', IsLed=0, },
[100003] = { strPromptInfo='角色名称重复，请重新输入！', promptPos=1, dwtype=1, strParamType='', IsLed=0, },
[100004] = { strPromptInfo='正在连接服务器！', promptPos=2, dwtype=1, strParamType='', IsLed=0, },
[100005] = { strPromptInfo='该帐号已被注册！', promptPos=2, dwtype=1, strParamType='', IsLed=0, },
[100006] = { strPromptInfo='与服务器断开连接', promptPos=2, dwtype=1, strParamType='', IsLed=0, },
}