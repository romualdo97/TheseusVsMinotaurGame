using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2Int MazePos { get => m_mazePos; }

    [Header("Other managers")]
    [SerializeField]
    private MazeWorld m_world;

    // Data
    private Vector2Int m_mazePos; // Position in the maze

    // For animating
    [Header("Animations")]
    [SerializeField]
    private float m_nodeToNodeDuration = 0.5f;
    private float m_elapsedTime = 0;
    private bool m_isTraversing = false;
    private Vector3 m_start;
    private Vector3 m_next;

    private void Start()
    {
        m_start = transform.position = MazePosToWorldPos(m_mazePos);
        m_elapsedTime = m_nodeToNodeDuration + 1f;
    }

    private void Update()
    {
        if (!m_isTraversing) return;

        if (m_elapsedTime >= m_nodeToNodeDuration)
        {
            m_elapsedTime = 0f;
            m_isTraversing = false;
        }
        else
        {
            // Update current pos
            float t = m_elapsedTime / m_nodeToNodeDuration;
            var lerp = Vector3.Lerp(m_start, m_next, t);
            lerp.y = 0.5f * Mathf.Sin(2.0f * Mathf.PI * t - Mathf.PI * 0.5F) + 0.5f + 0.25f;
            transform.position = lerp;

            // ...And the mazo pos
            m_mazePos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

            // Increse elapsed
            m_elapsedTime += Time.deltaTime;
        }
    }

    public void OnShouldMove(MazeCellEdges dir)
    {
        // Check movement
        if (m_world.MapInfo.IsWallAt(dir, m_mazePos)) return;

        // Start movement
        Vector2Int goal = m_mazePos + MazeGenerator.DIRECTIONS[dir];
        Vector3 worldGoal = MazePosToWorldPos(goal);
        StartMoving(worldGoal);
    }

    private void StartMoving(Vector3 newPos)
    {
        if (m_isTraversing) return;
        m_start = transform.position;
        m_next = newPos;
        m_isTraversing = true;
        m_elapsedTime = 0.0f;
    }

    private Vector3 MazePosToWorldPos(Vector2Int mazePos)
    {
        return new Vector3(mazePos.x + 0.5f, 0.25f, mazePos.y + 0.5f);
    }
}
