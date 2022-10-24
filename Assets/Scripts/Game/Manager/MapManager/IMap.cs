using System;

public interface IMap{
    void OnLoad(int mapID, Action callBack);
    void OnEnter();
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();
    void OnExit();
}