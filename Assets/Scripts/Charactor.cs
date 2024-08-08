using UnityEngine;

[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(MiningDevice))]
public class Charactor : MonoBehaviour
{
    Inventory _inventory;
    MiningDevice _miningDevice;

    Rigidbody _rigidbody;
    Vector3 _moveVector;
    Vector3 _lookPos;
    float _speed;


    [Range(1, 50)][SerializeField] float moveSpeed = 10;
    [Range(1, 50)][SerializeField] float interactionRange = 10;
    [Range(1, 100)][SerializeField] float interactionSpeed = 1;

    LineRenderer _lineRenderer;
    IInteractable onInteractObject;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _inventory = GetComponent<Inventory>();
        _miningDevice = GetComponent<MiningDevice>();
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
        if(interactableObject != null && interactableObject.TryInteract(this.transform.position, interactionRange))
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
