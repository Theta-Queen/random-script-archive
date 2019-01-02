using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipFlyScript : MonoBehaviour {

    public float maxShipSpeed = 60f;//max ship speed
    public float shipThrustSpeed = 20f; //how fast the ship will thrust
    public float initRiseDistance = 100f;//distance ship will rise when first started
    public float rotateStrength = 0.6f;//how fast ship will rotate on mouse
    public float tiltSpeed = 400f; //speed for tilting ship left and right with Q and E
    public float shipDrag = 4f;

    bool isTouchingGround; //raw bool for collision detection (used for detecting ground)
    bool canInitRise; //bool for allowing ship to rise when started
    bool isActive; //bool for checking if ship is active

    float tiltCooldown = 0f;

    Rigidbody shipRigidbody;

    void Start () {
        isActive = false;
        shipRigidbody = gameObject.GetComponent<Rigidbody>();
    }

	void Update () {
        if (isActive == true)
        {
            if (canInitRise == true)
            {
                //transform.Translate(transform.up.normalized * initRiseDistance);
                shipRigidbody.AddForce(Vector3.up * initRiseDistance);
                StartCoroutine(riseUpWait());
            }

            //lock mouse and disable gravity 
            shipRigidbody.useGravity = false;
            Cursor.lockState = CursorLockMode.Locked;

            //rotate ship along with mouse
            float h = rotateStrength * Input.GetAxis("Mouse X") * (1 + shipRigidbody.velocity.magnitude / 5);
            float v = rotateStrength * Input.GetAxis("Mouse Y") * (1 + shipRigidbody.velocity.magnitude / 5);
            transform.Rotate(v, h, 0);

            //'thrust' ship backwards and forwards
            //uses drag to slow down ship when not moving
            if (Input.GetKey(KeyCode.W))
            {
                if (Vector3.Project(gameObject.GetComponent<Rigidbody>().velocity, Vector3.back).magnitude < maxShipSpeed) //make sure ship is not reaching max speed
                {
                    shipRigidbody.AddForce(-transform.forward * shipThrustSpeed);
                }
                shipRigidbody.drag = shipThrustSpeed / 30;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (Vector3.Project(gameObject.GetComponent<Rigidbody>().velocity, Vector3.forward).magnitude < maxShipSpeed)
                {
                    shipRigidbody.AddForce(transform.forward * shipThrustSpeed);
                }
                shipRigidbody.drag = shipThrustSpeed / 30;
            }
            else shipRigidbody.drag = shipDrag;

            //tilt ship left(Q) and right (E)
            if (Input.GetKey(KeyCode.E))
            {
               //transform.Rotate(Vector3.forward * tiltSpeed * Time.deltaTime);
               shipRigidbody.AddTorque(transform.forward * tiltSpeed * Time.deltaTime);
               shipRigidbody.constraints = RigidbodyConstraints.None;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                shipRigidbody.AddTorque(-transform.forward * tiltSpeed * Time.deltaTime);
                //transform.Rotate(-Vector3.forward * tiltSpeed * Time.deltaTime);
                shipRigidbody.constraints = RigidbodyConstraints.None;
            }
            else
            {
                shipRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            
        }   
        
        //DEBUG - use just for activating/disactivating ship
        if (Input.GetKey(KeyCode.O))
            StartShip();
        if (Input.GetKey(KeyCode.P))
            StopShip();
    }

    //Use function for starting up ship
    public void StartShip()
    {
        isActive = true;
        if (isTouchingGround == true)
            canInitRise = true;
        else canInitRise = false;

        shipRigidbody.constraints = RigidbodyConstraints.FreezeRotation;//freeze rigidbody rotation for tilting function
    }
    //function for stopping ship
    public void StopShip()
    {
        isActive = false;
        canInitRise = false;
        Cursor.lockState = CursorLockMode.None;
        shipRigidbody.useGravity = true;

        shipRigidbody.constraints = RigidbodyConstraints.None;
    }

    private void OnCollisionEnter(Collision col)
    {
        isTouchingGround = true;
    }
    private void OnCollisionExit(Collision col)
    {
        isTouchingGround = false;
    }

    IEnumerator riseUpWait()
    {
        yield return new WaitForSeconds(1);
        canInitRise = false;
    }
}
