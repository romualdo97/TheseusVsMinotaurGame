using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public abstract class CharacterEntity : MonoBehaviour
{
    public Vector2Int MazePos { get => m_mazePos; }

    [Header("Animations")]
    [SerializeField]
    protected float m_nodeToNodeDuration = 0.5f;
    protected float m_elapsedTime = 0;
    protected bool m_isTraversing = false;
    protected Vector3 m_start;
    protected Vector3 m_next;

    // Other stuff
    protected MazeWorld m_world;
    protected Vector2Int m_mazePos; // Position in the maze

    protected virtual void InitEntity()
    {
        m_world = MazeWorld.Instance;
        Assert.IsNotNull(m_world, "World reference is required");

        // Set the inital pos based on level data
        CalculateInitialMazePos();

        m_start = transform.position = MazePosToWorldPos(m_mazePos);
        m_elapsedTime = m_nodeToNodeDuration + 1f;
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
}
