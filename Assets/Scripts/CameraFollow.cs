using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        transform.LookAt(m_target);
    }

}
