//#define BREADTH_FIRST_CYCLES_GENERATOR // TODO: This is not working correctly
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate data for a maze level and some utitility functions for navigation.
/// </summary>
public class MazeGenerator
{
    // Simple table of direction vectors
    private readonly Dictionary<MazeCellEdges, Vector2Int> DIRECTIONS = new Dictionary<MazeCellEdges, Vector2Int>()
    {
        { MazeCellEdges.Top, Vector2Int.up },
        { MazeCellEdges.Bottom, Vector2Int.down },
        { MazeCellEdges.Left, Vector2Int.left },
        { MazeCellEdges.Right, Vector2Int.right }
    };

    private int m_width;
    private int m_height;
    private float m_chanceOfCycles;
    private MazeGraph m_graph;
    private System.Random m_rng; // Intentionally not using UnityEngine.Random

    public MazeGenerator(int width, int height, int seed = -1, float chanceOfCycles = 0)
    {
        m_width = width;
        m_height = height;
        m_chanceOfCycles = chanceOfCycles;
        m_graph = new MazeGraph(width * height);
        
        // -1 means random seed
        if (seed == -1) m_rng = new System.Random();
        else m_rng = new System.Random(seed);
    }

    public void GenerateWorld()
    {
        // 1. Let's use some recursive backtracker for generating the maze nodes
        // this will generate a spanning tree (which is dual graph of the rectangular grid graph)...
        Vector2Int startCoord = new Vector2Int(m_rng.Next() % m_width, m_rng.Next() % m_height);
        RemoveWall(startCoord);

        // 2. Then we randomly add cell connections so our maze is not only a tree of corridors but a maze with some rooms
        // (add some cyclic connections to the maze graph) like in minotaur demo... https://estivalet.github.io/theseus-minotaur/
        CreateCycles();
    }

    public bool IsWallAt(MazeCellEdges cellEdge, Vector2Int cellCoord)
    {
        bool isValid = cellCoord.x >= 0 && cellCoord.x < m_width && cellCoord.y >= 0 && cellCoord.y < m_height;
        if (!isValid) return false;

        int cellIndex = cellCoord.x + m_width * cellCoord.y;
        return m_graph[cellIndex].Connections[(int)cellEdge] == null;
    }

    public bool SearchPath(Vector2Int start, Vector2Int end, out List<MazeCellNode> path)
    {
        // Check if valid input
        bool startIsValid = start.x >= 0 && start.x < m_width && start.y >= 0 && start.y < m_height;
        if (startIsValid)
        {
            bool endIsValid = end.x >= 0 && end.x < m_width && end.y >= 0 && end.y < m_height;
            if (!endIsValid)
            {
                path = null;
                return false;
            }
        }
        else
        {
            path = null;
            return false;
        }

        // Get start and end node
        int startIndex = start.x + m_width * start.y;
        int endIndex = end.x + m_width * end.y;
        MazeCellNode startNode = m_graph[startIndex];
        MazeCellNode endNode = m_graph[endIndex];

        // Use the search path algorithm
        path = MazeBreadthFirstSearch.SearchPath(startNode, endNode);
        return true;
    }

    public List<Vector2Int> CalculateCoordsPath(List<MazeCellNode> graphPath)
    {
        // Convert the graph path to a coords path
        var path = new List<Vector2Int>(graphPath.Count);
        for (int i = 0; i < graphPath.Count; ++i)
        {
            var node = graphPath[i];
            Vector2Int coord = new Vector2Int(node.Id % m_width, Mathf.FloorToInt((float)node.Id / m_width));
            path.Add(coord);
        }

        return path;
    }

    private void RemoveWall(Vector2Int cellCord)
    {
        // Use the shuffle toy
        var directions = new List<MazeCellEdges>(4) { MazeCellEdges.Top, MazeCellEdges.Bottom, MazeCellEdges.Left, MazeCellEdges.Right };
        directions.Shuffle(m_rng);

        // Start the algorithm
        for (int i = 0; i < directions.Count; ++i)
        {
            MazeCellEdges direction = directions[i];
            Vector2Int newCoord = cellCord + DIRECTIONS[direction];

            // Check if new coord is inside the grid
            bool isValid = newCoord.x >= 0 && newCoord.x < m_width && newCoord.y >= 0 && newCoord.y < m_height;
            if (!isValid) continue;

            var newCellIndex = newCoord.x + m_width * newCoord.y;
            var cellIndex = cellCord.x + m_width * cellCord.y;

            // Check if already visited
            bool isVisited = m_graph[newCellIndex].HasConnections;
            if (isVisited) continue;

            // Connect nodes
            m_graph[cellIndex].Connect(direction, m_graph[newCellIndex]);

            // Repeat recursively
            RemoveWall(newCoord);
        }
    }

    private void CreateCycles()
    {
#if BREADTH_FIRST_CYCLES_GENERATOR
        CreateCyclesUsingBreadthSearch();
#else
        CreateCyclesUsingRandomHoleAtGrid();
#endif
    }

    private void CreateCyclesUsingBreadthSearch()
    {
        if (m_chanceOfCycles <= 0) return;

        // Create func
        MazeBreadthFirstSearch.NodeReached DoWhenNodeReach = (mazeNode) =>
        {
            // Use the shuffle toy
            var directions = new List<MazeCellEdges>(4) { MazeCellEdges.Top, MazeCellEdges.Bottom, MazeCellEdges.Left, MazeCellEdges.Right };
            directions.Shuffle(m_rng);

            // Try to connect to adjacent cells
            for (int i = 0; i < directions.Count; ++i)
            {
                MazeCellEdges direction = directions[i];
                var potentialNode = mazeNode.Connections[(int)direction];
                bool canConnect = potentialNode == null;
                if (canConnect)
                {
                    Vector2Int currCoord = new Vector2Int(i % m_width, Mathf.FloorToInt((float)i / m_width));
                    Vector2Int newCoord = currCoord + DIRECTIONS[direction];

                    bool isValid = newCoord.x >= 0 && newCoord.x < m_width && newCoord.y >= 0 && newCoord.y < m_height;
                    if (!isValid) continue;

                    mazeNode.Connect(direction, m_graph[newCoord.x + m_width * newCoord.y]);
                    break; // Advance to next node
                }
            }
        };

        // Traverse the graph
        int graphOrder = m_width * m_height;
        MazeBreadthFirstSearch.TraverseGraph(m_graph[m_rng.Next() % graphOrder], DoWhenNodeReach);
    }

    private void CreateCyclesUsingRandomHoleAtGrid()
    {
        if (m_chanceOfCycles <= 0) return;

        for (int i = 0; i < m_width * m_height; ++i)
        {
            bool shouldAddAdditionalConnection = m_rng.NextDouble() % 1d <= m_chanceOfCycles;
            if (shouldAddAdditionalConnection)
            {
                // Use the shuffle toy
                var directions = new List<MazeCellEdges>(4) { MazeCellEdges.Top, MazeCellEdges.Bottom, MazeCellEdges.Left, MazeCellEdges.Right };
                directions.Shuffle(m_rng);

                MazeCellNode node = m_graph[i];
                for (int j = 0; j < directions.Count; ++j)
                {
                    MazeCellEdges direction = directions[j];
                    var potentialNode = node.Connections[(int)direction];
                    bool canConnect = potentialNode == null;
                    if (canConnect)
                    {
                        Vector2Int currCoord = new Vector2Int(i % m_width, Mathf.FloorToInt((float)i / m_width));
                        Vector2Int newCoord = currCoord + DIRECTIONS[direction];

                        bool isValid = newCoord.x >= 0 && newCoord.x < m_width && newCoord.y >= 0 && newCoord.y < m_height;
                        if (!isValid) continue;

                        node.Connect(direction, m_graph[newCoord.x + m_width * newCoord.y]);
                        break; // Advance to next node
                    }
                }
            }
        }
    }
}
