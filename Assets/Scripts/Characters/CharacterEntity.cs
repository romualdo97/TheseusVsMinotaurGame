using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public abstract class CharacterEntity : MonoBehaviour
{
    public bool IsInitialized { get; private set; } = false; // Component is initialized?
    public Vector2Int MazePos { get => m_mazePos; }
    public bool IsMoving { get => m_isMoving; }

    [Header("Animations")]
    [SerializeField]
    protected float m_nodeToNodeDuration = 0.5f;
    protected float m_elapsedTime = 0;
    protected bool m_isMoving = false;
    protected Vector3 m_start;
    protected Vector3 m_next;

    // Other stuff
    protected MazeWorld m_world;
    protected Vector2Int m_mazePos; // Position in the maze

    public virtual void InitEntity()
    {
        m_world = MazeWorld.Instance;
        Assert.IsNotNull(m_world, "World reference is required");

        // Set the inital pos based on level data
        CalculateInitialMazePos();

        m_start = transform.position = MazePosToWorldPos(m_mazePos);
        m_elapsedTime = m_nodeToNodeDuration + 1f;
        m_isMoving = false;
        IsInitialized = true;
    }

    public virtual void UpdateEntity()
    {
        if (!m_isMoving) return;

        // Tween completed...
        if (m_elapsedTime >= m_nodeToNodeDuration)
        {
            m_elapsedTime = 0f;
            m_isMoving = false;
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

            // Increse elapsed time
            m_elapsedTime += Time.deltaTime;
        }
    }

    public void Move(MazeCellEdges dir)
    {
        // Check movement
        if (m_world.MapInfo.IsWallAt(dir, m_mazePos)) return;

        // Start movement
        Vector2Int goal = m_mazePos + MazeGenerator.DIRECTIONS[dir];
        Vector3 worldGoal = MazePosToWorldPos(goal);
        StartMovingAnimation(worldGoal);
    }

    protected virtual void CalculateInitialMazePos()
    {
        m_mazePos = new Vector2Int();
        m_mazePos.x = Random.Range(0, m_world.CurrentLevel.Width);
        m_mazePos.y = Random.Range(0, m_world.CurrentLevel.Height);
    }

    protected Vector3 MazePosToWorldPos(Vector2Int mazePos)
    {
        return new Vector3(mazePos.x + 0.5f, 0.25f, mazePos.y + 0.5f);
    }

    private void StartMovingAnimation(Vector3 newPos)
    {
        // Ignore if already moving
        if (m_isMoving) return;

        // Start the movement animation
        m_start = transform.position;
        m_next = newPos;
        m_isMoving = true;
        m_elapsedTime = 0.0f;
    }
}
