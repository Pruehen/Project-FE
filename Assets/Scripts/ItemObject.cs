using UnityEngine;

public class ItemObject : MonoBehaviour
{
    Vector3 startPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;

    bool _droped = false;
    Rigidbody rb;
    SphereCollider sc;

    public bool Droped
    {
        get { return _droped; }
        set
        {
            if (_droped != value)
            {
                _droped = value;

                if(value)
                {
                    rb = this.gameObject.AddComponent<Rigidbody>();
                    rb.drag = 0.2f;

                    sc = this.gameObject.AddComponent<SphereCollider>();
                }
                else
                {
                    Destroy(rb);
                    Destroy(sc);
                }
            }
        }
    }

    public void SetPos(Vector3 startPos, Vector3 targetPos)
    {
        Droped = false;

        this.startPos = startPos;
        this.targetPos = targetPos;
        this.transform.position = startPos;        
    }
    public void ItemMove(float lerpValue)
    {
        this.transform.position = Vector3.Lerp(startPos, targetPos, lerpValue);
    }
    public void ItemDrop(Vector3 initPos)
    {
        Droped = true;

        this.transform.position = initPos + new Vector3(0, 0.5f, 0);
        rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized, ForceMode.VelocityChange);
        rb.AddTorque((Random.onUnitSphere * Random.Range(0f, 10)), ForceMode.VelocityChange);
    }
}