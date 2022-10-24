using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlaySoundScript : MonoBehaviour
{
    private static GameObject _audioList;
    private static int incInstId = 0;

    private Dictionary<int, AudioData> _audioContainer = new Dictionary<int, AudioData>();
    private List<int> _freeAudioData = new List<int>();
    private Dictionary<int,int> _useAudioData = new Dictionary<int, int>();

    [Header("Sound Set")]
    public AudioClip audioClip;
    public float audioVolume = 1f;
    public bool isLoop;

    [Header("Play Way")]
    public bool enablePushPlay = false;
    public float pushPlayDelay = 1f;
    private float _pushPlayNextTime = 0f;

    [Header("Auto Collision Play")]
    public bool enableCollision = false;
    public bool onlyRobotPart = false;

    public bool isPlay(int nInstId)
    {
        if (_useAudioData.ContainsKey(nInstId))
        {
            return true;
        }
        return false;
    }

    private void Start()
    {
        if (audioClip == null)
        {
            Debug.LogError("PlaySoundScript -> [audioClip] Is Null");
        }

        if (enableCollision == true)
        {
            if (isLoop == true)
            {
                Debug.LogError("PlaySoundScript -> isLoop Can Not Is True When [enableCollision] Is True");
            }

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                Debug.LogError("PlaySoundScript -> The Prefab Must Have Rigidbody Component When [enableCollision] Is True");
            }
        }

        if (_audioList == null)
        {
            _audioList = new GameObject("SceneAudioList");
        }
        _freeAudioData.Add(CreateAudio().instId);
    }

    private AudioData CreateAudio()
    {
        AudioData pAudioData = new AudioData();
        GameObject pGameObj = new GameObject(audioClip.name);
        pGameObj.SetActive(false);
        pGameObj.transform.parent = _audioList.transform;
        pAudioData.gameObj = pGameObj;

        AudioSource pAudioSource = pGameObj.AddComponent<AudioSource>();
        pAudioSource.clip = audioClip;
        pAudioSource.clip.LoadAudioData();
        pAudioSource.playOnAwake = true;
        pAudioSource.loop = isLoop;
        pAudioSource.volume = 1f;
        pAudioSource.spatialBlend = 1f;
        pAudioSource.volume = audioVolume;
        pAudioSource.minDistance = 20f;
        pAudioData.audioSource = pAudioSource;

        incInstId++;
        pAudioData.instId = incInstId;

        _audioContainer.Add(pAudioData.instId, pAudioData);
        return pAudioData;
    }

    private void Update()
    {
        if (_useAudioData.Count == 0)
        {
            return;
        }

        float nCurTime = Time.realtimeSinceStartup;
        List<int> stopList = new List<int>();
        foreach(var kvPair in _useAudioData)
        {
            AudioData pAudio = GetAudio(kvPair.Key);
            if (pAudio != null)
            {
                // 更新及身特效位置
                if (pAudio.parent != null)
                {
                    pAudio.gameObj.transform.position = pAudio.parent.position;
                }

                // 检查是否有特效结束
                float nEndTime = pAudio.endTime;
                if (!isLoop && nEndTime != 0f && nCurTime >= nEndTime)
                {
                    stopList.Add(pAudio.instId);
                }
            }
        }
        foreach(var nInstId in stopList)
        {
            Stop(nInstId);
        }
    }

    private void OnDestroy()
    {
        foreach(var nInstId in _freeAudioData)
        {
            var pAudioData = GetAudio(nInstId);
            if (pAudioData != null)
            {
                GameObject.Destroy(pAudioData.gameObj);
            }
        }
        foreach(var kvPair in _useAudioData)
        {
            var pAudioData = GetAudio(kvPair.Key);
            if (pAudioData != null)
            {
                GameObject.Destroy(pAudioData.gameObj);
            }
        }
    }

    private AudioData GetAudio(int nInstId)
    {
        if (!_audioContainer.ContainsKey(nInstId))
        {
            return null;
        }
        return _audioContainer[nInstId];
    }

    private AudioData GetFreeAudio()
    {
        AudioData pAudio;
        if (_freeAudioData.Count == 0)
        {
            pAudio = CreateAudio();
        }
        else
        {
            int nInstId = _freeAudioData[0];
            _freeAudioData.RemoveAt(0);
            pAudio = GetAudio(nInstId);
        }
        _useAudioData.Add(pAudio.instId,0);
        return pAudio;
    }

    public int Play(Vector3 pPosition)
    {
        if (enablePushPlay == true)
        {
            return PushPlay(pPosition);
        }
        else
        {
            return NormalPlay(delegate(AudioData pAudioData)
                {
                    pAudioData.gameObj.transform.position = pPosition;
                }
            );
        }
    }

    public int Play(Transform pParent)
    {
        if (enablePushPlay == true)
        {
            return PushPlay(pParent);
        }
        else
        {
            return NormalPlay(delegate(AudioData pAudioData)
                {
                    pAudioData.parent = pParent;
                    pAudioData.gameObj.transform.position = pParent.position;
                }
            );
        }
    }

    private int NormalPlay(Action<AudioData> fInit)
    {
        var pAudio = GetFreeAudio();
        fInit?.Invoke(pAudio);
        if (!isLoop)
        {
            pAudio.endTime = Time.realtimeSinceStartup + pAudio.audioSource.clip.length;
        }
        pAudio.gameObj.SetActive(true);
        return pAudio.instId;
    }

    public int NormalPlay(Vector3 pPosition)
    {
        return NormalPlay(delegate(AudioData pAudioData)
            {
                pAudioData.gameObj.transform.position = pPosition;
            }
        );
    }

    public int NormalPlay(Transform pParent)
    {
        return NormalPlay(delegate(AudioData pAudioData)
            {
                pAudioData.parent = pParent;
                pAudioData.gameObj.transform.position = pParent.position;
            }
        );
    }

    public delegate int PlayAction();
    private int PushPlay(PlayAction fPlay)
    {
        int nIdx = -1;
        float nCurTime = Time.realtimeSinceStartup;
        if (nCurTime >= _pushPlayNextTime)
        {
            nIdx = fPlay.Invoke();
        }
        _pushPlayNextTime = nCurTime + pushPlayDelay;
        return nIdx;
    }

    private int PushPlay(Vector3 pPosition)
    {
        return PushPlay(delegate()
            {
                return NormalPlay(pPosition);
            }
        );
    }

    private int PushPlay(Transform pParent)
    {
        return PushPlay(delegate()
            {
                return NormalPlay(pParent);
            }
        );
    }

    public void Stop(int nInstId)
    {
        if (_audioContainer.ContainsKey(nInstId))
        {
            if (_useAudioData.ContainsKey(nInstId))
            {
                var pAudio = GetAudio(nInstId);
                if (pAudio != null)
                {
                    pAudio.parent = null;
                    pAudio.endTime = 0f;
                    pAudio.gameObj.SetActive(false);
                    _freeAudioData.Add(nInstId);
                    _useAudioData.Remove(nInstId);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableCollision == false)
        {
            return;
        }

        ContactPoint contactPoint = collision.GetContact(0);

        if (onlyRobotPart == true)
        {
            RobotPartScriptBase pPart = contactPoint.thisCollider.gameObject.GetComponentInParent<RobotPartScriptBase>();
            if (pPart == null)
            {
                return;
            }
            PushPlay(contactPoint.point);
            return;
        }
        
        PushPlay(contactPoint.point);
    }
}

class AudioData
{
    public AudioSource audioSource;
    public GameObject gameObj;
    public Transform parent;
    public int instId;
    public float endTime = 0f;
}