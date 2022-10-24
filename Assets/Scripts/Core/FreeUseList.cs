using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FreeUseList<T> where T : new()
    {
        private Dictionary<int,T> _useList = new Dictionary<int,T>();
        private List<T> _freeList = new List<T>();
        private int UseId = 0;

        public FreeUseList(){}
        
        public Dictionary<int,List<T>> AccessUseList()
        {
            return new Dictionary<int,List<T>>();
        }

        public KeyValuePair<int, T> Assign(Action<T> fDispose, params object[] args)
        {
            T element;
            if (_freeList.Count > 0)
            {
                element = _freeList[0];
                _freeList.RemoveAt(0);
                fDispose?.Invoke(element);
            }
            else{
                element = (T)Activator.CreateInstance(typeof(T), args);
            }

            int nUseId = UseId;
            _useList.Add(nUseId,element);
            UseId++;
            return new KeyValuePair<int, T>(nUseId,element);
        }

        public T Recover(int nUseId, Action<T> fDispose = null)
        {
            if (!_useList.ContainsKey(nUseId))
            {
                return default(T);
            }

            var element = _useList[nUseId];

            _useList.Remove(nUseId);
            if (fDispose != null)
            {
                fDispose.Invoke(element);
            }
            _freeList.Add(element);
            return element;
        }

        public void RecoverAll(Action<T> fDispose = null)
        {
            foreach(var kvPair in _useList)
            {
                if (fDispose != null)
                {
                    fDispose.Invoke(kvPair.Value);
                }
                _freeList.Add(kvPair.Value);
            }
            _useList.Clear();
        }

        public void Clear()
        {
            _useList.Clear();
            _freeList.Clear();
        }

        public T GetUseItem(int nIdx)
        {
            return _useList[nIdx];
        }
    }
}