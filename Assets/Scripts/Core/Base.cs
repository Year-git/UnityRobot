using System;

namespace Framework
{
    public abstract class Base : IDisposable
    {
        int _id = 0;
        static int _defaultID = 1000000;
        public Base()
        {
            _id = (_defaultID += 1);
        }

        public virtual void Dispose()
        {
            _id = 0;
        }

        public virtual bool IsDisposed()
        {
            return _id == 0;
        }
    }
}