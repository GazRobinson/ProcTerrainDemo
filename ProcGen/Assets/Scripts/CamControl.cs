using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamControl : MonoBehaviour {
    public static CamControl Instance;
    Vector3 focalPoint = Vector3.zero;
    float yaw = 45f, pitch = 30f;
    public float yawSpeed = 30.0f;
    public float pitchSpeed = 20.0f;
    float distance = 170.0f;
    float panSpeed = 10.0f;
    Vector2 lastMousePos = Vector2.zero;
	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        float dt = Time.deltaTime;
        yaw -= Input.GetAxis("Cam_Horizontal") * yawSpeed * dt;
        pitch += Input.GetAxis("Cam_Vertical") * pitchSpeed * dt;
        if (Input.GetMouseButton(0))
        {
            yaw +=  (Input.mousePosition.x - lastMousePos.x) * 0.5f * yawSpeed * dt;
            pitch -= (Input.mousePosition.y - lastMousePos.y) * 0.5f * pitchSpeed * dt;
        }
        pitch = Mathf.Clamp( pitch, -80f, 80f );
        distance -= Input.mouseScrollDelta.y * Time.deltaTime * distance;
        distance = Mathf.Clamp( distance, 1f, 800f );

        /*Vector3 moveDir = transform.right * Input.GetAxis( "Horizontal" ) * panSpeed * dt;
        moveDir +=
        focalPoint += transform.right * Input.GetAxis( "Horizontal" ) * panSpeed * dt;
        focalPoint.z += Input.GetAxis( "Vertical" ) * panSpeed * dt;
        */
        Vector3 pos = focalPoint;
        Vector3 dir  =  Quaternion.AngleAxis( yaw, Vector3.up ) * Quaternion.AngleAxis( pitch, Vector3.right ) *  Vector3.forward;
        pos -= dir * distance;
        transform.position = pos;

        transform.LookAt( focalPoint, Vector3.up );
        lastMousePos = Input.mousePosition;
    }

    public void SetFocus( Vector3 pos ) {
        focalPoint = pos;
    }
}
