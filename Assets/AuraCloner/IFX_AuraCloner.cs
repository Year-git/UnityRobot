using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_AuraCloner : IFX_ControlledObject
    {
        public float CloneLifeTime;
        public float CloneRate;
        public bool IsRandomOffsetEnabled;
        public float CloneOffset;
        public float RandomSphereSize;
        public Material CloneMaterial;

        public float CloneSpeed;
        public IFX_AnimationModule CloneOpacityAnimation;

        public bool ForceMakeClone;
        public Component[] ComponentsWithRequirements;

        private float _time;

        private GameObject _target;

        public GameObject myParentObj;

        public override void Run()
        {
            _time = 0;

            base.Run();

            if (ForceMakeClone)
            {
                MakeClone();
                return;
            }
        }

        public void MakeCloneOnce()
        {
            _time = 0;
            MakeClone();
        }

        private void MakeClone()
        {
            var targetPosition = _target.transform.position;

            var clone = Instantiate(_target, targetPosition, _target.transform.rotation);

            clone.SetActive(false);

            clone.transform.parent = this.myParentObj.transform;
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localRotation = Quaternion.identity;

            var idxInOrder = 0;
            bool needToUseOrder = ComponentsWithRequirements != null && ComponentsWithRequirements.Length > 0;

            foreach (var comp in clone.GetComponentsInChildren<Component>())
            {
                if (!needToUseOrder || comp.GetType() != ComponentsWithRequirements[idxInOrder].GetType())
                    continue;

                Destroy(comp);
                idxInOrder++;

                if (idxInOrder >= ComponentsWithRequirements.Length)
                    needToUseOrder = false;
            }

            List<Component> removeJoint = new List<Component>();
            List<Component> removeRigidbody = new List<Component>();

            foreach (var comp in clone.GetComponentsInChildren<Component>())
            {
                if (comp is Joint)
                {
                    removeJoint.Add(comp);
                    continue;
                }
                else if (comp is Rigidbody)
                {
                    removeRigidbody.Add(comp);
                    continue;
                }
                else if(comp is Transform)
                {
                    comp.gameObject.name += "IFX";
                }
                else if(comp is Collider) 
                {
                    ((Collider)comp).enabled = false;
                }

                if (!(comp is Renderer) && !(comp is Transform) && !(comp is MeshFilter))
                    Destroy(comp);
            }

            for(int i = removeJoint.Count - 1; i > -1; i--)
            {
                Destroy(removeJoint[i]);
            }
            for(int i = removeRigidbody.Count - 1; i > -1; i--)
            {
                Destroy(removeRigidbody[i]);
            }

            clone.SetActive(true);

            var cloneDestroyer = clone.AddComponent<IFX_SelfDestroyer>();
            cloneDestroyer.LifeTime = CloneLifeTime;
            cloneDestroyer.Run();

            IFX_MaterialUtil.RemoveAllMaterials(clone);
            IFX_MaterialUtil.AddMaterial(clone, CloneMaterial);

            var opacityAnimator = clone.AddComponent<IFX_ShaderAnimator>();
            opacityAnimator.DynamicShaderParameters = new[]
            {
                new IFX_DynamicShaderParameter
                {
                    AnimationModule = CloneOpacityAnimation,
                    ParameterName = "_TintColor",
                    Type = IFX_DynamicShaderParameter.ParameterType.ColorAlpha
                }
            };
            opacityAnimator.Setup();
            opacityAnimator.RunWithDelay();

            // var lerpMotion = clone.AddComponent<IFX_LerpMotion>();
            // lerpMotion.LaunchPosition = targetPosition;

            // if (!IsRandomOffsetEnabled)
            //     lerpMotion.TargetPosition = targetPosition + transform.forward * CloneOffset;
            // else
            // {
            //     var randomSphere = Random.insideUnitSphere * RandomSphereSize;
            //     lerpMotion.TargetPosition = targetPosition + randomSphere;
            // }

            // lerpMotion.ChangeRotation = false;
            // lerpMotion.Speed = CloneSpeed;
            // lerpMotion.Run();
        }

        private void Awake()
        {
            _target = gameObject;
        }

        private void Update()
        {
            if (!IsRunning || ForceMakeClone)
                return;

            _time += Time.deltaTime;

            if (_time >= CloneRate)
            {
                MakeClone();
                _time = 0;
            }
        }
    }
}