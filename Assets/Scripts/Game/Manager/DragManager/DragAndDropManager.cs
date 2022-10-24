using System;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using LuaInterface;
using Newtonsoft.Json.Linq;
//重写DragDropManager
public  class DragAndDropManager
{
	private GLoader _agent;
	private object _sourceData;
	//private GameObject go;
	private List<Vector3> _holepos=new List<Vector3>();
	private List<int> _holeid=new List<int>();
	private float posrange=1.5f;//以槽位为中心的范围大小
	private int Partid;
	private static DragAndDropManager _inst;
	public static DragAndDropManager inst
	{
		get
		{
			if (_inst == null)
				_inst = new DragAndDropManager();
			return _inst;
		}
	}

	private Camera Cam
	{
		get
		{
			return GameObject.Find("Camera/Main Camera").GetComponent<Camera>();
		}
	}
	private Camera UICam
	{
		get
		{
			return GameObject.Find("Stage Camera").GetComponent<Camera>();
		}
	}
	/// <summary>
	/// Start dragging.
	/// 开始拖动。
	/// </summary>
	public void StartDrag(string partpath,LuaFunction luaFun,LuaTable luaTable = null)
	{
		int id=RobotPart.GetRobotPartModelId(int.Parse(partpath));
		string sModelName = Model.GetModelName(id);
		Partid=int.Parse(partpath);
		_holepos.Clear();
		_holeid.Clear();
		Model mo=new Model(null, sModelName, new Vector3(0, 0, -1000), new Vector3(0, 100, 0), 
		100f, delegate (Model model)
            {
				if(model!=null)
				{
					if (luaFun != null)
                    {
                        luaFun.BeginPCall();
                        if (luaTable != null)
                        {
                            luaFun.Push(luaTable);
                        }
                        luaFun.Push(model.gameObject);
                        luaFun.PCall();
                        luaFun.EndPCall();
                    }
					//当拖拽的配件生成时,以此配件能安防的槽位点为中心 生成一个正方形，正方形所在的范围为可拖拽上去的范围
					RobotPartType robotPartType = RobotPart.GetRobotPartType(Partid);
					BaseNpc mynpc= MapManager.Instance.baseMap.GetNpc(MyPlayer.playerInstId);
					List<int> holelist=mynpc.GetAllHoleByType(robotPartType);
					for(int i=0;i<holelist.Count;i++)
					{
						foreach(Transform item in mynpc.myModel.transform.GetComponentInChildren<Transform>())
						{
							if(item.name==holelist[i].ToString())
							{
								//坐标转换
								Vector3 pos=Cam.WorldToScreenPoint(item.position);
								Vector3 screenpos=UICam.ScreenToWorldPoint(pos);
								_holepos.Add(screenpos);
								_holeid.Add(holelist[i]);
							}
						}		
					}
				}
			});				
	}
	public void DragEnd()
	{
		int nHoleIdx=-1;
		//射线检测
		#region
		/*Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			Vector3 point = hit.point;     
			if(hit.collider!=null)
			{		
				//先检测是不是替换配件,不是即为第一次安装配件
				RobotPartScriptBase pPart=hit.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
				if(pPart!=null)
				{
					nHoleIdx=pPart.myElement.myRobotPart.holeIdx;			
					LuaExtend.NpcRobotPartInstall(MyPlayer.playerInstId,nHoleIdx,Partid,null,null);
				}
				else
				{

					if(int.TryParse(hit.collider.gameObject.name,out nHoleIdx))
					{
						NpcBase mynpc= MapManager.Instance.baseMap.GetNpc(MyPlayer.playerInstId);
						RobotPartType robotPartType = RobotPart.GetRobotPartType(Partid);			
						//int nIndex = mynpc.GetHolePassPartType(robotPartType);
						LuaExtend.NpcRobotPartInstall(MyPlayer.playerInstId,nHoleIdx,Partid,null,null);
					}
					else
					{
						Debug.LogError("拖拽位置有误");
					}
					
				}					
			}
			else
			{
				Debug.LogError("未检测到物体");
			}
		}*/
		#endregion

		//范围检测 与_holepos表里数据一一对比 符合范围的即为可安放
		Vector3 screenPos1 = UICam.ScreenToWorldPoint(Input.mousePosition);
		if(_holepos!=null)
		{
			for(int i=0;i<_holepos.Count;i++)
			{
				if(Math.Abs(_holepos[i].x-screenPos1.x)<posrange&&Math.Abs(_holepos[i].y-screenPos1.y)<posrange)
				{
					//符合条件
					nHoleIdx=_holeid[i];
					LuaExtend.NpcRobotPartInstall(MyPlayer.playerInstId,nHoleIdx,Partid,null,null);
					break;
				}
			}
		}  
		
	}
}
