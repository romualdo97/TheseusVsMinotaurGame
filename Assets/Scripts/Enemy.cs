using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemy : CharacterEntity
{
    [SerializeField]
    private Player m_player;
    private List<Vector2Int> m_path;

    // Start is called before the first frame update
    private void Start()
    {
        Assert.IsNotNull(m_player, "Enemy requires a reference of player");

        InitEntity();
    }

    // Update is called once per frame
    private void Update()
    {
        // Just for debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GoToPlayerNode();
        }

        if (m_path == null || !m_isTraversing) return;

        if (m_elapsedTime >= m_nodeToNodeDuration)
        {
            m_isTraversing = m_path.Count > 0;
            if (!m_isTraversing) return;

            m_start = transform.position;
            m_next = MazePosToWorldPos(m_path[0]);
            m_path.RemoveAt(0);

            m_elapsedTime = 0f;
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
        if (m_world.MapInfo.IsValidCoord(m_world.CurrentLevel.EnemyPos) && m_world.CurrentLevel.EnemyPos != m_world.CurrentLevel.PlayerPos)
        {
            m_mazePos = m_world.CurrentLevel.EnemyPos;
        }
        else
        {
            base.CalculateInitialMazePos();
        }
    }

    private void GoToPlayerNode()
    {
        Vector2Int goalCell = m_player.MazePos;

        List<MazeCellNode> foundPath;
        if (!m_world.MapInfo.SearchPath(MazePos, goalCell, out foundPath))
        {
            return;
        }

        // Traverse the found path
        List<Vector2Int> coordsPath = m_world.MapInfo.CalculateCoordsPath(foundPath);
        TraversePath(coordsPath);
    }

    private void TraversePath(List<Vector2Int> path)
    {
        m_path = path;
        m_isTraversing = true;
        m_elapsedTime = m_nodeToNodeDuration + 1f;
    }
}
