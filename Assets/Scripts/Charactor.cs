using UnityEngine;

public class Charactor : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 _moveVector;
    float _speed;

    [Range(1, 50)][SerializeField] float Max_Speed;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = _rigidbody.velocity;

        if(_moveVector != Vector3.zero)
        {
            _speed = velocity.magnitude;
            if (_speed < Max_Speed)
            {
                _rigidbody.AddForce(_moveVector * Max_Speed * 3, ForceMode.Force);
            }
        }

        this.transform.LookAt(this.transform.position + velocity);
    }

    public void SetMoveVector(Vector3 vector)
    {
        _moveVector = vector;
    }
}
