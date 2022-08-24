using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rig;
    private Vector3 jumpForce = new Vector3(0.0f, 5.0f, 0.0f);
    private float speed = 10.0f;
    private int pushButtonCount = 0;
    private bool moveFlag = true;

    public enum Item
    {
        none,
        r,
        normal,
    };
    Item itemType;

    public void Init(Vector3 position)
    {
        rig = this.GetComponent<Rigidbody>();
        this.rig.position = position;
    }

    public void ManagedUpdate()
    {
        itemType = Item.none;
        Control();
    }

    private void Control()
    {
        if(Input.GetMouseButton(0))
        {
            pushButtonCount++;
            Debug.Log(pushButtonCount);
        }
        else
        {
            pushButtonCount = 0;
        }

        if(Input.GetMouseButton(0))
        {
            if(moveFlag)
            {
                moveFlag = false;
            }
            else
            {
                moveFlag = true;
            }

            if(pushButtonCount > 70)
            {
                moveFlag = false;
            }
        }

        if (moveFlag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                Vector3 playerPos = this.transform.position;
                Vector3 direction = hitInfo.point - playerPos;
                transform.rotation = Quaternion.LookRotation(new Vector3
                            (direction.x, 0, direction.z));
                this.rig.position += transform.forward * speed * Time.deltaTime;
            }
            Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red, 10.0f, false);
        }

        if(Input.GetMouseButtonDown(1))
        {
            rig.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.transform.tag == "NormalItem")
        {
            itemType = Item.normal;
            col.gameObject.SetActive(false);
        }
    }

    public Item FindItem()
    {
        return itemType;
    }
}
