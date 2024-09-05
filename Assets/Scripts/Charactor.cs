using System.ComponentModel;
using UnityEngine;

public class Charactor : MonoBehaviour
{
    InventoryModule _inventory;    
    ToolModule _tool;

    bool IsBuildMode
    {
        get 
        {
            return _tool.IsBuildMode;
        }
    }

    Rigidbody _rigidbody;
    UnityEngine.Vector3 _moveVector;
    UnityEngine.Vector3 _lookPos;
    float _speed;
    float _interactTime = 0;

    bool _inventoryUIActive = false;

    [Range(1, 50)][SerializeField] float moveSpeed = 10;
    [Range(1, 50)][SerializeField] float interactionRange = 10;
    [Range(1, 100)][SerializeField] float interactionSpeed = 1;

    LineRenderer _lineRenderer;
    IInteractable onInteractObject;
    IInteractable onMouseObjectTemp;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _inventory = GetComponent<InventoryModule>();
        _tool = GetComponent<ToolModule>();

        Player.Instance.PropertyChanged += OnPropertyChanged;
        Register_OnStart();
    }

    void Register_OnStart()
    {
        Player.Instance.Register_KeyAction(KeyCode.Alpha1, () => _tool.ToolSelect_OnNumKeyClick(0));
        Player.Instance.Register_KeyAction(KeyCode.Alpha2, () => _tool.ToolSelect_OnNumKeyClick(1));
        Player.Instance.Register_KeyAction(KeyCode.Alpha3, () => _tool.ToolSelect_OnNumKeyClick(2));
        Player.Instance.Register_KeyAction(KeyCode.Alpha4, () => _tool.ToolSelect_OnNumKeyClick(3));
        Player.Instance.Register_KeyAction(KeyCode.Alpha5, () => _tool.ToolSelect_OnNumKeyClick(4));
        Player.Instance.Register_KeyAction(KeyCode.Alpha6, () => _tool.ToolSelect_OnNumKeyClick(5));
        Player.Instance.Register_KeyAction(KeyCode.Alpha7, () => _tool.ToolSelect_OnNumKeyClick(6));
        Player.Instance.Register_KeyAction(KeyCode.Alpha8, () => _tool.ToolSelect_OnNumKeyClick(7));
        Player.Instance.Register_KeyAction(KeyCode.Alpha9, () => _tool.ToolSelect_OnNumKeyClick(8));
        Player.Instance.Register_KeyAction(KeyCode.Alpha0, () => _tool.ToolSelect_OnNumKeyClick(9));
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Player.Instance.LookTargetPosVector):
                _lookPos = Player.Instance.LookTargetPosVector;
                //GridDraw_OnMouseMove();
                break;
            case nameof(Player.Instance.InputVector_Move):
                _moveVector = new UnityEngine.Vector3(Player.Instance.InputVector_Move.x, 0, Player.Instance.InputVector_Move.y);
                break;
            case nameof(Player.Instance.OnMouseObjectTemp):
                onMouseObjectTemp = Player.Instance.OnMouseObjectTemp;
                break;
        }
    }

    private void FixedUpdate()
    {
        Move_OnFixedUpdate();
        PointLook_OnFixedUpdate();        
    }

    private void Update()
    {
        if (onInteractObject != null)
        {
            DrawBeam(onInteractObject.GetPos());
        }
        else
        {
            RemoveBeam();
        }

        GridDraw_OnMouseMove();
    }

    void GridDraw_OnMouseMove()
    {
        if (IsBuildMode)
        {
            Vector3Int hitPoint_Grid = _lookPos.ToIntVector();
            GridRenderer.Instance.DrawGrid(hitPoint_Grid);
            BeltManager.Instance.OnMove(hitPoint_Grid);
        }
        else
        {
            GridRenderer.Instance.Command_HideAllGridLines();
            BeltManager.Instance.DeActive();
        }
    }

    public void Select_OnMouseLeftClick()
    {
        if (IsBuildMode)
        {
            Vector3Int hitPoint_Grid = _lookPos.ToIntVector();
            BeltManager.Instance.OnClick(hitPoint_Grid);
        }
        else
        {
            onMouseObjectTemp?.Select();
        }

    }

    public void TryInteract()
    {        
        if(onMouseObjectTemp != null && onMouseObjectTemp.TryInteract(this.transform.position, interactionRange))
        {
            onInteractObject = onMouseObjectTemp;
            _interactTime += Time.deltaTime;            
        }
        else
        {
            EndInteract();
        }
    }
    public void EndInteract()
    {
        onInteractObject = null;
        _interactTime = 0;
    }
    public void InventoryToggle()
    {
        _inventoryUIActive = !_inventoryUIActive;
        if (_inventoryUIActive)
        {
            _inventory.Active_Wdw();
        }       
        else
        {
            _inventory.Close_Wdw();
        }
    }

    void Move_OnFixedUpdate()
    {        
        if (_moveVector != UnityEngine.Vector3.zero)
        {
            _speed = _rigidbody.velocity.magnitude;
            if (_speed < moveSpeed)
            {
                if(onInteractObject == null)
                {
                    _rigidbody.AddForce(_moveVector * moveSpeed * 3, ForceMode.Force);
                }                
                else
                {
                    _rigidbody.AddForce(_moveVector * moveSpeed, ForceMode.Force);
                }
            }
        }
    }

    void PointLook_OnFixedUpdate()
    {
        if(_lookPos != UnityEngine.Vector3.zero)
        {
            this.transform.LookAt(_lookPos);
        }
        else
        {
            this.transform.LookAt(this.transform.position + _rigidbody.velocity);
        }
    }

    void DrawBeam(UnityEngine.Vector3 targetPos)
    {
        _lineRenderer.SetPosition(0, this.transform.position);
        _lineRenderer.SetPosition(1, targetPos);

        _lineRenderer.widthMultiplier = 1;
    }
    void RemoveBeam()
    {
        _lineRenderer.widthMultiplier = 0;
    }
}
