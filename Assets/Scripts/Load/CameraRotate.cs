using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Vector3 rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = transform.eulerAngles + (rotateSpeed * Time.deltaTime);
    }
}
