using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIK2d_Limb : MonoBehaviour
{
    public Vector3 inboardPosition {
        get {
            return transform.position - transform.right * transform.localScale.x * 0.5f;
        }
        set {
            transform.position = value + transform.right * transform.localScale.x * 0.5f;
        }
    }
    public Vector3 outboardPosition {
        get {
            return transform.position + transform.right * transform.localScale.x * 0.5f;
        }
        set {
            transform.position = value - transform.right * transform.localScale.x * 0.5f;
        }
    }

    public void LookAt(Vector3 position)
    {
        Vector3 oldIn = inboardPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, position - inboardPosition);
        transform.rotation *= Quaternion.AngleAxis(90.0f, Vector3.forward);
        inboardPosition = oldIn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(inboardPosition, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(outboardPosition, 0.25f);
    }

}
