using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 inputVector_Move { get; private set; }
    Action<Vector2> onInput_Move;

    [SerializeField] Charactor controlledCharactor;

    private void Start()
    {
        onInput_Move += Command_CharactorMove;
    }
    // Update is called once per frame
    void Update()
    {
        PlayerInput_OnUpdate();
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

    void Command_CharactorMove(Vector2 inputVector)
    {
        if(controlledCharactor != null)
        {
            controlledCharactor.SetMoveVector(new Vector3(inputVector.x, 0, inputVector.y));
        }
    }
}
