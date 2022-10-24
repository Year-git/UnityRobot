using System;
using System.Threading.Tasks;

namespace Framework
{
    public class Timer : Looper
    {
        bool _loop { get; set; }
        float _time { get; set; }
        float _deadline { get; set; }

        public virtual void Handler() { }

        public override void Update()
        {
            float timeNow = GTime.RealtimeSinceStartup;
            if (timeNow >= _deadline)
            {
                if (_loop)
                    _deadline = timeNow + _time;
                else
                    Stop();

                Handler();
            }
        }

        public void Start()
        {
            _deadline = GTime.RealtimeSinceStartup + _time;
            Attach();
        }

        public void Stop()
        {
            Dispose();
        }

        static T CreateTimer<T>(float time, bool loop) where T : Timer
        {
            T timer = Activator.CreateInstance<T>();
            timer._loop = loop;
            timer._time = time;
            timer._deadline = GTime.RealtimeSinceStartup + timer._time;
            return timer;
        }

        public static Timer CreateTimer(float time, bool loop, Action func)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_0 timer = CreateTimer<Timer_0>(time, loop);
            timer.SetFuncAndParam(func);

            return timer;
        }

        public static Timer CreateTimer<T1>(float time, bool loop, Action<T1> func, T1 arg1)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_1<T1> timer = CreateTimer<Timer_1<T1>>(time, loop);
            timer.SetFuncAndParam(func, arg1);

            return timer;
        }

        public static Timer CreateTimer<T1, T2>(float time, bool loop, Action<T1, T2> func, T1 arg1, T2 arg2)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_2<T1, T2> timer = CreateTimer<Timer_2<T1, T2>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3>(float time, bool loop, Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_3<T1, T2, T3> timer = CreateTimer<Timer_3<T1, T2, T3>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3, T4>(float time, bool loop, Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_4<T1, T2, T3, T4> timer = CreateTimer<Timer_4<T1, T2, T3, T4>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3, arg4);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3, T4, T5>(float time, bool loop, Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_5<T1, T2, T3, T4, T5> timer = CreateTimer<Timer_5<T1, T2, T3, T4, T5>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3, arg4, arg5);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3, T4, T5, T6>(float time, bool loop, Action<T1, T2, T3, T4, T5, T6> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_6<T1, T2, T3, T4, T5, T6> timer = CreateTimer<Timer_6<T1, T2, T3, T4, T5, T6>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3, arg4, arg5, arg6);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3, T4, T5, T6, T7>(float time, bool loop, Action<T1, T2, T3, T4, T5, T6, T7> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_7<T1, T2, T3, T4, T5, T6, T7> timer = CreateTimer<Timer_7<T1, T2, T3, T4, T5, T6, T7>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3, T4, T5, T6, T7, T8>(float time, bool loop, Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_8<T1, T2, T3, T4, T5, T6, T7, T8> timer = CreateTimer<Timer_8<T1, T2, T3, T4, T5, T6, T7, T8>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

            return timer;
        }

        public static Timer CreateTimer<T1, T2, T3, T4, T5, T6, T7, T8, T9>(float time, bool loop, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            if (func == null)
                throw new Exception("Timer.CreateTimer,  func is null");

            Timer_9<T1, T2, T3, T4, T5, T6, T7, T8, T9> timer = CreateTimer<Timer_9<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(time, loop);
            timer.SetFuncAndParam(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

            return timer;
        }

        public static void Stop(Timer timer)
        {
            timer?.Stop();
        }

        public static Task<bool> Wait(float time)
        {
            WaitTimer timer = CreateTimer<WaitTimer>(time, false);
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            timer.SetTaskCompletionSource(tcs);

            return tcs.Task;
        }
    }

    //+-------------------------------------------------------------------------------------------------------------------------------------------------------------
    class Timer_0 : Timer
    {
        Action _func;

        public void SetFuncAndParam(Action func)
        {
            _func = func;
        }

        public override void Handler()
        {
            _func?.Invoke();
        }
    }

    class Timer_1<T1> : Timer
    {
        T1 _arg1;
        Action<T1> _func;

        public void SetFuncAndParam(Action<T1> func, T1 arg1)
        {
            _func = func;
            _arg1 = arg1;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1);
        }
    }

    class Timer_2<T1, T2> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        Action<T1, T2> _func;

        public void SetFuncAndParam(Action<T1, T2> func, T1 arg1, T2 arg2)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2);
        }
    }

    class Timer_3<T1, T2, T3> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        Action<T1, T2, T3> _func;

        public void SetFuncAndParam(Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3);
        }
    }

    class Timer_4<T1, T2, T3, T4> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        T4 _arg4;
        Action<T1, T2, T3, T4> _func;

        public void SetFuncAndParam(Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3, _arg4);
        }
    }

    class Timer_5<T1, T2, T3, T4, T5> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        T4 _arg4;
        T5 _arg5;
        Action<T1, T2, T3, T4, T5> _func;

        public void SetFuncAndParam(Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5);
        }
    }

    class Timer_6<T1, T2, T3, T4, T5, T6> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        T4 _arg4;
        T5 _arg5;
        T6 _arg6;
        Action<T1, T2, T3, T4, T5, T6> _func;

        public void SetFuncAndParam(Action<T1, T2, T3, T4, T5, T6> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6);
        }
    }

    class Timer_7<T1, T2, T3, T4, T5, T6, T7> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        T4 _arg4;
        T5 _arg5;
        T6 _arg6;
        T7 _arg7;
        Action<T1, T2, T3, T4, T5, T6, T7> _func;

        public void SetFuncAndParam(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7);
        }
    }

    class Timer_8<T1, T2, T3, T4, T5, T6, T7, T8> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        T4 _arg4;
        T5 _arg5;
        T6 _arg6;
        T7 _arg7;
        T8 _arg8;
        Action<T1, T2, T3, T4, T5, T6, T7, T8> _func;

        public void SetFuncAndParam(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
            _arg8 = arg8;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8);
        }
    }

    class Timer_9<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Timer
    {
        T1 _arg1;
        T2 _arg2;
        T3 _arg3;
        T4 _arg4;
        T5 _arg5;
        T6 _arg6;
        T7 _arg7;
        T8 _arg8;
        T9 _arg9;
        Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> _func;

        public void SetFuncAndParam(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            _func = func;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
            _arg8 = arg8;
            _arg9 = arg9;
        }

        public override void Handler()
        {
            _func?.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8, _arg9);
        }
    }

    class WaitTimer : Timer
    {
        private TaskCompletionSource<bool> _tcs;

        public void SetTaskCompletionSource(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        public override void Handler()
        {
            _tcs?.SetResult(true);
        }
    }
}
