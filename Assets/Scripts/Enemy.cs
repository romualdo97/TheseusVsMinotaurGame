//#define ENABLE_SUPREME_KILLER_INSTINCT
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
        if (Input.GetKeyDown(KeyCode.F))
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
            do
            {
                base.CalculateInitialMazePos();
            }
            while (MazePos == m_player.MazePos || !m_world.MapInfo.IsValidCoord(MazePos));
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

        // Only moves two cells
        // If he can move one square horizontally and get closer, he will do that
        // Will do this only if after the movement is closer to player (killer instinct)
        // Distances should be calculated using manhattan (only 4 possible directions)

        // for each step until it reaches two steps
        //      if (can move horizontally?)
        //          if (will get closer?)
        //              horizontalStep()
        //      else if (can move vertically?)
        //          if (will get closer?)
        //              verticalStep()
        
        List<Vector2Int> coordsPath = new List<Vector2Int>();

#if ENABLE_SUPREME_KILLER_INSTINCT
        // This minotaur will always find the player
        coordsPath = m_world.MapInfo.CalculateCoordsPath(foundPath);
#else

        // Player and enemy pos
        Vector2Int playerPos = m_player.MazePos;
        Vector2Int currPos = MazePos;

        // Next 4 possible positions
        Vector2Int upPos = currPos + Vector2Int.up;
        Vector2Int downPos = currPos + Vector2Int.down;
        Vector2Int rightPos = currPos + Vector2Int.right;
        Vector2Int leftPos = currPos + Vector2Int.left;

        // Calculate manhattan distances
        int upToPlayerDistance = (playerPos - upPos).ManhattanDistance();
        int downToPlayerDistance = (playerPos - downPos).ManhattanDistance();
        int leftToPlayerDistance = (playerPos - leftPos).ManhattanDistance();
        int rightToPlayerDistance = (playerPos - rightPos).ManhattanDistance();
        int[] distances = new int[4] { upToPlayerDistance, downToPlayerDistance, leftToPlayerDistance, rightToPlayerDistance };

        // Which option will move the enemy closer? (ignoring the walls)
        int min = int.MaxValue;
        for (int i = 0; i < distances.Length; i++)
        {
            min = distances[i] < min ? distances[i] : min;
        }

        List<int> bestOptions = new List<int>();
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] == min)
            {
                bestOptions.Add(i);
            }
        }
        
        // There's an horizontal best option?
        MazeCellEdges dir = MazeCellEdges.None;
        for (int i = 0; i < bestOptions.Count; i++)
        {
            MazeCellEdges option = (MazeCellEdges) bestOptions[i];

            // Looking for 2 or 3 (horizontal movement option)
            if (bestOptions[i] > 1 && !m_world.MapInfo.IsWallAt(option, currPos))
            {
                dir = option;
            }
        }

        // If don't found an horizontal movement... use wathever it is the best option (which should be a vertical dir)
        if (dir == MazeCellEdges.None && !m_world.MapInfo.IsWallAt((MazeCellEdges) bestOptions[0], currPos))
        {
            dir = (MazeCellEdges) bestOptions[0];
        }

        // Add coord to the path
        if (dir != MazeCellEdges.None)
        {
            coordsPath.Add(currPos + MazeGenerator.DIRECTIONS[dir]);
        }
#endif

        // Traverse the found path
        TraversePath(coordsPath);
    }

    private void TraversePath(List<Vector2Int> path)
    {
        m_path = path;
        m_isTraversing = true;
        m_elapsedTime = m_nodeToNodeDuration + 1f;
    }

    private bool WillGetCloserToPlayer(Vector2Int currPos, Vector2Int nextCoord)
    {
        // Check if Will get closer using manhattan distance
        Vector2Int fromNewPosToPlayerPos = m_player.MazePos - nextCoord;
        int newDistance = Mathf.Abs(fromNewPosToPlayerPos.x) + Mathf.Abs(fromNewPosToPlayerPos.y);
        Vector2Int fromCurrPosToPlayerPos = m_player.MazePos - currPos;
        int currDistance = Mathf.Abs(fromCurrPosToPlayerPos.x) + Mathf.Abs(fromCurrPosToPlayerPos.y);

        // New position will satisfy the killer instinct of the minotaur
        if (newDistance <= currDistance)
        {
            return true;
        }

        return false;
    }
}
