using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPartScriptWeaponBulletSaw : MonoBehaviour
{
    [SerializeField] [HeaderAttribute("特效")] GameObject _effect;
    [SerializeField] [HeaderAttribute("拖尾特效")] GameObject _effectTuoWei;
    public float _offsetHeight = 10; // 悬浮高度偏移
    public float _posx = 1; // 从中心点往左右偏移距离
    public float _posz = 1; // 从中心点往前后偏移距离
    public float _coefficient = 1; // 浮空系数
    public float _pow = 1; // 浮空曲线
    public float _moveForce = 3; // 移动加速度
    public float _forqueForce = 10000; // 旋转力
    public float _dragFalg = 0.25f; // 模拟的空气阻力
    public float _deleteTime = 3f; // 持续时间
    public float _stopTime = 3f; // 飞行停止时间
    public float _thrustForce = 4; // 推力系数
    public float _indexForce = 1; // 推力指数
    public float _maxDamageTime = 0.5f; //伤害检测间隔
    public float _scaleValue; // 技能模型放大倍数
    private float _curHeight = 0; // 当前悬浮高度
    private Rigidbody _rb;
    public float _curDeleteTime = 0;
    public float _curDamageTime = 0;
    private List<Vector3> _pos = new List<Vector3>();
    private BaseNpc _Owner;
    private RobotPartScriptWeaponSaw _thisPart;
    private float _curScaleValue;
    private float _tuoweiTime = 0.5f;
    private Vector3 _startScale;
    private bool _bActivate = false; // 是否被激活

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 100000;
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
        _pos.Add(transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz)));
    }

    void Update()
    {
        if (_curDeleteTime <= 0 || !gameObject.activeInHierarchy || !_bActivate) return;
        _tuoweiTime -= Time.deltaTime;
        if (_tuoweiTime < 0)
        {
            _effectTuoWei.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (!gameObject.activeInHierarchy || !_bActivate) return;

        _curDeleteTime -= Time.fixedDeltaTime;
        _curScaleValue -= Time.fixedDeltaTime;


        if (_curScaleValue > 0)
        {
            _rb.AddTorque(0, _forqueForce, 0, ForceMode.Acceleration);
            _rb.AddForce(transform.forward * _moveForce, ForceMode.Acceleration);
            transform.transform.localScale += _startScale * _scaleValue / _stopTime * Time.fixedDeltaTime;
            transform.transform.localScale = transform.transform.localScale.x > (_startScale.x * _scaleValue) ? _startScale * _scaleValue : transform.transform.localScale;
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY;
        }

        _pos[0] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz));
        _pos[1] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x - _posx, _rb.centerOfMass.y, _rb.centerOfMass.z - _posz));
        _pos[2] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x + _posx, _rb.centerOfMass.y, _rb.centerOfMass.z - _posz));
        _pos[3] = transform.TransformPoint(new Vector3(_rb.centerOfMass.x - _posx, _rb.centerOfMass.y, _rb.centerOfMass.z + _posz));

        float g = Physics.gravity.y * -1;
        float G = _rb.mass * g;
        float useForce = G / 4;
        _rb.AddForce(new Vector3(0, -_rb.velocity.y * _dragFalg, 0), ForceMode.Acceleration);

        foreach (Vector3 p in _pos)
        {
            float space = _curHeight - p.y;
            float addCoe = Mathf.Pow(Mathf.Abs(space) * _coefficient, _pow) * (space >= 0 ? 1 : -1);
            float addForce = useForce + addCoe;
            if (Mathf.Abs(addForce) < 999999999)
            {
                _rb.AddForceAtPosition(Vector3.up * addForce, p, ForceMode.Force);
            }
        }

        if (_curDeleteTime <= 0)
        {
            // 清除
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.eulerAngles = Vector3.zero;
            _curDamageTime = 0;
            _curDeleteTime = _deleteTime;
            _curScaleValue = _stopTime;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _effectTuoWei.SetActive(false);
            _bActivate = false;
            _rb.constraints = RigidbodyConstraints.None;
            MapManager.Instance.baseMap.objectPoolController.Recover(gameObject);
        }
    }
    // 碰撞开始
    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 point = Vector3.zero;
        Vector3 normal = Vector3.zero;
        List<ContactPoint> Contacts = new List<ContactPoint>();
        int ContactCount = collision.GetContacts(Contacts);
        foreach (ContactPoint item in Contacts)
        {
            point += item.point;
            normal += item.normal;
        }
        point /= ContactCount;
        normal /= ContactCount;

        if (!_effect.activeSelf)
        {
            _effect.SetActive(true);
        }
        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {
            float myMass = _Owner.gameObject.GetComponent<Rigidbody>().mass;
            if (_Owner.gameObject.GetComponent<Rigidbody>().mass > rigidbody.mass)
            {
                rigidbody.AddForceAtPosition(-normal * rigidbody.mass * Physics.gravity.magnitude * _thrustForce, point, ForceMode.Force);
            }
        }

        RobotPartScriptBase robotPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();
        if (robotPartScript)
        {
            int attack = _Owner.GetNpcAttr(AttributeType.Attack);
            _Owner.DamageOutput(_thisPart.myElement, robotPartScript.myElement, attack);
        }
        // 播放碰撞音效
        _thisPart.PlayPartSound(0, collision.GetContact(0).point);
    }
    // 碰撞持续中
    void OnCollisionStay(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        Vector3 point = Vector3.zero;
        Vector3 normal = Vector3.zero;
        List<ContactPoint> Contacts = new List<ContactPoint>();
        int ContactCount = collision.GetContacts(Contacts);
        foreach (ContactPoint item in Contacts)
        {
            point += item.point;
            normal += item.normal;
        }
        point /= ContactCount;
        normal /= ContactCount;

        if (!_effect.activeSelf)
        {
            _effect.SetActive(true);
        }
        Rigidbody rigidbody = collision.rigidbody;
        if (rigidbody)
        {
            float myMass = _Owner.gameObject.GetComponent<Rigidbody>().mass;
            if (_Owner.gameObject.GetComponent<Rigidbody>().mass > rigidbody.mass)
            {
                rigidbody.AddForceAtPosition(-normal * rigidbody.mass * 20 * _thrustForce, point, ForceMode.Force);
            }
        }

        RobotPartScriptBase robotPartScript = collision.collider.gameObject.GetComponentInParent<RobotPartScriptBase>();

        _curDamageTime += Time.fixedDeltaTime;
        if (_curDamageTime > _maxDamageTime && robotPartScript != null && robotPartScript.gameObject.layer != _Owner.gameObject.layer)
        {
            if (!robotPartScript.myElement.isDead)
            {
                _curDamageTime = 0;
                int attack = _Owner.GetNpcAttr(AttributeType.Attack);
                _Owner.DamageOutput(_thisPart.myElement, robotPartScript.myElement, attack);
            }
        }
    }

    // 碰撞离开
    void OnCollisionExit(Collision collision)
    {
        _effect.SetActive(false);
    }
    
    public void SetOwner(BaseNpc npc, RobotPartScriptWeaponSaw rs)
    {

        _Owner = npc;
        _thisPart = rs;

        foreach (var pChildTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            pChildTransform.gameObject.layer = _Owner.gameObject.layer;
        }
        _tuoweiTime = 0.5f;
        _rb = GetComponent<Rigidbody>();
        _curHeight = transform.TransformPoint(_rb.centerOfMass).y;
        _bActivate = true;
    }

    public void SetAttribute(float offsetHeight, float posx, float posz, float coefficient, float pow, float moveForce, float forqueForce, float dragFalg, float deleteTime, float thrustForce, float maxDamageTime, float stopTime, float scaleValue)
    {
        _offsetHeight = offsetHeight;
        _posx = posx;
        _posz = posz;
        _coefficient = coefficient;
        _pow = pow;
        _moveForce = moveForce;
        _forqueForce = forqueForce;
        _dragFalg = dragFalg;
        _deleteTime = _curDeleteTime = deleteTime;
        _thrustForce = thrustForce;
        _maxDamageTime = maxDamageTime;
        _stopTime = _curScaleValue = stopTime;
        _scaleValue = scaleValue;
        _startScale = transform.localScale;
    }
}
