using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MazeWorld : MonoBehaviour
{
    // Properties
    public MazeLevel CurrentLevel {
        get 
        {
            return m_mazeLevelData; 
        }
        set
        {
            m_mazeLevelData = value;
            // TODO: Manage new level generation
        }
    }

    public MazeGenerator MapInfo { get => m_mazeGenerator; }

    [Header("Data")]
    [SerializeField]
    private MazeLevel m_mazeLevelData = null;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject m_cellPrefab = null;

    // Internals
    private MazeGenerator m_mazeGenerator = null;

    private void Start()
    {
        // Some assertions
        Assert.IsNotNull(m_mazeLevelData, "Please define the first level data");
        Assert.IsNotNull(m_cellPrefab, "Please define the cell prefab");

        // Setup the world generator
        SetupMazeGenerator();

        // Render the world
        RenderWorld();
    }

    private void SetupMazeGenerator()
    {
        m_mazeGenerator = new MazeGenerator(m_mazeLevelData.Width, m_mazeLevelData.Height, m_mazeLevelData.Seed, m_mazeLevelData.ChanceOfCycles);
        m_mazeGenerator.GenerateWorld();
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
    }
}
