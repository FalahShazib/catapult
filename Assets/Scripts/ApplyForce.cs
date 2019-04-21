using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
    public GameObject Balloon;
    public Transform TargetObject;
    public string TargetName;
    public GameObject CurrentStar;
    public GameObject localCam;
    [Range(20.0f, 75.0f)] public float LaunchAngle;

    // state
    private bool isFLying;
    private char planeNumber;

    // cache
    private Rigidbody rigid;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion initCameraRot;
    private bool triggerPressed = false;
    private bool triggerReleased = false;
    private float timeDelay;

    private int starCaught;
    private bool[] planesVisited;
    
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        isFLying = false;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        planesVisited = new bool[7];
    }

    /// <summary>
    /// launches the object towards the TargetObject with a given LaunchAngle
    /// Code adapted from (insert link)
    /// </summary>
    private void Launch()
    {
        if (TargetObject != null && !isFLying)
        {
            Debug.Log(TargetName);
            if (TargetName != "balloon")
            {
                isFLying = true;
                // think of it as top-down view of vectors: 
                //   we don't care about the y-component(height) of the initial and target position.
                Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
                Vector3 targetXZPos = new Vector3(TargetObject.position.x, 0.0f, TargetObject.position.z);

                // rotate the object to face the target
                initCameraRot = transform.rotation;
                transform.LookAt(targetXZPos);

                // shorthands for the formula
                float R = Vector3.Distance(projectileXZPos, targetXZPos);
                float G = Physics.gravity.y;
                float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
                float H = TargetObject.position.y - transform.position.y;

                // calculate the local space components of the velocity 
                // required to land the projectile on the target object 
                float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
                float Vy = tanAlpha * Vz;

                // create the velocity vector in local space and get it in global space
                Vector3 localVelocity = new Vector3(0f, Vy, Vz);
                Vector3 globalVelocity = transform.TransformDirection(localVelocity);

                // launch the object by setting its initial velocity and flipping its state
                transform.rotation = initCameraRot;
                rigid.velocity = globalVelocity;
            }
            else
            {
                transform.position = TargetObject.position;
                Debug.Log("game finished");
            }
        }
    }


    // resets the projectile to its initial position
    private void ResetToInitialState()
    {
        rigid.velocity = Vector3.zero;
        this.transform.SetPositionAndRotation(initialPosition, initialRotation);
        isFLying = false;
        CurrentStar = null;
    }

    // Update is called once per frame
    void Update()
    {
        DetectTrigger();
        CheckFinishState();

        // Increment angle when trigger is pressed
        if (triggerPressed)
        {
            IncrementAngle();
            // Start Launch when trigger is released
            if (triggerReleased)
            {
                if (CurrentStar != null)
                {
                    CurrentStar.SetActive(false);
                }
                Launch();
                triggerReleased = false;
                triggerPressed = false;
            }
        }

        ManageIslandStars();
        DetectReset();

        Quaternion rot = Quaternion.Euler(0, transform.rotation.y, 0);
        transform.rotation = rot;
    }

    /// <summary>
    /// Interaction with selectable items such as planes and islands
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Contains("plane"))
        {
            isFLying = false;

            planeNumber = other.gameObject.name[other.gameObject.name.Length - 1];
            if (CurrentStar != null)
            {
                string planeName = other.gameObject.name;
                string starName = CurrentStar.gameObject.name;
                int planeNumber = planeName.Length - 1;
                int starNumber = starName.Length - 1;
                if (starName[starNumber] == planeName[planeNumber])
                {
                    CurrentStar.SetActive(false);
                }
            }
        } else
        {
            planeNumber = 'n';
        }
    }

    /// <summary>
    /// Interaction with catchable items such as stars
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("star"))
        {
            Destroy(other.gameObject);
            Debug.Log("caught a star!");
            starCaught++;
        }
    }

    /// <summary>
    /// Increment launch angle the longer the trigger is held
    /// </summary>
    private void IncrementAngle()
    {
        if (Time.time > timeDelay)
        {
            float inc;
            timeDelay = Time.time + 0.75f;
            LaunchAngle = LaunchAngle + 5f;
            inc = LaunchAngle % 75;
            if (inc < 35f)
            {
                LaunchAngle = 35;
            }
        }
    }

    /// <summary>
    /// Detects trigger using Oculus Rift Right Index Trigger
    /// </summary>
    private void DetectTrigger()
    {
        if ((OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0))
        {
            triggerPressed = true;
        }
        else if (triggerPressed)
        {
            triggerReleased = true;
        }
    }

    /// <summary>
    /// Required adjustments for island star visibility
    /// </summary>
    private void ManageIslandStars()
    {
        if (CurrentStar != null && !isFLying)
        {
            CurrentStar.SetActive(true);
            string starName = CurrentStar.gameObject.name;
            int starNumber = starName.Length - 1;
            if (starName[starNumber] == planeNumber)
            {
                CurrentStar.SetActive(false);
            }
        }
        else if (CurrentStar != null)
        {
            CurrentStar.SetActive(false);
        }
    }

    /// <summary>
    /// Detect reset via Oculus Rift X Button
    /// </summary>
    private void DetectReset()
    {
        if ((OVRInput.Get(OVRInput.RawButton.X)))
        {
            ResetToInitialState();
            Debug.Log("stars: " + starCaught);
        }
    }

    private void RecordPlane(char number)
    {
        int num = int.Parse(number.ToString());
        planesVisited[num] = true;
    }

    private void CheckFinishState()
    {
        foreach (bool state in planesVisited)
        {
            if (!state)
            {
                return;
            }
        }
        Balloon.SetActive(true);
    }
}