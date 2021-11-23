using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_Gaz : MonoBehaviour {
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_IsGrounded = false;

    private Vector3 InputDir = Vector3.zero;
    private CharacterController cc;
    public float Speed = 5.0f;
    public float m_JumpSpeed = 10.0f;


    //MouseLook
    public float sensitivity = 5.0f;
    private Vector3 lastMouse = Vector3.zero;
    private Transform cam = null;
    private float yaw = 0f;
    private float pitch = 0f;
    public bool mouseYInversion = false;
    public bool useGravity = true;
	// Use this for initialization
	void Start () {
        cam = transform.GetChild( 0 );
        cc= GetComponent<CharacterController> ();

        lastMouse = Input.mousePosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
        MouseLook();
        
        if ( Input.GetKeyDown( KeyCode.R ) )
            ResetPos();
        if ( !useGravity ) {
            m_Velocity.y = 0.0f;
            if ( Input.GetKey( KeyCode.E ) )
                m_Velocity.y = 10f;
            if ( Input.GetKey( KeyCode.Q ) )
                m_Velocity.y = -10f;
        }
        InputDir = new Vector3( Input.GetAxis( "Horizontal" ), 0.0f, Input.GetAxis( "Vertical" ) );
        Vector3 groundVel = InputDir * Speed;
        groundVel = transform.TransformDirection( groundVel );
        m_Velocity.x = groundVel.x;
        if ( useGravity && !m_IsGrounded )
            m_Velocity += Physics.gravity * Time.deltaTime;
        m_Velocity.z = groundVel.z;

        m_IsGrounded = Physics.Raycast( transform.position, Vector3.down, 1.1f );

        if (m_IsGrounded && Input.GetButtonDown( "Jump" ) ) {

            m_Velocity.y = m_JumpSpeed;
            m_IsGrounded = false;
        }

        cc.Move( m_Velocity * Time.deltaTime);
    }

    private void MouseLook() {
        if ( Input.GetMouseButtonDown( 0 ) ) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if ( Input.GetKeyDown( KeyCode.Escape ) ) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if ( Cursor.lockState == CursorLockMode.Locked ) {
            Vector3 mouseDelta = Input.mousePosition - lastMouse;
            lastMouse = Input.mousePosition;
            yaw += Input.GetAxis( "Mouse X" ) * Time.deltaTime * sensitivity;
            pitch += Input.GetAxis( "Mouse Y" ) * Time.deltaTime * sensitivity * ( mouseYInversion ? 1f : -1f );

            pitch = Mathf.Clamp( pitch, -80f, 80f );
            cam.localRotation = Quaternion.AngleAxis( pitch, Vector3.right );
            transform.localRotation = Quaternion.AngleAxis( yaw, Vector3.up );
        }
    }
    private void ResetPos() {
        transform.position = new Vector3( 10f, 50f, 10f );
        m_Velocity = Vector3.zero;
        m_IsGrounded = false;
    }

    private void OnControllerColliderHit( ControllerColliderHit hit ) {
        if ( hit.normal == Vector3.up )
            Ground();
    }

    private void Ground() {
        m_IsGrounded = true;
        m_Velocity.y = 0.0f;
    }
}
