using UnityEngine;
using System.Collections;

public class CameraRotateAround : MonoBehaviour
{

	public Transform target;
	public Vector3 offset;
	public float sensitivity; // чувствительность мышки
	public float upLimit, downLimit = 80; // ограничение вращения по Y
	public float zoomSpeed; // чувствительность при увеличении, колесиком мышки
	public float zoomMax; // макс. увеличение
	public float zoomMin; // мин. увеличение
	private float X, Y;

	void Start()
	{
		offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax));
		transform.position = target.position + offset;
		CameraControl();
	}

	void Update()
	{
		if(Input.GetMouseButton(1))
		{
			CameraControl();
		}
	}
	private void CameraControl()
    {
		if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoomSpeed;
		else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoomSpeed;
		offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));

		X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
		Y -= Input.GetAxis("Mouse Y") * sensitivity;
		Y = Mathf.Clamp(Y, downLimit, upLimit);
		transform.localEulerAngles = new Vector3(Y, X, 0);
		transform.position = transform.localRotation * offset + target.position;
	}
}