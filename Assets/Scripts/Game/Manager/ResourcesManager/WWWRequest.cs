using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public class WWWRequest : Looper
    {
        public UnityWebRequest WebRequest { get; private set; }
        private bool _isCancel;
        private TaskCompletionSource<bool> _tcs;

        public float DownloadProgress
        {
            get
            {
                if (WebRequest == null)
                {
                    return 0;
                }
                return WebRequest.downloadProgress;
            }
        }

        public ulong DownloadedBytes
        {
            get
            {
                if (WebRequest == null)
                {
                    return 0;
                }
                return WebRequest.downloadedBytes;
            }
        }

        public override void Update()
        {
            if (_isCancel)
            {
                _tcs.SetResult(false);
                return;
            }

            if (WebRequest == null || !WebRequest.isDone)
            {
                return;
            }

            if (!string.IsNullOrEmpty(WebRequest.error))
            {
                Detach();
                _tcs.SetException(new Exception($"WWWRequest error: {WebRequest.error}"));
                return;
            }

            Detach();
            TaskCompletionSource<bool> tcs = _tcs;
            _tcs = null;
            tcs?.SetResult(true);
        }

        public Task<bool> LoadFromCacheOrDownload(string url, Hash128 hash)
        {
            if (_tcs == null)
            {
                if (WebRequest == null)
                {
                    WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, 0);
                    WebRequest.SendWebRequest();
                }
                _tcs = new TaskCompletionSource<bool>();
                Attach();
            }
            return _tcs.Task;
        }

        public Task<bool> LoadFromCacheOrDownload(string url, Hash128 hash, CancellationToken cancellationToken)
        {
            if (_tcs == null)
            {
                if (WebRequest == null)
                {
                    WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, 0);
                    WebRequest.SendWebRequest();
                }
                _tcs = new TaskCompletionSource<bool>();
                Attach();
            }
            cancellationToken.Register(() => { _isCancel = true; });
            return _tcs.Task;
        }

        public Task<bool> LoadAsync(string url)
        {
            if (_tcs == null)
            {
                if (WebRequest == null)
                {
                    WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    WebRequest.SendWebRequest();
                }
                _tcs = new TaskCompletionSource<bool>();
                Attach();
            }
            return _tcs.Task;
        }

        public Task<bool> LoadAsync(string url, CancellationToken cancellationToken)
        {
            if (_tcs == null)
            {
                if (WebRequest == null)
                {
                    WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    WebRequest.SendWebRequest();
                }
                _tcs = new TaskCompletionSource<bool>();
                Attach();
            }
            cancellationToken.Register(() => { _isCancel = true; });
            return _tcs.Task;
        }

        public override void Dispose()
        {
            base.Dispose();
            WebRequest?.Dispose();
            WebRequest = null;
        }
    }
}