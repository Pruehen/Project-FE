using EnumTypes;
using System.ComponentModel;
using UnityEngine;

public class Charactor : MonoBehaviour
{
    CharactorInventoryModule _inventory;    
    CharactorToolModule _tool;
    CraftingModule _crafting;
    MinerModule _miner;

    BuildMode BuildMode
    {
        get 
        {
            return _tool.BuildMode;
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
    IInteractable onSelectObject;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();

        _inventory = GetComponent<CharactorInventoryModule>();
        _tool = GetComponent<CharactorToolModule>();
        _crafting = GetComponent<CraftingModule>();
        _miner = GetComponent<MinerModule>();

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
        Player.Instance.Register_KeyAction(KeyCode.R, () => OnKeyDown(KeyCode.R));
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
            DrawBeam(onInteractObject.GetPos(_lookPos));
        }
        else
        {
            RemoveBeam();
        }

        if(onSelectObject != null)
        {
            if(Vector3.Distance(this.transform.position, onSelectObject.GetPos(this.transform.position)) > interactionRange)
            {
                onSelectObject.DeSelect();
                onSelectObject = null;
            }
        }

        GridDraw_OnMouseMove();
    }

    void GridDraw_OnMouseMove()
    {
        if (BuildMode != BuildMode.None)
        {
            Vector3Int hitPoint_Grid = _lookPos.ToIntVector();
            _tool.ToolOnMove(hitPoint_Grid);
        }
    }

    public void OnMouseLeftClick()
    {
        if (BuildMode != BuildMode.None)
        {
            Vector3Int hitPoint_Grid = _lookPos.ToIntVector();
            _tool.ToolOnClick(hitPoint_Grid);
        }
        else
        {
            if(onMouseObjectTemp != null && onMouseObjectTemp.TrySelect(_lookPos, this.transform.position, interactionRange))
            {
                onSelectObject?.DeSelect();
                onSelectObject = onMouseObjectTemp;
            }
        }
    }

    void OnKeyDown(KeyCode key)
    {
        if (BuildMode != BuildMode.None)
        {
            _tool.ToolOnKeyDown(key);
        }
    }

    public void TryInteract()
    {        
        if(onMouseObjectTemp != null && onMouseObjectTemp.TryInteract(_lookPos, this.transform.position, interactionRange))
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

    public void InventoryOpen()
    {
        _inventoryUIActive = true;
        _inventory.Active_Wdw();
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
    public void InventoryClose()
    {
        _inventoryUIActive = false;
        _inventory.Close_Wdw();
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
        _lineRenderer.startWidth = 0;
        _lineRenderer.endWidth = 0.5f;
    }
    void RemoveBeam()
    {
        _lineRenderer.endWidth = 0;
    }
}
