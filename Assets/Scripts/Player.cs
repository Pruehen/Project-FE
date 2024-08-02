using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVector_Move { get; private set; }
    public Vector3 lookTargetPosVector { get; private set; }

    Action<Vector2> onInput_Move;
    Action<Vector3> onLookTargetPosSet;

    [SerializeField] Charactor controlledCharactor;

    IInteractable onMouseObject;

    private void Start()
    {
        onInput_Move += Command_CharactorMove;
        onLookTargetPosSet += Command_SetCharactorLookPos;
    }
    // Update is called once per frame
    void Update()
    {
        PlayerInput_OnUpdate();
        SetLookTargetPos_OnUpdate();
        MouseClickCheck_OnUpdate();
    }    

    void PlayerInput_OnUpdate()
    {
        inputVector_Move = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputVector_Move += new Vector2(0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector_Move += new Vector2(0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector_Move += new Vector2(-1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector_Move += new Vector2(1, 0);
        }

        inputVector_Move = inputVector_Move.normalized;
        onInput_Move?.Invoke(inputVector_Move);
    }

    void SetLookTargetPos_OnUpdate()
    {        
        Vector3 mousePosition = Input.mousePosition;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);                
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.TryGetComponent(out onMouseObject))
            {
                lookTargetPosVector = onMouseObject.GetPos();
                //Debug.Log($"{onMouseObject.GetData()} º±≈√");
            }
            else
            {
                onMouseObject = null;
                lookTargetPosVector = hit.point;
            }            
            onLookTargetPosSet?.Invoke(lookTargetPosVector);
        }
    }

    void MouseClickCheck_OnUpdate()
    {
        if(Input.GetMouseButton(1))
        {
            Command_TryInteract();
        }
        if(Input.GetMouseButtonUp(1))
        {
            Command_EndInteract();
        }
    }

    void Command_CharactorMove(Vector2 inputVector)
    {
        if(controlledCharactor != null)
        {
            controlledCharactor.SetMoveVector(new Vector3(inputVector.x, 0, inputVector.y));
        }
    }
    void Command_SetCharactorLookPos(Vector3 pos)
    {
        if (controlledCharactor != null)
        {
            controlledCharactor.SetLookPosVector(pos);
        }
    }
    void Command_TryInteract()
    {
        if (controlledCharactor != null)
        {
            controlledCharactor.TryInteract(onMouseObject);
        }
    }
    void Command_EndInteract()
    {
        if (controlledCharactor != null)
        {
            controlledCharactor.EndInteract();
        }
    }
}
