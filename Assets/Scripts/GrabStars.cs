using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabStars : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "star")
        {
            Destroy(this.gameObject);
            Debug.Log("caught a star");
        }
    }
}
