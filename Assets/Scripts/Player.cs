using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : CharacterEntity
{
    public Vector2Int PrevMazePos { get; private set; }

    [SerializeField]
    private Enemy m_enemy;

    private void Start()
    {
        Assert.IsNotNull(m_enemy, "Player requires a reference of enemy");

        InitEntity();
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

    protected override void CalculateInitialMazePos()
    {
        if (m_world.MapInfo.IsValidCoord(m_world.CurrentLevel.PlayerPos))
        {
            m_mazePos = m_world.CurrentLevel.PlayerPos;
        }
        else
        {
            do
            {
                base.CalculateInitialMazePos();
            }
            while (MazePos == m_enemy.MazePos || !m_world.MapInfo.IsValidCoord(MazePos));
        }
    }

    public void OnShouldMove(MazeCellEdges dir)
    {
        // Check movement
        if (m_world.MapInfo.IsWallAt(dir, m_mazePos)) return;

        // Start movement
        PrevMazePos = m_mazePos;
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
}
