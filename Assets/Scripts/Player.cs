using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player : SceneSingleton<Player>
{
    [SerializeField] LayerMask mouseSelectablelayerMask;
    public Vector2 InputVector_Move { get; private set; }

    Vector3 _lookTargetPosVector;
    public Vector3 LookTargetPosVector
    {
        get { return _lookTargetPosVector; }
        private set
        {
            if(_lookTargetPosVector != value)
            {
                _lookTargetPosVector = value;
                OnLookTargetPosSet?.Invoke(_lookTargetPosVector);
                OnPropertyChanged(nameof(LookTargetPosVector));
            }
        }
    }

    IInteractable _onMouseObjectTemp;
    public IInteractable OnMouseObjectTemp
    {
        get { return _onMouseObjectTemp; }
        private set
        {
            if (_onMouseObjectTemp != null)
            {
                _onMouseObjectTemp.MouseExit();
            }
            _onMouseObjectTemp = value;
            OnPropertyChanged(nameof(OnMouseObjectTemp));
            if (_onMouseObjectTemp != null)
            {
                _onMouseObjectTemp.MouseEnter();
            }
        }
    }    

    public Action<Vector3> OnLookTargetPosSet;
    public Action<string> OnMouseObjectNameChanged;
    Action<KeyCode> OnKeyClickDown;

    Dictionary<KeyCode, Action> keyActions;
    public void Register_KeyAction(KeyCode key, Action callBack) { keyActions[key] += callBack; }
    public void UnRegister_KeyAction(KeyCode key, Action callBack) { keyActions[key] -= callBack; }

    [SerializeField] int controlledCharactorIndex = 0;
    Charactor _charactor;

    public Charactor ControlledCharactor
    {
        get
        {
            return _charactor;
        }
    }
    void SetControlledCharactor()
    {
        Charactor charactor = CharactorManager.Instance.GetCharactor(controlledCharactorIndex);

        if (_charactor != charactor)
        {
            _charactor = charactor;
            _charactor.Init();

            CamMove.Instance.SetTargetObject(_charactor.transform);
        }
    }

    public void GetItem(ushort itemKey, int count = 1)
    {
        if(itemKey != 0)
        {
            ControlledCharactor.GetItem(itemKey, count);
        }        
    }
    public void GetItem(string itemKey, int count = 1)
    {
        ControlledCharactor.GetItem(itemKey, count);
    }
    public bool CanUseItem(ushort itemKey, int count = 1)
    {
        if(itemKey == 0)
        {
            return false;
        }
        return ControlledCharactor.CanUseItem(itemKey, count);
    }
    public void UseItem(ushort itemKey, int count = 1)
    {
        ControlledCharactor.UseItem(itemKey, count);
    }

    public void Init()
    {
        keyActions = new Dictionary<KeyCode, Action>
        {
            { KeyCode.I, () => OnKeyClickDown?.Invoke(KeyCode.I) },
            { KeyCode.E, () => OnKeyClickDown?.Invoke(KeyCode.E) },
            { KeyCode.R, () => OnKeyClickDown?.Invoke(KeyCode.R) },
            { KeyCode.Tab, () => OnKeyClickDown?.Invoke(KeyCode.Tab) },
            { KeyCode.Escape, () => OnKeyClickDown?.Invoke(KeyCode.Escape) },

            { KeyCode.Alpha1, () => OnKeyClickDown?.Invoke(KeyCode.Alpha1) },
            { KeyCode.Alpha2, () => OnKeyClickDown?.Invoke(KeyCode.Alpha2) },
            { KeyCode.Alpha3, () => OnKeyClickDown?.Invoke(KeyCode.Alpha3) },
            { KeyCode.Alpha4, () => OnKeyClickDown?.Invoke(KeyCode.Alpha4) },
            { KeyCode.Alpha5, () => OnKeyClickDown?.Invoke(KeyCode.Alpha5) },
            { KeyCode.Alpha6, () => OnKeyClickDown?.Invoke(KeyCode.Alpha6) },
            { KeyCode.Alpha7, () => OnKeyClickDown?.Invoke(KeyCode.Alpha7) },
            { KeyCode.Alpha8, () => OnKeyClickDown?.Invoke(KeyCode.Alpha8) },
            { KeyCode.Alpha9, () => OnKeyClickDown?.Invoke(KeyCode.Alpha9) },
            { KeyCode.Alpha0, () => OnKeyClickDown?.Invoke(KeyCode.Alpha0) }
        };

        Register_KeyAction(KeyCode.Tab, Command_CharactorInventoryToggle);
        Register_KeyAction(KeyCode.I, Command_CharactorInventoryToggle);
        Register_KeyAction(KeyCode.E, Command_CharactorCraftingModuleToggle);
        Register_KeyAction(KeyCode.Escape, Command_CharactorInventoryClose_OnEscClick);
        //foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
        //{
        //    keyActions.Add(value, () => OnKeyClickDown?.Invoke(value));
        //}      

        SetControlledCharactor();
    }
    // Update is called once per frame
    void Update()
    {
        OnPlayerMoveInput_OnUpdate();
        OnMouseMove_OnUpdate();
        OnMouseClick_OnUpdate();

        OnKeyDown_OnUpdate();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 50;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Time.timeScale = 1;
        }
    }

    void OnPlayerMoveInput_OnUpdate()
    {
        InputVector_Move = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            InputVector_Move += new Vector2(0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            InputVector_Move += new Vector2(0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            InputVector_Move += new Vector2(-1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            InputVector_Move += new Vector2(1, 0);
        }

        InputVector_Move = InputVector_Move.normalized;
        OnPropertyChanged(nameof(InputVector_Move));
    }

    void OnMouseMove_OnUpdate()
    {
        Vector3 mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, mouseSelectablelayerMask))
        {
            string mouseOverObjectName;

            if (hit.collider.TryGetComponent(out IInteractable onMouseObject))
            {
                LookTargetPosVector = onMouseObject.GetPos(hit.point);
                mouseOverObjectName = onMouseObject.GetName();
                OnMouseObjectTemp = onMouseObject;
            }
            else
            {
                OnMouseObjectTemp = null;
                LookTargetPosVector = hit.point;
                mouseOverObjectName = null;
            }            
            OnMouseObjectNameChanged?.Invoke(mouseOverObjectName);
        }
    }


    void OnMouseClick_OnUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Command_TryInteract();
        }
        if (Input.GetMouseButtonUp(1))
        {
            Command_EndInteract();
        }
        if (Input.GetMouseButtonDown(0))
        {
            ControlledCharactor?.OnMouseLeftClick();
        }
    }

    void OnKeyDown_OnUpdate()
    {
        foreach (var keyAction in keyActions)
        {
            if (Input.GetKeyDown(keyAction.Key))
            {
                keyAction.Value.Invoke();
                Debug.Log(keyAction.Key);
            }
        }
    }

    void Command_TryInteract()
    {
        if (ControlledCharactor != null)
        {
            ControlledCharactor.TryInteract();
        }
    }
    void Command_EndInteract()
    {
        ControlledCharactor?.EndInteract();
    }

    public void Command_CharactorInventoryOpen()
    {
        ControlledCharactor?.InventoryOpen();
    }
    void Command_CharactorInventoryToggle()
    {
        ControlledCharactor?.InventoryToggle();
    }
    void Command_CharactorCraftingModuleToggle()
    {
        ControlledCharactor?.CraftingModuleToggle();
    }
    void Command_CharactorInventoryClose_OnEscClick()
    {
        ControlledCharactor?.InventoryClose();
        ControlledCharactor?.CraftingModuleClose();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)//값이 변경되었을 때 이벤트를 발생시키기 위한 용도 (데이터 바인딩)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
