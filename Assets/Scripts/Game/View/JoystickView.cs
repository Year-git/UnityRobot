using UnityEngine;

/// <summary>
/// 虚拟摇杆
/// </summary>
public class JoystickView : MonoBehaviour
{
    public ETCJoystick joystick;
    private Vector2 _joystickPos;
    private Transform _mainCamera;
    private Transform _player;
    private Model _npcModel;
    private Vector3 _offset;
    private readonly float _rotateSpeed = 5;
    private Transform _npc;

    void Awake()
    {
        _joystickPos = joystick.transform.localPosition;
        EventTriggerListener.Get(joystick.gameObject).onDown = OnDown;
        EventTriggerListener.Get(joystick.gameObject).onUp = OnUp;
    }

    //----------temp----------
    private Transform Load(string name)
    {
        GameObject obj = Resources.Load(name) as GameObject;
        GameObject go = Instantiate(obj) as GameObject;
        go.name = name;
        go.transform.localScale = Vector3.one;
        DontDestroyOnLoad(go);
        return go.transform;
    }
    //----------temp----------

    void Start()
    {
        //----------temp----------
        _npcModel = MyPlayer.player.myModel;
        _player = _npcModel.transform;
        _mainCamera = Load("Prefabs/MainCamera");
        _offset = _mainCamera.position - _player.position;
        _npc = Load("Models/Npc");
        //----------temp----------

        joystick.onMoveStart.AddListener(OnMoveStart);
        joystick.onMove.AddListener(OnMoving);
        joystick.onMoveEnd.AddListener(OnMoveEnd);
    }

    void Update()
    {
        _mainCamera.position = _player.position + _offset;
        RotateCamera();
    }

    void OnDown(GameObject go)
    {
        //Canvas canvas = ViewManager.canvas;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
        // if (Vector2.Distance(_joystickPos, localPoint) > joystick.thumb.rect.width / 2)
        // {
        //     joystick.transform.localPosition = localPoint;
        // }
    }

    void OnUp(GameObject go)
    {
        joystick.transform.localPosition = _joystickPos;
    }

    void OnMoveStart()
    {

    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            //左右
            _mainCamera.RotateAround(_player.position, _player.up, Input.GetAxis("Mouse X") * _rotateSpeed);
            _offset = _mainCamera.position - _player.position;
            _mainCamera.position = _player.position + _offset;
            //上下
            _mainCamera.RotateAround(_player.position, _mainCamera.right, -Input.GetAxis("Mouse Y") * _rotateSpeed);
            _offset = _mainCamera.position - _player.position;
            _mainCamera.position = _player.position + _offset;
        }
    }

    void OnMoving(Vector2 pos)
    {
        // _npcModel.animator.ChangeNpcAction(NpcAction.walk, true);
        // Vector3 targetDirection = new Vector3(pos.x, 0, pos.y);
        // targetDirection = Quaternion.Euler(0, _mainCamera.rotation.eulerAngles.y, 0) * targetDirection;
        // _player.LookAt(targetDirection + _player.position);
        // _player.Translate(Vector3.forward * Time.deltaTime * 2f);
    }

    void OnMoveEnd()
    {
        // _npcModel.animator.ChangeNpcAction(NpcAction.walk, false);
    }
}