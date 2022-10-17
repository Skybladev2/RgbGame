using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour {

    public Transform target;
	
	void Update () {
        transform.position = target.position + Vector3.forward * -10f; 
	}
}
