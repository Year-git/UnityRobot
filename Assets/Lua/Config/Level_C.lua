--[[ G-关卡表.xlsm 场景表
[n] : 关卡ID
strLevelName : 关卡名字
sceneId : 场景ID
preLevelId : 前置关卡ID
type : 类型
strDirectorAi : 导演AI
drop : 掉落(结算奖励展示)
strLevelIcon : 关卡图标
xCoordinates : 图标的X坐标
yCoordinates : 图标的Y坐标
angle : 图标的角度
victoryCondition : 通关条件
physical : 体力消耗
modelShow : 怪物模型展示
RewardIcon : 奖励展示
ownPart : 玩家拥有的部件
delayTime : 结算UI延迟时间
]]
return {
[1001] = { strLevelName='1', sceneId=10000, preLevelId=0, type=1, strDirectorAi='MapDirector_0', drop={30100,10000}, strLevelIcon='0', xCoordinates=80, yCoordinates=189, angle=0, victoryCondition=0, physical={5,10}, modelShow={101000,150000}, RewardIcon=0, ownPart={10000,10001,20300,20301,20500,20501,30200,40400}, delayTime=2000, },
[1002] = { strLevelName='2', sceneId=10001, preLevelId=1001, type=1, strDirectorAi='MapDirector_0', drop={30101,10100,20400}, strLevelIcon='0', xCoordinates=534, yCoordinates=87, angle=0, victoryCondition=0, physical={5,10}, modelShow={102010,150000}, RewardIcon=0, ownPart={10000,10001,20300,20301,20500,20501,30200,40400}, delayTime=2000, },
[1003] = { strLevelName='3', sceneId=10001, preLevelId=1002, type=1, strDirectorAi='MapDirector_0', drop={20400,20300,30100}, strLevelIcon='0', xCoordinates=946, yCoordinates=71, angle=0, victoryCondition=0, physical={5,10}, modelShow={103010,150000}, RewardIcon=0, ownPart={10000,10001,10002,20300,20301,20500,20501,30200,40400}, delayTime=2000, },
[1004] = { strLevelName='4', sceneId=10001, preLevelId=1003, type=2, strDirectorAi='MapDirector_0', drop={10200,20500,}, strLevelIcon='0', xCoordinates=1613, yCoordinates=59, angle=0, victoryCondition=0, physical={5,10}, modelShow={104020,150000}, RewardIcon=40100, ownPart={10000,10001,10002,20300,20301,20500,20501,30200,40400}, delayTime=2000, },
[1005] = { strLevelName='5', sceneId=10000, preLevelId=1004, type=1, strDirectorAi='MapDirector_1', drop={20300,10000,20400}, strLevelIcon='0', xCoordinates=2220, yCoordinates=133, angle=0, victoryCondition=0, physical={5,10}, modelShow={105030,150000}, RewardIcon=0, ownPart={10000,10001,10002,10003,20100,20101,20300,20301,20500,20501,30200,40100,40400}, delayTime=2000, },
[1006] = { strLevelName='6', sceneId=10000, preLevelId=1005, type=1, strDirectorAi='MapDirector_0', drop={20300,40400}, strLevelIcon='0', xCoordinates=71, yCoordinates=515, angle=0, victoryCondition=0, physical={5,10}, modelShow={106030,150000}, RewardIcon=0, ownPart={10000,10001,10002,10003,20100,20101,20300,20301,20500,20501,30200,40100,40400}, delayTime=2000, },
[1007] = { strLevelName='7', sceneId=10000, preLevelId=1006, type=2, strDirectorAi='MapDirector_0', drop={40100,30101,20100}, strLevelIcon='0', xCoordinates=735, yCoordinates=553, angle=0, victoryCondition=0, physical={5,10}, modelShow={107010,150000}, RewardIcon=20100, ownPart={10000,10001,10002,10003,10004,20100,20101,20300,20301,20500,20501,20602,20603,30200,40100,40400}, delayTime=2000, },
[1008] = { strLevelName='8', sceneId=10001, preLevelId=1007, type=1, strDirectorAi='MapDirector_1', drop={10100,20500}, strLevelIcon='0', xCoordinates=1360, yCoordinates=447, angle=0, victoryCondition=0, physical={5,10}, modelShow={108040,150000}, RewardIcon=0, ownPart={10000,10001,10002,10003,10004,20100,20101,20300,20301,20500,20501,20602,20603,30200,40100,40400}, delayTime=2000, },
[1009] = { strLevelName='9', sceneId=10001, preLevelId=1008, type=1, strDirectorAi='MapDirector_1', drop={30100,10200,20301}, strLevelIcon='0', xCoordinates=2032, yCoordinates=568, angle=0, victoryCondition=0, physical={5,10}, modelShow={109040,150000}, RewardIcon=0, ownPart={10000,10001,10002,10003,10004,20100,20101,20300,20301,20500,20501,20602,20603,30200,40100,40400}, delayTime=2000, },
[1010] = { strLevelName='10', sceneId=10000, preLevelId=1009, type=2, strDirectorAi='MapDirector_20191205_5', drop={20200,10100,30100,40100,20400}, strLevelIcon='0', xCoordinates=2619, yCoordinates=423, angle=0, victoryCondition=0, physical={5,10}, modelShow={110000,150000}, RewardIcon=20201, ownPart={10000,10001,10002,10003,10004,20100,20101,20200,20300,20301,20500,20501,20602,20603,30200,40100,40400}, delayTime=2000, },
}