namespace Framework
{
    public class DoubleLinkNode<T> : Base where T : DoubleLinkNode<T>
    {
        internal T _pre;
        internal T _next;
        internal DoubleLink<T> _dl;
        internal bool IsAttach { get { return _dl != null; } }

        public void Attach(DoubleLink<T> dl)
        {
            dl?.PushBack(this as T);
        }

        public void Detach()
        {
            if (_dl != null)
                _dl.Remove(this);
            _dl = null;
        }

        public T Pre()
        {
            return _pre;
        }

        public T Next()
        {
            return _next;
        }

        public override void Dispose()
        {
            if (IsDisposed())
                return;
            Detach();
        }
    }

    //+-------------------------------------------------------------------------------------------------------------------------------------------------------------

    public class DoubleLink<T> where T : DoubleLinkNode<T>
    {
        DoubleLinkNode<T> _root = new DoubleLinkNode<T>();
        DoubleLinkNode<T> _bridge = new DoubleLinkNode<T>();

        public bool IsEmpty()
        {
            return _root._next == null;
        }

        public bool Next(ref T node)
        {
            node = (node == null) ? _root._next : (((node.IsAttach) ? node._next : _bridge._next));
            bool ret = node != null;
            if (ret)
            {
                _bridge._pre = node._pre;
                _bridge._next = node._next;
            }
            return ret;
        }

        public bool Pre(ref T node)
        {
            node = (node == null) ? _root._pre : (((node.IsAttach) ? node._pre : _bridge._pre));
            bool ret = node != null;
            if (ret)
            {
                _bridge._pre = node._pre;
                _bridge._next = node._next;
            }
            return ret;
        }

        public T GetHead()
        {
            return _root._next;
        }

        public T GetTail()
        {
            return _root._pre;
        }

        public void PushBack(T node)
        {
            node.Detach();
            node._dl = this;
            DoubleLinkNode<T> pre = _root._pre ?? _root;
            _root._pre = node;
            Link(pre, node);
        }

        public void PushFront(T node)
        {
            node.Detach();
            node._dl = this;
            DoubleLinkNode<T> next = _root._next ?? _root;
            _root._next = node;
            Link(node, next);
        }

        public void Remove(DoubleLinkNode<T> node)
        {
            if (node == null || !node.IsAttach)
                return;

            DoubleLinkNode<T> pre = node._pre ?? _root;
            DoubleLinkNode<T> next = node._next ?? _root;
            Link(pre, next);
            node._dl = null;
        }

        public void Clear()
        {
            T node = null;
            while (Next(ref node))
            {
                Remove(node);
            }
        }

        void Link(DoubleLinkNode<T> pre, DoubleLinkNode<T> next)
        {
            pre._next = next as T;
            next._pre = pre as T;
        }
    }
}