using EnumTypes;
using System.ComponentModel;
using UnityEngine;

public class Charactor : MonoBehaviour
{
    public CharactorInventoryModule builtIn_InventoryModule { get; private set; }
    public CharactorToolModule builtIn_ToolModule { get; private set; }
    public CharactorCraftingModule builtIn_CraftingModule { get; private set; }
    public CharactorMinerModule builtIn_MinerModule { get; private set; }

    BuildMode BuildMode
    {
        get 
        {
            return builtIn_ToolModule.BuildMode;
        }
    }

    Rigidbody _rigidbody;
    UnityEngine.Vector3 _moveVector;
    UnityEngine.Vector3 _lookPos;
    float _speed;

    bool _inventoryUIActive = false;
    bool _cmUIActive = false;

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

        builtIn_InventoryModule = GetComponent<CharactorInventoryModule>();
        builtIn_ToolModule = GetComponent<CharactorToolModule>();
        builtIn_CraftingModule = GetComponent<CharactorCraftingModule>();
        builtIn_MinerModule = GetComponent<CharactorMinerModule>();

        builtIn_InventoryModule.Init(this);

        builtIn_ToolModule.Init(this);
        builtIn_CraftingModule.Init(this);
        builtIn_MinerModule.Init(this);

        Player.Instance.PropertyChanged += OnPropertyChanged;
        Register_OnStart();
    }

    void Register_OnStart()
    {
        Player.Instance.Register_KeyAction(KeyCode.Alpha1, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(0));
        Player.Instance.Register_KeyAction(KeyCode.Alpha2, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(1));
        Player.Instance.Register_KeyAction(KeyCode.Alpha3, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(2));
        Player.Instance.Register_KeyAction(KeyCode.Alpha4, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(3));
        Player.Instance.Register_KeyAction(KeyCode.Alpha5, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(4));
        Player.Instance.Register_KeyAction(KeyCode.Alpha6, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(5));
        Player.Instance.Register_KeyAction(KeyCode.Alpha7, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(6));
        Player.Instance.Register_KeyAction(KeyCode.Alpha8, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(7));
        Player.Instance.Register_KeyAction(KeyCode.Alpha9, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(8));
        Player.Instance.Register_KeyAction(KeyCode.Alpha0, () => builtIn_ToolModule.ToolSelect_OnNumKeyClick(9));
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
            Vector3Int hitPoint_Grid = _lookPos.ToVector3Int();
            builtIn_ToolModule.ToolOnMove(hitPoint_Grid);
        }
    }

    public void OnMouseLeftClick()
    {
        if (BuildMode != BuildMode.None)
        {
            Vector3Int hitPoint_Grid = _lookPos.ToVector3Int();
            builtIn_ToolModule.ToolOnClick(hitPoint_Grid);
        }
        else
        {
            if(onMouseObjectTemp != null && onMouseObjectTemp.TrySelect(_lookPos, this.transform.position, interactionRange))
            {
                if (onSelectObject != null && onSelectObject != onMouseObjectTemp)
                {
                    onSelectObject?.DeSelect();
                }
                onSelectObject = onMouseObjectTemp;
            }
        }
    }

    void OnKeyDown(KeyCode key)
    {
        if (BuildMode != BuildMode.None)
        {
            builtIn_ToolModule.ToolOnKeyDown(key);
        }
    }

    public void TryInteract()
    {        
        if(onMouseObjectTemp != null && onMouseObjectTemp.TryInteract(_lookPos, this.transform.position, interactionRange))
        {
            onInteractObject = onMouseObjectTemp;
            builtIn_MinerModule.Mining_OnTryInteract(Time.deltaTime * interactionSpeed, onInteractObject);
        }
        else
        {
            EndInteract();
        }
    }
    public void EndInteract()
    {
        onInteractObject = null;
    }
    //================================================================================
    public void InventoryOpen()
    {
        _inventoryUIActive = true;
        builtIn_InventoryModule.Active_Wdw();
    }
    public void InventoryToggle()
    {
        _inventoryUIActive = !_inventoryUIActive;
        if (_inventoryUIActive)
        {
            builtIn_InventoryModule.Active_Wdw();
        }       
        else
        {
            builtIn_InventoryModule.Close_Wdw();
        }
    }
    public void InventoryClose()
    {
        _inventoryUIActive = false;
        builtIn_InventoryModule.Close_Wdw();
    }
    //================================================================================
    public void CraftingModuleOpen()
    {
        _cmUIActive = true;
        builtIn_CraftingModule.Active_Wdw();
    }
    public void CraftingModuleToggle()
    {
        _cmUIActive = !_cmUIActive;
        if (_cmUIActive)
        {
            builtIn_CraftingModule.Active_Wdw();
            Player.Instance.Command_CharactorInventoryOpen();
        }
        else
        {
            builtIn_CraftingModule.Close_Wdw();
        }
    }
    public void CraftingModuleClose()
    {
        _cmUIActive = false;
        builtIn_CraftingModule.Close_Wdw();
    }
    //================================================================================

    public void GetItem(string itemKey, int count = 1)
    {
        ItemData itemData = JsonDataManager.GetItem(itemKey);
        builtIn_InventoryModule.TryGetInputInventory().AddItem(itemData.Id_UShort, count, out int r);
    }
    public void GetItem(ushort itemKey, int count = 1)
    {        
        builtIn_InventoryModule.TryGetInputInventory().AddItem(itemKey, count, out int r);
    }
    public bool CanUseItem(ushort itemKey, int count = 1)
    {
        return builtIn_InventoryModule.TryGetInputInventory().CanUseItem(itemKey, count);
    }
    public void UseItem(ushort itemKey, int count = 1)
    {
        builtIn_InventoryModule.TryGetInputInventory().UseItem(itemKey, count);
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
