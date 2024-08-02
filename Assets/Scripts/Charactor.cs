using UnityEngine;

public class Charactor : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 _moveVector;
    Vector3 _lookPos;
    float _speed;

    [Range(1, 50)][SerializeField] float Max_Speed;

    LineRenderer _lineRenderer;
    IInteractable onInteractObject;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        Move_OnFixedUpdate();
        PointLook_OnFixedUpdate();        
    }

    private void Update()
    {
        if(onInteractObject != null)
        {
            DrawBeam(onInteractObject.GetPos());
        }
        else
        {
            RemoveBeam();
        }
    }

    public void SetMoveVector(Vector3 vector)
    {
        _moveVector = vector;
    }
    public void SetLookPosVector(Vector3 vector)
    {
        _lookPos = vector;
    }
    public void TryInteract(IInteractable interactableObject)
    {        
        if(interactableObject != null && interactableObject.TryInteract(this.transform.position, 10))
        {
            onInteractObject = interactableObject;           
        }
        else
        {
            onInteractObject = null;
        }
    }
    public void EndInteract()
    {
        onInteractObject = null;
    }

    void Move_OnFixedUpdate()
    {        
        if (_moveVector != Vector3.zero)
        {
            _speed = _rigidbody.velocity.magnitude;
            if (_speed < Max_Speed)
            {
                if(onInteractObject == null)
                {
                    _rigidbody.AddForce(_moveVector * Max_Speed * 3, ForceMode.Force);
                }                
                else
                {
                    _rigidbody.AddForce(_moveVector * Max_Speed, ForceMode.Force);
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

    void DrawBeam(Vector3 targetPos)
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
