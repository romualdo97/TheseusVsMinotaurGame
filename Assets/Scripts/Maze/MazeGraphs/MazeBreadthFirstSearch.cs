using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeBreadthFirstSearch
{
    public delegate void NodeReached(MazeCellNode node);

    public static void TraverseGraph(MazeCellNode start, NodeReached Func)
    {
        Queue<MazeCellNode> frontier = new Queue<MazeCellNode>();
        HashSet<MazeCellNode> visited = new HashSet<MazeCellNode>();

        Func(start);
        frontier.Enqueue(start);
        visited.Add(start);

        // Check frontier elements
        while (frontier.Count > 0)
        {
            MazeCellNode currentNode = frontier.Dequeue();

            // Check if should visit neighbors
            foreach (var nearNode in currentNode.Connections)
            {
                // Ignore if no connection
                if (nearNode == null) continue;

                // If near node is not visited... 
                if (!visited.Contains(nearNode)) // Use the keys for tracking if something was visited before
                {
                    frontier.Enqueue(nearNode); // ... mark for visiting later
                    visited.Add(nearNode);
                    Func(nearNode);
                }
            }
        }
    }

    public static List<MazeCellNode> SearchPath(MazeCellNode start, MazeCellNode end)
    {
        Queue<MazeCellNode> frontier = new Queue<MazeCellNode>();
        Dictionary<MazeCellNode, MazeCellNode> cameFrom = new Dictionary<MazeCellNode, MazeCellNode>();

        frontier.Enqueue(start);
        cameFrom.Add(start, null);

        // Check frontier elements
        while (frontier.Count > 0)
        {
            MazeCellNode currentNode = frontier.Dequeue();

            bool shouldBreak = false;

            // Check if should visit neighbors
            foreach (var nearNode in currentNode.Connections)
            {
                // Ignore if no connection
                if (nearNode == null) continue;

                // If near node is not visited... 
                if (!cameFrom.ContainsKey(nearNode)) // Use the keys for tracking if something was visited before
                {
                    frontier.Enqueue(nearNode); // ... mark for visiting later
                    cameFrom.Add(nearNode, currentNode);

                    // Early return
                    shouldBreak = nearNode == end;
                    if (shouldBreak) break;
                }
            }

            if (shouldBreak) break;
        }

        // Follow the path back to home
        List<MazeCellNode> path = new List<MazeCellNode>();
        MazeCellNode curr = end;
        while (curr != start)
        {
            path.Add(curr);
            curr = cameFrom[curr];
        }

        // Just because the path is inverted...
        path.Reverse();
        return path;
    }
}
