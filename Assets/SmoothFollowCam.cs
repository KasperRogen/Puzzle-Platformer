using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera/3RDPerson Camera")]
public class SmoothFollowCam : MonoBehaviour
{

    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 15;
    // the height we want the camera to be above the target
    public float height = 5;
    // How much we 
    public float heightDamping = 3;
    public float rotationDamping = 3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            // Calculate the current rotation angles
            float wantedRotationAngleY = transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * 20;
            float wantedRotationAngleX = transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * 20;

            
            float currentRotationAngleY = transform.eulerAngles.y;
            float currentRotationAngleX = transform.eulerAngles.x;

            
            // Damp the rotation around the y-axis
            currentRotationAngleY = Mathf.LerpAngle(currentRotationAngleY, wantedRotationAngleY, rotationDamping * Time.deltaTime);
            currentRotationAngleX = Mathf.LerpAngle(currentRotationAngleX, wantedRotationAngleX, rotationDamping * Time.deltaTime);
            
            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(currentRotationAngleX, currentRotationAngleY, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target

            Vector3 pos = target.position;
            pos -= currentRotation * Vector3.forward * distance;
            //pos.y = currentHeight;

            //pos = new Vector3(pos.x, pos.y -= currentRotation * Vector3.forward * distance, pos.z);

            transform.position = pos;

            // Always look at the target
            transform.LookAt(target);
        }
    }


}