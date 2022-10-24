using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectExplosionScript : MonoBehaviour
{
    public int _intervalSpawnTime;
    private float _nextSpawnTime = 0f;
    public GameObject _effectPrefabExplosion;
    public int _effectPlayTime;
    private float _effectStopTime = 0f;
    private GameObject _effectExplosion;
    private Vector3 _position;
    private Quaternion _rotation;
    private Rigidbody _myRigidbody;
    private Collider _myCollider;
    private MeshRenderer _myMeshRender;
    public float _explosionRadius;
    public float _explosionForceRatio;
    public int _explosionDamage;

    private PlaySoundScript[] _playSoundScriptList;

    private void Start()
    {
        _playSoundScriptList = GetComponents<PlaySoundScript>();

        _myRigidbody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<Collider>();
        _myMeshRender = GetComponent<MeshRenderer>();

        _position = transform.position;
        _rotation = transform.rotation;
        _effectExplosion = Instantiate<GameObject>(_effectPrefabExplosion, transform.position, transform.rotation);
        _effectExplosion.SetActive(false);
    }

    private void Update()
    {
        float nCurTime = Framework.GTime.RealtimeSinceStartup;

        if (_effectStopTime != 0f && nCurTime >= _effectStopTime)
        {
            _effectStopTime = 0f;
            _effectExplosion.SetActive(false);
        }

        if (_nextSpawnTime > 0f && nCurTime >= _nextSpawnTime)
        {
            _nextSpawnTime = 0f;

            _myRigidbody.velocity = Vector3.zero;
            _myRigidbody.angularVelocity = Vector3.zero;

            transform.position = _position;
            transform.rotation = _rotation;

            _myCollider.enabled = true;
            _myMeshRender.enabled = true;

            _effectExplosion.SetActive(false);
        }
    }

    // 碰撞开始
    public void OnCollisionEnter(Collision collision)
    {
        if (_nextSpawnTime != 0)
        {
            return;
        }

        RobotPartScriptBase pPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (pPartScript == null)
        {
            return;
        }

        if (_effectPrefabExplosion == null)
        {
            return;
        }

        float nSquaredLength = collision.relativeVelocity.sqrMagnitude;
        // float nLength = collision.relativeVelocity.magnitude;
        if (nSquaredLength > 3)
        {
            _effectExplosion.transform.position = transform.position;
            _effectExplosion.transform.rotation = Quaternion.identity;
            _effectExplosion.SetActive(true);

            Explosion();

            if (_playSoundScriptList.Length > 0)
            {
                // 播放爆炸音效
                _playSoundScriptList[0].Play(transform.position);
            }

            _myCollider.enabled = false;
            _myMeshRender.enabled = false;

            float nCurTime = Framework.GTime.RealtimeSinceStartup;
            _effectStopTime = (float)nCurTime + _effectPlayTime / 1000f;
            _nextSpawnTime = nCurTime + _intervalSpawnTime / 1000f;
        }
    }

    public void Explosion()
    {
        // float _radius = 15f;
        // float f = 10f;

        List<RobotPartScriptBase> listRobotPartScript = new List<RobotPartScriptBase>();

        //定义爆炸位置为炸弹位置
        Vector3 explosionPos = transform.position;
        //这个方法用来反回球型半径之内（包括半径）的所有碰撞体collider[]
        Collider[] colliders = Physics.OverlapSphere(explosionPos, _explosionRadius);

        List<Collider> colliderList = new List<Collider>();
        colliderList.CopyTo(colliders);

        Dictionary<int, Rigidbody> rigList = new Dictionary<int, Rigidbody>();
        //遍历返回的碰撞体，如果是刚体，则给刚体添加力
        foreach (Collider hit in colliders)
        {
            Rigidbody rig = hit.attachedRigidbody;
            RobotPartScriptBase robotPartScript = hit.gameObject.GetComponentInParent<RobotPartScriptBase>();
            if (robotPartScript != null)
            {
                listRobotPartScript.Add(robotPartScript);
            }

            if(rig != null){
                bool bIgnoreLayerCollision = Physics.GetIgnoreLayerCollision(this.gameObject.layer, rig.gameObject.layer);
                if (!bIgnoreLayerCollision)
                {
                    if (!rigList.ContainsKey(rig.GetInstanceID()))
                    {
                        rigList.Add(rig.GetInstanceID(), rig);
                    }
                }
            }
        }

        foreach (var keyVal in rigList)
        {
            Rigidbody rig = keyVal.Value;
            float a = Vector3.Distance(rig.gameObject.transform.position,explosionPos);
            float b = 1 - (a / _explosionRadius);
            float c = rig.mass * b;
            float d = c * _explosionForceRatio;
            rig.AddExplosionForce(d, explosionPos, _explosionRadius, 0.30f, ForceMode.Impulse);
        }

        BaseMap pMap = MapManager.Instance.baseMap;
        foreach(var robotPartScript in listRobotPartScript)
        {
            RobotPartElement pElement = robotPartScript.myElement;
            if (pElement != null)
            {
                BaseNpc pNpc = pMap.GetNpc(robotPartScript.myElement.myRobotPart.npcInstId);
                if (pNpc != null)
                {
                    pNpc.NpcDamageInput(pElement, _explosionDamage);
                }
            }
        }
    }
}
