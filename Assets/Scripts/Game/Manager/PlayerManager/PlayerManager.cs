using Framework;
using System.Collections.Generic;
using System;

public class PlayerManager : Singleton<PlayerManager>
{
    /// <summary>
    /// 玩家列表
    /// </summary>
    private Dictionary<int, PlayerNpc> _playerData = new Dictionary<int, PlayerNpc>();

    public PlayerNpc Get(int instanceID)
    {
        PlayerNpc ret = null;
        _playerData.TryGetValue(instanceID, out ret);
        return ret;
    }

    // public Player Create(int instanceID, int configID, Action<NpcBase> loaded = null)
    // {
    //     if (_playerData.ContainsKey(instanceID)) return null;
    //     Player ret = new Player(instanceID, configID, loaded);
    //     _playerData.Add(instanceID, ret);
    //     return ret;
    // }

    public void Destroy(int instanceID)
    {
        if (!_playerData.ContainsKey(instanceID)) return;
        PlayerNpc ret = _playerData[instanceID];
        ret.NpcDestroy();
        _playerData.Remove(instanceID);
    }

    public void DestroyAll()
    {
        PlayerNpc myPlayer = MyPlayer.player;
        if (myPlayer != null)
        {
            _playerData.Remove(myPlayer.InstId);
        }
        foreach (int key in _playerData.Keys)
        {
            _playerData[key].NpcDestroy();
        }
        _playerData.Clear();
        if (myPlayer != null)
        {
            _playerData.Add(myPlayer.InstId, myPlayer);
        }
    }
}