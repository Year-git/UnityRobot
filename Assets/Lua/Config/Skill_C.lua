--[[ J-技能表.xlsm Sheet1
[n] : 技能ID
skillname : 技能名字
skilldesc : 技能描述
strSkillIcon : 技能图标
skillCd : 技能CD(ms)
actionIndex : 动作索引
maxRange : 最大范围
minRange : 最小范围
directionRange : 朝向范围
beforeTime : 前摇时间
durationTime : 持续时间
afterTime : 后摇时间
auto : 是否自动释放
warning : 是否预警
]]
return {
--//攻击},
[20100] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_yuanju_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[20101] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_yuanju_01', skillCd=5000, actionIndex=-1, maxRange=50, minRange=0, directionRange=15, beforeTime=500, durationTime=2500, afterTime=1000, auto=0, warning=1, },
[20102] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_yuanju_01', skillCd=3000, actionIndex=-1, maxRange=50, minRange=0, directionRange=15, beforeTime=500, durationTime=500, afterTime=500, auto=0, warning=0, },
[20103] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_yuanju_01', skillCd=500, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=1, warning=0, },
[20200] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_jupao_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[20201] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_jupao_01', skillCd=1000, actionIndex=-1, maxRange=60, minRange=5, directionRange=10, beforeTime=500, durationTime=0, afterTime=500, auto=0, warning=1, },
[20202] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_jupao_01', skillCd=2000, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=1, },
[20203] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_jupao_01', skillCd=500, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=1, warning=0, },
[20204] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_jupao_01', skillCd=1000, actionIndex=-1, maxRange=60, minRange=5, directionRange=10, beforeTime=500, durationTime=4000, afterTime=1000, auto=0, warning=1, },
[20300] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_zhuizi_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[20400] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_chuizi_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[20500] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_quantou_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[20501] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_quantou_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=1, },
[20600] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_zhongchan_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[20601] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_zhongchan_01', skillCd=200, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=1, warning=0, },
--//辅助},
[40100] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_tuijinqi_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[40300] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_zhongchan_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[40400] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_baoqiping_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[40500] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_paichihuan_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[40501] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_paichihuan_01', skillCd=0, actionIndex=1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[40600] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_baoqiping_01', skillCd=10000, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
[40700] = { skillname=0, skilldesc=0, strSkillIcon='icon_skill_baoqiping_01', skillCd=0, actionIndex=-1, maxRange=999, minRange=0, directionRange=180, beforeTime=0, durationTime=0, afterTime=0, auto=0, warning=0, },
}