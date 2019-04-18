using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

    public Camera playerCamera;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int layer = 9;
        int layerMask = 1 << layer;

        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 20;
        Debug.DrawRay(transform.position, forward, Color.green, 1, true);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 10000000, layerMask) )
            //&& hit.collider.gameObject.name == "Target")
        {
            if (hit.collider.gameObject.name == "island1")
            {
                Debug.Log("targeting island 1");
            }
            else if (hit.collider.gameObject.name == "island2")
            {
                Debug.Log("targeting island 2");
            }
            else if (hit.collider.gameObject.name == "island3")
            {
                Debug.Log("targeting island 3");
            }
        }
	}
}
