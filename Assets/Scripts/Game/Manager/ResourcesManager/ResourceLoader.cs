using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public class AssetLoader : Looper
    {
        private AssetBundle _assetBundle;
        private AssetBundleRequest _request;
        private TaskCompletionSource<bool> _tcs;

        public AssetLoader(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
        }

        public override void Update()
        {
            if (IsDisposed())
                return;

            if (!_request.isDone)
                return;

            Detach();
            TaskCompletionSource<bool> tcs = _tcs;
            _tcs = null;
            tcs?.SetResult(true);
        }
        public async Task<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            await InnerLoadAssetAsync<T>(name);
            return _request.asset as T;
        }
        private Task<bool> InnerLoadAssetAsync<T>(string name)
        {
            _tcs = new TaskCompletionSource<bool>();
            _request = _assetBundle.LoadAssetAsync<T>(name);
            return _tcs.Task;
        }
        public async Task<UnityEngine.Object[]> LoadAllAssetsAsync()
        {
            await InnerLoadAllAssetsAsync();
            return _request.allAssets;
        }
        private Task<bool> InnerLoadAllAssetsAsync()
        {
            _tcs = new TaskCompletionSource<bool>();
            _request = _assetBundle.LoadAllAssetsAsync();
            return _tcs.Task;
        }
    }

    public class ResourcesAssetLoader : Looper
    {
        private ResourceRequest _request;
        private TaskCompletionSource<bool> _tcs;
        public override void Update()
        {
            if (IsDisposed())
                return;

            if (!_request.isDone)
                return;

            Detach();
            TaskCompletionSource<bool> tcs = _tcs;
            _tcs = null;
            tcs?.SetResult(true);
        }
        public async Task<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            await InnerLoadAssetAsync<T>(name);
            return _request.asset as T;
        }
        Task<bool> InnerLoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            _tcs = new TaskCompletionSource<bool>();
            _request = Resources.LoadAsync<T>(name);
            return _tcs.Task;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------
    public class AssetBundleInfo
    {
        public AssetBundle assetBundle;
        public int referencedCount;

        public AssetBundleInfo(UnityWebRequest webRequest)
        {
            assetBundle = (webRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            referencedCount = 1;
        }
    }

    public class AssetBundleLoader : Looper
    {
        public bool IsDone { get; private set; }
        private string _assetBundleName;
        private string[] _dependencies;
        private TaskCompletionSource<bool> _tcs;
        private Dictionary<string, UnityEngine.Object> _resourceCaches = new Dictionary<string, UnityEngine.Object>();
        public Task<bool> LoadAssetBundle(string bundleName)
        {
            if (_tcs == null)
            {
                if (string.IsNullOrEmpty(_assetBundleName))
                {
                    _assetBundleName = bundleName;
                    InnerLoadAssetBundle(_assetBundleName);
                }
                _tcs = new TaskCompletionSource<bool>();
            }
            return _tcs.Task;
        }

        public async Task<T> LoadAssetAsync<T>(string asset) where T : UnityEngine.Object
        {
            T obj = GetAsset<T>(asset);

            if (obj != null)
                return obj;

            if (_bundleInfoCaches.TryGetValue(_assetBundleName, out AssetBundleInfo assetBundleInfo))
            {
                using (AssetLoader request = new AssetLoader(assetBundleInfo.assetBundle))
                {
                    obj = await request.LoadAssetAsync<T>(asset);
                    _resourceCaches[asset] = obj;
                }
            }

            return obj;
        }
        public async Task<UnityEngine.Object[]> LoadAllAssetsAsync()
        {
            UnityEngine.Object[] objs = null;
            if (_bundleInfoCaches.TryGetValue(_assetBundleName, out AssetBundleInfo assetBundleInfo))
            {
                using (AssetLoader request = new AssetLoader(assetBundleInfo.assetBundle))
                {
                    objs = await request.LoadAllAssetsAsync();
                    if (objs != null)
                    {
                        for (int i = 0; i < objs.Length; i++)
                        {
                            _resourceCaches[objs[i].name] = objs[i];
                        }
                    }
                }
            }
            return objs;
        }

        public override void Update()
        {
            if (IsDisposed())
                return;

            if (IsDone)
                return;

            if (GetAssetBundleInfo(_assetBundleName) == null)
                return;

            Detach();
            TaskCompletionSource<bool> tcs = _tcs;
            _tcs = null;
            IsDone = true;
            tcs?.SetResult(true);
        }

        public override void Dispose()
        {
            base.Dispose();
            UnloadAssetBundle();
        }

        //------------------------------------------------------------------------------------------------------------------
        void UnloadDependencies()
        {
            if (_dependencies == null)
                return;

            for (int i = 0; i < _dependencies.Length; i++)
            {
                UnloadAssetBundleInternal(_dependencies[i]);
            }
        }

        void UnloadAssetBundleInternal(string assetBundleName)
        {
            AssetBundleInfo bundle = GetAssetBundleInfo(assetBundleName);
            if (bundle == null)
                return;

            if (--bundle.referencedCount == 0)
            {
                bundle.assetBundle.Unload(false);
                _bundleInfoCaches.Remove(assetBundleName);
            }
        }

        AssetBundleInfo GetAssetBundleInfo(string assetBundleName)
        {
            _bundleInfoCaches.TryGetValue(assetBundleName, out AssetBundleInfo bundleInfo);
            if (bundleInfo == null)
                return null;

            if (_dependencies == null)
                return bundleInfo;

            for (int i = 0; i < _dependencies.Length; i++)
            {
                _bundleInfoCaches.TryGetValue(_dependencies[i], out AssetBundleInfo dependentBundle);
                if (dependentBundle == null)
                    return null;
            }

            return bundleInfo;
        }

        void InnerLoadAssetBundle(string assetBundleName)
        {
            if (ResourcesManager.Instance.AssetBundleManifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest by calling ResourcesManager.Start");
                Detach();
                TaskCompletionSource<bool> tcs = _tcs;
                _tcs = null;
                _tcs?.SetResult(false);
                return;
            }
            LoadDependencies(assetBundleName);
            LoadAssetBundleInternal(assetBundleName);
        }

        void LoadDependencies(string assetBundleName)
        {
            _dependencies = ResourcesManager.Instance.AssetBundleManifest.GetAllDependencies(assetBundleName);
            for (int i = 0; i < _dependencies.Length; i++)
            {
                LoadAssetBundleInternal(_dependencies[i]);
            }
        }

        async void LoadAssetBundleInternal(string assetBundleName)
        {
            _bundleInfoCaches.TryGetValue(assetBundleName, out AssetBundleInfo bundleInfo);
            if (bundleInfo != null)
            {
                bundleInfo.referencedCount++;
                return;
            }

            string url = $"{_wwwPath}/{assetBundleName}";
            using (WWWRequest request = new WWWRequest())
            {
                try
                {
                    await request.LoadAsync(url);
                    _bundleInfoCaches.Add(assetBundleName, new AssetBundleInfo(request.WebRequest));
                }
                catch (Exception e)
                {
                    Debug.LogError($"资源加载失败:{e.ToString()}" + url);
                    Detach();
                    TaskCompletionSource<bool> tcs = _tcs;
                    _tcs = null;
                    _tcs?.SetResult(false);
                }
            }
        }

        T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
            if (_resourceCaches.TryGetValue(assetName, out UnityEngine.Object resource))
            {
                return resource as T;
            }
            return default;
        }

        void UnloadAssetBundle()
        {
            UnloadAssetBundleInternal(_assetBundleName);
            UnloadDependencies();
        }

        public static Dictionary<string, AssetBundleInfo> _bundleInfoCaches = new Dictionary<string, AssetBundleInfo>();

#if UNITY_ANDROID && !UNITY_EDITOR
        public static readonly string _wwwPath = "file://" + Application.persistentDataPath + "/" + LuaConst.osDir;
#else
        public static readonly string _wwwPath = Application.persistentDataPath + "/" + LuaConst.osDir;
#endif
    }
}