using System;

namespace Framework
{
    public class Singleton<T> : Base where T : Singleton<T>
    {
        public override void Dispose()
        {
            base.Dispose();
            _instance = default(T);
        }

        //+---------------------------------------------------------
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return _instance;
            }
        }

        protected static T _instance;
        private static readonly object _lock = new object();
    }
}
