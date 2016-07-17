using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InventoryConsumer : MonoBehaviour
{
    Rigidbody rigidbody;
    Inventory inventory;
    Texture2D lootCursor;
	Texture2D normalCursor;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        inventory = GetComponentInChildren<Inventory>();
		normalCursor = Resources.Load<Texture2D>("Cursors/normal-cursor");
        lootCursor = Resources.Load<Texture2D>("Cursors/loot-cursor");

		Cursor.SetCursor (normalCursor, new Vector2 (0, 0), CursorMode.Auto);
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider.tag == "Loot") {
				if (Input.GetButtonDown ("Fire2")) {
					// check how close our character is to the loot
					//Vector3.Distance(hit.collider.t)
					var playerPosition = rigidbody.position;
					var hitPosition = hit.collider.transform.position;
					var distance = Vector3.Distance (playerPosition, hitPosition);

					if (distance < 1.5f) {
						var lootInfo = hit.collider.GetComponent<ItemInfo> ();
						inventory.AddItem (lootInfo.ItemId);
						Destroy (hit.collider.gameObject);
					}
				}

				Debug.Log ("lootCursor" + lootCursor);
				if (lootCursor == null) {
					Debug.Log ("null");
				}

				Cursor.SetCursor (lootCursor, new Vector2 (0, 0), CursorMode.Auto);
			} 
			else
			{
				Cursor.SetCursor (normalCursor, new Vector2 (0, 0), CursorMode.Auto);
			}
		} 
        

        // todo: if hovering over loot item, change mouse cursor.
    }
}
