using EnumTypes;
using System.ComponentModel;
using UnityEngine;

public class Charactor : MonoBehaviour
{
    public CharactorInventoryModule BuiltIn_InventoryModule { get; private set; }
    public CharactorToolModule BuiltIn_ToolModule { get; private set; }
    public CharactorCraftingModule BuiltIn_CraftingModule { get; private set; }
    public CharactorMinerModule BuiltIn_MinerModule { get; private set; }

    BuildMode BuildMode
    {
        get 
        {
            return BuiltIn_ToolModule.BuildMode;
        }
    }

    Rigidbody _rigidbody;
    Vector3 _moveVector;
    Vector3 _lookPos;
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

    bool isInit = false;
    // Start is called before the first frame update
    public void Init(SaveData_Charactor saveData_Charactor)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();

        BuiltIn_InventoryModule = GetComponent<CharactorInventoryModule>();
        BuiltIn_ToolModule = GetComponent<CharactorToolModule>();
        BuiltIn_CraftingModule = GetComponent<CharactorCraftingModule>();
        BuiltIn_MinerModule = GetComponent<CharactorMinerModule>();

        BuiltIn_InventoryModule.LodeData_OnInit(saveData_Charactor.dic_ItemId_Count);

        BuiltIn_ToolModule.Init(this);
        BuiltIn_CraftingModule.Init(this);
        BuiltIn_MinerModule.Init(this);

        Player.Instance.PropertyChanged += OnPropertyChanged;

        isInit = true;
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
        if (isInit == false) return;
        
        Move_OnFixedUpdate();
        PointLook_OnFixedUpdate();        
    }

    private void Update()
    {
        if (isInit == false) return;

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
            BuiltIn_ToolModule.ToolOnMove(hitPoint_Grid);
        }
    }

    public void OnMouseLeftClick()
    {
        if (BuildMode != BuildMode.None)
        {
            Vector3Int hitPoint_Grid = _lookPos.ToVector3Int();
            BuiltIn_ToolModule.ToolOnClick(hitPoint_Grid);
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

    public void OnKeyDown(KeyCode key)
    {
        if (BuildMode != BuildMode.None)
        {
            BuiltIn_ToolModule.ToolOnKeyDown(key);
        }
    }

    public void TryInteract()
    {        
        if(onMouseObjectTemp != null && onMouseObjectTemp.TryInteract(_lookPos, this.transform.position, interactionRange))
        {
            onInteractObject = onMouseObjectTemp;
            BuiltIn_MinerModule.Mining_OnTryInteract(Time.deltaTime * interactionSpeed, onInteractObject);
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
        BuiltIn_InventoryModule.Active_Wdw();
    }
    public void InventoryToggle()
    {
        _inventoryUIActive = !_inventoryUIActive;
        if (_inventoryUIActive)
        {
            BuiltIn_InventoryModule.Active_Wdw();
        }       
        else
        {
            BuiltIn_InventoryModule.Close_Wdw();
        }
    }
    public void InventoryClose()
    {
        _inventoryUIActive = false;
        BuiltIn_InventoryModule.Close_Wdw();
    }
    //================================================================================
    public void CraftingModuleOpen()
    {
        _cmUIActive = true;
        BuiltIn_CraftingModule.Active_Wdw();
    }
    public void CraftingModuleToggle()
    {
        _cmUIActive = !_cmUIActive;
        if (_cmUIActive)
        {
            BuiltIn_CraftingModule.Active_Wdw();
            Player.Instance.Command_CharactorInventoryOpen();
        }
        else
        {
            BuiltIn_CraftingModule.Close_Wdw();
        }
    }
    public void CraftingModuleClose()
    {
        _cmUIActive = false;
        BuiltIn_CraftingModule.Close_Wdw();
    }
    //================================================================================

    public void GetItem(string itemKey, int count = 1)
    {
        ItemData itemData = JsonDataManager.GetItem(itemKey);
        BuiltIn_InventoryModule.TryGetInputInventory().AddItem(itemData.Id_UShort, count, out int r);
    }
    public void GetItem(ushort itemKey, int count = 1)
    {        
        BuiltIn_InventoryModule.TryGetInputInventory().AddItem(itemKey, count, out int r);
    }
    public bool CanUseItem(ushort itemKey, int count = 1)
    {
        return BuiltIn_InventoryModule.TryGetInputInventory().CanUseItem(itemKey, count);
    }
    public void UseItem(ushort itemKey, int count = 1)
    {
        BuiltIn_InventoryModule.TryGetInputInventory().UseItem(itemKey, count);
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
        if(_lookPos != Vector3.zero)
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
