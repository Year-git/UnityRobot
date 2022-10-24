using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    interface IEvent { }
    class Event_0 : IEvent { internal Action Listen; }
    class Event_1<T1> : IEvent { internal Action<T1> Listen; }
    class Event_2<T1, T2> : IEvent { internal Action<T1, T2> Listen; }
    class Event_3<T1, T2, T3> : IEvent { internal Action<T1, T2, T3> Listen; }
    class Event_4<T1, T2, T3, T4> : IEvent { internal Action<T1, T2, T3, T4> Listen; }
    class Event_5<T1, T2, T3, T4, T5> : IEvent { internal Action<T1, T2, T3, T4, T5> Listen; }
    class Event_6<T1, T2, T3, T4, T5, T6> : IEvent { internal Action<T1, T2, T3, T4, T5, T6> Listen; }
    class Event_7<T1, T2, T3, T4, T5, T6, T7> : IEvent { internal Action<T1, T2, T3, T4, T5, T6, T7> Listen; }
    class Event_8<T1, T2, T3, T4, T5, T6, T7, T8> : IEvent { internal Action<T1, T2, T3, T4, T5, T6, T7, T8> Listen; }
    class Event_9<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IEvent { internal Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Listen; }

    public sealed class GEvent : IEvent
    {
        public static void DispatchEvent(string type)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_0).Listen();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1>(string type, T1 arg1)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_1<T1>).Listen(arg1);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2>(string type, T1 arg1, T2 arg2)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_2<T1, T2>).Listen(arg1, arg2);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3>(string type, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_3<T1, T2, T3>).Listen(arg1, arg2, arg3);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_4<T1, T2, T3, T4>).Listen(arg1, arg2, arg3, arg4);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4, T5>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_5<T1, T2, T3, T4, T5>).Listen(arg1, arg2, arg3, arg4, arg5);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4, T5, T6>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_6<T1, T2, T3, T4, T5, T6>).Listen(arg1, arg2, arg3, arg4, arg5, arg6);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4, T5, T6, T7>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_7<T1, T2, T3, T4, T5, T6, T7>).Listen(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4, T5, T6, T7, T8>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_8<T1, T2, T3, T4, T5, T6, T7, T8>).Listen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string type, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                try
                {
                    (ev as Event_9<T1, T2, T3, T4, T5, T6, T7, T8, T9>).Listen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public static void RegistEvent(string type, Action func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_0>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_0).Listen += func;
        }

        public static void RegistEvent<T1>(string type, Action<T1> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_1<T1>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_1<T1>).Listen += func;
        }

        public static void RegistEvent<T1, T2>(string type, Action<T1, T2> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_2<T1, T2>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_2<T1, T2>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3>(string type, Action<T1, T2, T3> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_3<T1, T2, T3>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_3<T1, T2, T3>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3, T4>(string type, Action<T1, T2, T3, T4> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_4<T1, T2, T3, T4>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_4<T1, T2, T3, T4>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3, T4, T5>(string type, Action<T1, T2, T3, T4, T5> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_5<T1, T2, T3, T4, T5>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_5<T1, T2, T3, T4, T5>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3, T4, T5, T6>(string type, Action<T1, T2, T3, T4, T5, T6> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_6<T1, T2, T3, T4, T5, T6>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_6<T1, T2, T3, T4, T5, T6>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3, T4, T5, T6, T7>(string type, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_7<T1, T2, T3, T4, T5, T6, T7>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_7<T1, T2, T3, T4, T5, T6, T7>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3, T4, T5, T6, T7, T8>(string type, Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_8<T1, T2, T3, T4, T5, T6, T7, T8>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_8<T1, T2, T3, T4, T5, T6, T7, T8>).Listen += func;
        }

        public static void RegistEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string type, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
        {
            if (!_eventDic.TryGetValue(type, out IEvent ev))
            {
                ev = Activator.CreateInstance<Event_9<T1, T2, T3, T4, T5, T6, T7, T8, T9>>();
                _eventDic.Add(type, ev);
            }
            (ev as Event_9<T1, T2, T3, T4, T5, T6, T7, T8, T9>).Listen += func;
        }

        public static void RemoveEvent(string type, Action func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_0).Listen -= func;
            }
        }

        public static void RemoveEvent<T1>(string type, Action<T1> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_1<T1>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2>(string type, Action<T1, T2> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_2<T1, T2>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3>(string type, Action<T1, T2, T3> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_3<T1, T2, T3>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3, T4>(string type, Action<T1, T2, T3, T4> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_4<T1, T2, T3, T4>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3, T4, T5>(string type, Action<T1, T2, T3, T4, T5> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_5<T1, T2, T3, T4, T5>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3, T4, T5, T6>(string type, Action<T1, T2, T3, T4, T5, T6> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_6<T1, T2, T3, T4, T5, T6>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3, T4, T5, T6, T7>(string type, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_7<T1, T2, T3, T4, T5, T6, T7>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3, T4, T5, T6, T7, T8>(string type, Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_8<T1, T2, T3, T4, T5, T6, T7, T8>).Listen -= func;
            }
        }

        public static void RemoveEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string type, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
        {
            if (_eventDic.TryGetValue(type, out IEvent ev))
            {
                (ev as Event_9<T1, T2, T3, T4, T5, T6, T7, T8, T9>).Listen -= func;
            }
        }

        static Dictionary<string, IEvent> _eventDic = new Dictionary<string, IEvent>();
    }
}