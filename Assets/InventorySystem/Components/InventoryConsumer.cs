using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InventoryConsumer : MonoBehaviour
{
    Rigidbody rigidbody;
    Inventory inventory;

    void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
        //inventory = GetComponent<Inventory>();
        //inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        inventory = GetComponentInChildren<Inventory>();
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Loot")
        {
            inventory.AddItem(1003);
            Destroy(col.gameObject);
        }
    }

    void Update()
    {

    }
}
