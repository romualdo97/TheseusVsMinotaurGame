using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

[DefaultExecutionOrder(-1)]
public class MazeWorld : MonoBehaviour
{
    // No need for scene lookup because we will assume this component will awake before than any other
    public static MazeWorld Instance { get; private set; }
    private static int s_instanceCount = 0;

    public UnityEvent LevelStarted;

    // Properties
    public MazeLevel CurrentLevel {
        get 
        {
            return m_mazeLevelData; 
        }
        set
        {
            m_mazeLevelData = value;

            // Destroy world
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Setup the world generator
            SetupMazeGenerator();

            // Render the world
            RenderWorld();

            // Invoke event
            LevelStarted?.Invoke();
        }
    }

    public MazeGenerator MapInfo { get => m_mazeGenerator; }

    [Header("Levels")]
    [SerializeField]
    private LevelsList m_levels = null;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject m_cellPrefab = null;

    [SerializeField]
    private GameObject m_exitCellPrefab = null;


    // Internals
    private MazeLevel m_mazeLevelData = null;
    private MazeGenerator m_mazeGenerator = null;

    private void Start()
    {
        // Some assertions
        Assert.IsNotNull(m_levels, "Please set the levels reference");
        Assert.IsNotNull(m_cellPrefab, "Please set the cell prefab");
        Assert.IsNotNull(m_exitCellPrefab, "Please set the exit cell prefab");
        Assert.IsTrue(s_instanceCount == 0, "World was already instanced... you should guarantee only one World in this scene."); Assert.IsTrue(s_instanceCount == 0, "World was already instanced... you should guarantee only one World in this scene.");

        // Init singleton
        Instance = this;
        ++s_instanceCount;

        // Set the current level
        CurrentLevel = m_levels.Levels[0];
    }

    private void SetupMazeGenerator()
    {
        // Generate world
        m_mazeGenerator = new MazeGenerator(m_mazeLevelData.Width, m_mazeLevelData.Height, m_mazeLevelData.Seed, m_mazeLevelData.ChanceOfCycles);
        m_mazeGenerator.GenerateWorld(m_mazeLevelData.Exit, m_mazeLevelData.ExitConnectedEdge);

        // Init the unity seed
        Random.InitState(m_mazeLevelData.Seed);
    }

    private void RenderWorld()
    {
        int mazeWidth = m_mazeLevelData.Width;
        int mazeHeight = m_mazeLevelData.Height;
        for (int i = 0; i < mazeWidth * mazeHeight; ++i)
        {
            // Iterate each cell and check for walls
            Vector2Int cell = new Vector2Int(i % mazeWidth, Mathf.FloorToInt((float)i / mazeWidth));

            // Instantiate
            var cellObject = Instantiate(m_cellPrefab, transform);
            cellObject.transform.localPosition = new Vector3((float)cell.x + 0.5f, 0, (float)cell.y + 0.5f);

            // Hide the walls of the cell
            if (!m_mazeGenerator.IsWallAt(MazeCellEdges.Top, cell))
            {
                cellObject.transform.GetChild((int)MazeCellEdges.Top).gameObject.SetActive(false);
            }
            if (!m_mazeGenerator.IsWallAt(MazeCellEdges.Bottom, cell))
            {
                cellObject.transform.GetChild((int)MazeCellEdges.Bottom).gameObject.SetActive(false);
            }
            if (!m_mazeGenerator.IsWallAt(MazeCellEdges.Left, cell))
            {
                cellObject.transform.GetChild((int)MazeCellEdges.Left).gameObject.SetActive(false);
            }
            if (!m_mazeGenerator.IsWallAt(MazeCellEdges.Right, cell))
            {
                cellObject.transform.GetChild((int)MazeCellEdges.Right).gameObject.SetActive(false);
            }
        }

        // Create the exit cell
        var exitCellObject = Instantiate(m_exitCellPrefab, transform);
        exitCellObject.transform.localPosition = new Vector3((float)m_mazeLevelData.Exit.x + 0.5f, 0, (float)m_mazeLevelData.Exit.y + 0.5f);
    }
}
