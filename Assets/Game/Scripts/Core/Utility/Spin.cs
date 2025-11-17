using UnityEngine;

public class Spin : MonoBehaviour
{
	public Vector3 axis = Vector3.up;
	public float speed = 90f;

	private void Update()
	{
		transform.Rotate(axis.normalized * speed * Time.deltaTime, Space.Self);
	}
}
