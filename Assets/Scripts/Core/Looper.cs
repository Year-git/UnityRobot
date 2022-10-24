namespace Framework
{
    public class Looper : DoubleLinkNode<Looper>
    {
        public Looper()
        {
            Attach();
        }

        public Looper(bool attach)
        {
            if (attach)
                Attach();
        }

        public void Attach()
        {
            Attach(_link);
        }

        public virtual void Update() { }

        //+---------------------------------------------------------
        public static void TryUpdate()
        {
            Looper looper = null;
            while (_link.Next(ref looper))
            {
                if (looper.IsDisposed())
                    continue;
                looper.Update();
            }
        }

        static DoubleLink<Looper> _link = new DoubleLink<Looper>();
    }
}
