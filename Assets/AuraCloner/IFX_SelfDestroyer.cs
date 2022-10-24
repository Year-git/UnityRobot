// ReSharper disable once CheckNamespace
using UnityEngine;

namespace QFX.IFX
{
    public class IFX_SelfDestroyer : IFX_ControlledObject
    {
        public float LifeTime;

        public override void Run()
        {
            base.Run();
            Destroy(gameObject, LifeTime);
        }

        public void Update()
        {
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
        }
    }
}