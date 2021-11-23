using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIK2D : MonoBehaviour
{
    [SerializeField] private Transform m_GoalObject;
    private Vector3 WorldGoal {
        get { return m_GoalObject.position; }
    }
    
    public  FABRIK2d_Limb[] m_Limbs;
    private Vector3         m_CurrentGoal   = Vector3.zero;
    private FABRIK2d_Limb   fABRIK2D_Limb   = null;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Limbs = GetComponentsInChildren<FABRIK2d_Limb>();
    }

    // Update is called once per frame
    void Update()
    {
        int c = 0;
        while (c < 100 && Vector3.Distance(m_Limbs[m_Limbs.Length - 1].outboardPosition, WorldGoal) > Mathf.Epsilon)
        {
            Forward();
            //Backward();
            c++;
        }
    }

    void Forward()
    {
        m_CurrentGoal = WorldGoal;
        for(int i = m_Limbs.Length-1; i >-1; i--)
        {
            m_Limbs[i].LookAt(m_CurrentGoal);
            m_Limbs[i].outboardPosition = m_CurrentGoal;
            m_CurrentGoal = m_Limbs[i].inboardPosition;
        }
    }

    void Backward()
    {
        m_CurrentGoal = transform.position;
        for (int i = 0; i < m_Limbs.Length; i++)
        {
            m_Limbs[i].inboardPosition = m_CurrentGoal;
            m_CurrentGoal = m_Limbs[i].outboardPosition;
        }
    }
}
