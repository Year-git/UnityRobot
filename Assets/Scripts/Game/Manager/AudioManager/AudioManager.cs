using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Framework;

public static class AudioManager
{
    // 存放音频文件路径
    private static Dictionary<int, string> _audioPathDict = new Dictionary<int, string>();
    private static AudioSource _musicAudioSource;
    // 存放可以使用的音频组件
    private static List<AudioSource> _unusedSoundAudioSourceList = new List<AudioSource>();
    // 存放正在使用的音频组件
    private static List<AudioSource> _usedSoundAudioSourceList = new List<AudioSource>();
    // 缓存音频文件
    private static Dictionary<int, AudioClip> _audioClipDict = new Dictionary<int, AudioClip>();
    private static float _musicVolume = 1;
    private static float _soundVolume = 1;
    private static readonly string _musicVolumePrefs = "MusicVolume";
    private static readonly string _soundVolumePrefs = "SoundVolume";
    // 对象池数量
    private static readonly int _poolCount = 5;
    private static GameObject _gameObject;

    public static void Start()
    {
        _gameObject = new GameObject("Audio");
        Object.DontDestroyOnLoad(_gameObject);

        _musicAudioSource = _gameObject.AddComponent<AudioSource>();

        // 从本地缓存读取声音音量
        if (PlayerPrefs.HasKey(_musicVolumePrefs))
        {
            _musicVolume = PlayerPrefs.GetFloat(_musicVolumePrefs);
        }
        if (PlayerPrefs.HasKey(_soundVolumePrefs))
        {
            _musicVolume = PlayerPrefs.GetFloat(_soundVolumePrefs);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="id"></param>
    /// <param name="loop"></param>
    public static void PlayMusic(int id, bool loop = true)
    {
        // 通过Tween将声音淡入淡出
        DOTween.To(() => _musicAudioSource.volume, value => _musicAudioSource.volume = value, 0, 0.5f).OnComplete(() =>
        {
            _musicAudioSource.clip = GetAudioClip(id);
            _musicAudioSource.clip.LoadAudioData();
            _musicAudioSource.loop = loop;
            _musicAudioSource.volume = _musicVolume;
            _musicAudioSource.Play();
            DOTween.To(() => _musicAudioSource.volume, value => _musicAudioSource.volume = value, _musicVolume, 0.5f);
        });
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="id"></param>
    public static void PlaySound(int id)
    {
        if (_unusedSoundAudioSourceList.Count != 0)
        {
            AudioSource audioSource = UnusedToUsed();
            audioSource.clip = GetAudioClip(id);
            audioSource.clip.LoadAudioData();
            audioSource.Play();

            Timer.CreateTimer(audioSource.clip.length, false, UsedToUnused, audioSource);
        }
        else
        {
            AddAudioSource();

            AudioSource audioSource = UnusedToUsed();
            audioSource.clip = GetAudioClip(id);
            audioSource.clip.LoadAudioData();
            audioSource.volume = _soundVolume;
            audioSource.loop = false;
            audioSource.Play();

            Timer.CreateTimer(audioSource.clip.length, false, UsedToUnused, audioSource);
        }
    }

    /// <summary>
    /// 获取音频文件，获取后会缓存一份
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private static AudioClip GetAudioClip(int id)
    {
        if (!_audioClipDict.ContainsKey(id))
        {
            if (!_audioPathDict.ContainsKey(id))
            {
                //通过配置表获取音频路径
                return null;
            }
            AudioClip ac = Resources.Load(_audioPathDict[id]) as AudioClip;
            _audioClipDict.Add(id, ac);
        }
        return _audioClipDict[id];
    }

    /// <summary>
    /// 添加音频组件
    /// </summary>
    /// <returns></returns>
    private static AudioSource AddAudioSource()
    {
        if (_unusedSoundAudioSourceList.Count != 0)
        {
            return UnusedToUsed();
        }
        else
        {
            AudioSource audioSource = _gameObject.AddComponent<AudioSource>();
            _unusedSoundAudioSourceList.Add(audioSource);
            return audioSource;
        }
    }

    /// <summary>
    /// 将未使用的音频组件移至已使用集合里
    /// </summary>
    /// <returns></returns>
    private static AudioSource UnusedToUsed()
    {
        AudioSource audioSource = _unusedSoundAudioSourceList[0];
        _unusedSoundAudioSourceList.RemoveAt(0);
        _usedSoundAudioSourceList.Add(audioSource);
        return audioSource;
    }

    /// <summary>
    /// 将使用完的音频组件移至未使用集合里
    /// </summary>
    /// <param name="audioSource"></param>
    private static void UsedToUnused(AudioSource audioSource)
    {
        _usedSoundAudioSourceList.Remove(audioSource);
        if (_unusedSoundAudioSourceList.Count >= _poolCount)
        {
            Object.Destroy(audioSource);
        }
        else
        {
            _unusedSoundAudioSourceList.Add(audioSource);
        }
    }

    /// <summary>
    /// 修改背景音乐音量
    /// </summary>
    /// <param name="volume"></param>
    public static void ChangeMusicVolume(float volume)
    {
        _musicVolume = volume;
        _musicAudioSource.volume = volume;

        PlayerPrefs.SetFloat(_musicVolumePrefs, volume);
    }

    /// <summary>
    /// 修改音效音量
    /// </summary>
    /// <param name="volume"></param>
    public static void ChangeSoundVolume(float volume)
    {
        _soundVolume = volume;
        for (int i = 0; i < _unusedSoundAudioSourceList.Count; i++)
        {
            _unusedSoundAudioSourceList[i].volume = volume;
        }
        for (int i = 0; i < _usedSoundAudioSourceList.Count; i++)
        {
            _usedSoundAudioSourceList[i].volume = volume;
        }
        PlayerPrefs.SetFloat(_soundVolumePrefs, volume);
    }    
}