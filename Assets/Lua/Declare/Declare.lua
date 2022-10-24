--单例类需要在此注册
--回调函数包括：OnInitialize(初始化调用)、OnDayRefresh(凌晨24点刷新调用)、OnLogout(玩家登出调用)
RegistSingleton("BloodMgr")
RegistSingleton("NetworkMgr")
RegistSingleton("EventMgr")
RegistSingleton("CoroutineMgr")
RegistSingleton("MapMgr")
RegistSingleton("StatusMgr")
RegistSingleton("ViewMgr")

--功能相关单例类
RegistSingleton("LoginLogic")
RegistSingleton("CreateLogic")
RegistSingleton("MainLogic")
RegistSingleton("ReadLogic")  
RegistSingleton("RefitLogic")
RegistSingleton("BattleLogic")
RegistSingleton("CheckpointLogic")
RegistSingleton("ShopLogic")


--非单例类类需要在此注册
--UI类需要继承BaseUI
RegistClass("BaseView")
InheritClass("Login_MainUI", "BaseView")
InheritClass("Login_AccountUI", "BaseView")

InheritClass("Common_SecondPromptUI", "BaseView")


InheritClass("CreateView", "BaseView")
InheritClass("Main_MainUI", "BaseView")
InheritClass("Battle_RefitUI","BaseView")
InheritClass("Battle_RobotInfoUI","BaseView")
InheritClass("Battle_MainUI", "BaseView")
InheritClass("Battle_BloodViewUI", "BaseView")
InheritClass("Mail_MainUI", "BaseView")
InheritClass("Refit_MainUI", "BaseView")
InheritClass("Refit_ScendSellUI", "BaseView")
InheritClass("Common_LoadViewUI", "BaseView")
InheritClass("Checkpoint_MainUI", "BaseView")
InheritClass("Checkpoint_CheckUI", "BaseView")
InheritClass("Checkpoint_ReadyUI", "BaseView")
InheritClass("Shop_MainUI", "BaseView")
InheritClass("Battle_ClearUI","BaseView")
InheritClass("Common_TipsUI","BaseView")
InheritClass("Battle_SuspendUI", "BaseView")
InheritClass("Battle_AllSkillUI","BaseView")
InheritClass("Set_MainUI","BaseView")