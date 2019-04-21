using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningStar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    /// <summary>
    /// Used to spin stars indicating island selection
    /// </summary>
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, 50 * Time.deltaTime);

    }
}
