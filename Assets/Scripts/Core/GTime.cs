using System;
using UTime = UnityEngine.Time;

namespace Framework
{
    public static class GTime
    {
        public static float RealtimeSinceStartup { get { return UTime.realtimeSinceStartup; } }
        public static float DeltaTime { get { return UTime.deltaTime; } }
        public static float Time { get { return UTime.time; } }
        public static int RenderedFrameCount { get { return UTime.renderedFrameCount; } }
        public static int FrameCount { get { return UTime.frameCount; } }
        public static float TimeScale { get { return UTime.timeScale; } set { UTime.timeScale = value; } }
        public static float MaximumParticleDeltaTime { get { return UTime.maximumParticleDeltaTime; } set { UTime.maximumParticleDeltaTime = value; } }
        public static float SmoothDeltaTime { get { return UTime.smoothDeltaTime; } }
        public static float MaximumDeltaTime { get { return UTime.maximumDeltaTime; } set { UTime.maximumDeltaTime = value; } }
        public static int CaptureFramerate { get { return UTime.captureFramerate; } set { UTime.captureFramerate = value; } }
        public static float FixedDeltaTime { get { return UTime.fixedDeltaTime; } set { UTime.fixedDeltaTime = value; } }
        public static float UnscaledDeltaTime { get { return UTime.unscaledDeltaTime; } }
        public static float FixedUnscaledTime { get { return UTime.fixedUnscaledTime; } }
        public static float UnscaledTime { get { return UTime.unscaledTime; } }
        public static float FixedTime { get { return UTime.fixedTime; } }
        public static float TimeSinceLevelLoad { get { return UTime.timeSinceLevelLoad; } }
        public static float FixedUnscaledDeltaTime { get { return UTime.fixedUnscaledDeltaTime; } }
        public static bool InFixedTimeStep { get { return UTime.inFixedTimeStep; } }

        public static DateTime ServerTime { get { return DateTime.Now.AddSeconds(_diffTime); } }
        public static int ServerSeconds { get { TimeSpan span = ServerTime - DateTimeOrigin.ToLocalTime(); return (int)span.TotalSeconds; } }
        public static readonly DateTime DateTimeOrigin = new DateTime(1970, 1, 1);
        public static readonly DateTime MaxDateTime = DateTime.MaxValue;

        public static void SetServerTime(double serverTime, double milliTime)
        {
            _serverTime = serverTime;
            DateTime serverDateTime = DateTimeOrigin.AddSeconds(serverTime).ToLocalTime();
            _diffTime = (double)((serverDateTime - DateTime.Now).TotalSeconds);
        }

        //上线时服务器同步的时间戳
        static double _serverTime = 0;
        //服务器和客户端的误差时间
        static double _diffTime = 0;
    }
}
