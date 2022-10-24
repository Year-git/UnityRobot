using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[System.Serializable]
public class KTLevelEditorCommands_Object
{
    [System.Serializable]
    public class PointAndZones
    {
        [ButtonGroup("Group"), Button("位置", ButtonSizes.Medium)]
        public void NewPoint()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewPoint());
        }

		[ButtonGroup("Group"), Button("出生点", ButtonSizes.Medium)]
		public void NewSpawnPoint()
		{
			KTUtils.UseObject<KTLevel>(l => l.NewSpawnPoint());
		}

		[ButtonGroup("Group"), Button("巡逻点", ButtonSizes.Medium)]
        public void NewPatrolPoint()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewPatrolPoint());
        }

        [ButtonGroup("Group"), Button("巡逻路线", ButtonSizes.Medium)]
        public void NewPatrolPath()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewPatrolPath());
        }

        [ButtonGroup("Group"), Button("区域", ButtonSizes.Medium)]
        public void NewPatrolZone()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewPatrolZone());
        }
    }

    [TabGroup("位置 & 区域"), HideLabel]
    public PointAndZones pointAndZones;

    [System.Serializable]
    public class Spawners
    {
        [ButtonGroup("Group"), Button("物件孵化器", ButtonSizes.Medium)]
        public void NewItemSpawner()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewItemSpawner());
        }

        [ButtonGroup("Group"), Button("交互物孵化器", ButtonSizes.Medium)]
        public void NewInteractionItemSpawner()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewInteractionItemSpawner());
        }

        [ButtonGroup("Group"), Button("战斗单位孵化器", ButtonSizes.Medium)]
        public void NewBattleUnitSpawner()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewBattleUnitSpawner());
        }

        [ButtonGroup("Group"), Button("孵化器群组", ButtonSizes.Medium)]
        public void NewSpawnerGroup()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewSpawnerGroup());
        }
    }

    [TabGroup("孵化器"), HideLabel]
    public Spawners spawners;

    [System.Serializable]
    public class Others
    {
        [ButtonGroup("Group"), Button("战斗阶段", ButtonSizes.Medium)]
        public void NewFightStage()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewFightStage());
        }

        [ButtonGroup("Group"), Button("服务器触发器", ButtonSizes.Medium)]
        public void NewServerTrigger()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewServerTrigger());
        }

        [ButtonGroup("Group"), Button("客户端触发器", ButtonSizes.Medium)]
        public void NewClientTrigger()
        {
            KTUtils.UseObject<KTLevel>(l => l.NewClientTrigger());
        }
    }

    [TabGroup("战斗阶段 & 触发器"), HideLabel]
    public Others others;
}
