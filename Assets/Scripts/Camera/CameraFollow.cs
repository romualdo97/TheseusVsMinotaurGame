using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    private Camera m_camera;
    private MazeWorld m_world;

    public void OnLevelStarted()
    {
        m_world = MazeWorld.Instance;
        m_camera = GetComponent<Camera>();
        m_camera.backgroundColor = m_world.CurrentLevel.CameraClearColor;
        m_camera.transform.position = m_world.CurrentLevel.CameraPos;
        m_camera.fieldOfView = m_world.CurrentLevel.CameraFov;
    }

    private void Update()
    {
        transform.LookAt(m_target);
    }

}
