using EnumTypes;
using System.Collections;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    Vector3 startPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;

    bool _droped = false;
    bool _pooled = false;
    Charactor charactor;//æ∆¿Ã≈€¿ª »πµÊ«— ≈∏∞Ÿ¿« ∆Æ∑£Ω∫∆˚

    Rigidbody rb;
    BoxCollider bc;
    Outline outline;

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
                    rb.angularDrag = 1f;

                    StartCoroutine(AddComponent_BoxCollider());

                    outline = this.gameObject.AddComponent<Outline>();
                }
                else
                {
                    StopAllCoroutines();
                    Destroy(rb);
                    if (bc != null)
                    {
                        Destroy(bc);
                    }
                    Destroy(outline);
                }
            }
        }
    }
    IEnumerator AddComponent_BoxCollider()
    {
        yield return new WaitForSeconds(1);
        bc = this.gameObject.AddComponent<BoxCollider>();
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
        rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized + new Vector3(0, 10, 0), ForceMode.VelocityChange);
        rb.AddTorque((Random.onUnitSphere * Random.Range(0f, 10)), ForceMode.VelocityChange);
    }
    public void PoolItem(Charactor charactor)
    {
        _pooled = true;
        rb.drag = 5f;
        rb.useGravity = false;

        this.charactor = charactor;
    }
    public void GetItem(Charactor charactor)
    {
        _pooled = false;
        charactor.GetItem(this.name);
        this.RemoveObject();
    }

    void FixedUpdate()
    {
        if (_pooled)
        {
            Vector3 toTargetVec = charactor.transform.position - this.transform.position;
            rb.AddForce(toTargetVec.normalized * 50, ForceMode.Acceleration);
            if (toTargetVec.sqrMagnitude < 1)
            {
                GetItem(charactor);
            }
        }
    }


    public string GetName()
    {
        ItemData data = JsonDataManager.GetItem(this.gameObject.name);
        if (data != null)
        {
            string name = data.Name;
            return name;
        }
        else
        {
            return "≈∞∏¶ √£¿ª ºˆ æ¯¿Ω";
        }
    }

    public EntityType GetEntityType()
    {
        return EntityType.Item;
    }

    public Vector3 GetPos(Vector3 hitPos)
    {
        return this.transform.position;
    }

    public float InteractSpeedGain()
    {
        return 0;
    }

    public bool TryInteract(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        return false;
    }

    public bool TrySelect(Vector3 hitPos, Vector3 originPos, float checkRange)
    {
        this.RemoveObject();
        return false;
    }

    public void DeSelect()
    {
    }

    public void MouseEnter()
    {
        outline.IsOutlineEnabled = true;
    }

    public void MouseExit()
    {
        outline.IsOutlineEnabled = false;
    }
}