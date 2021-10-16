using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputManager : MonoBehaviour
{
    public UnityEvent<MazeCellEdges> ShouldMove;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShouldMove.Invoke(MazeCellEdges.Top);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShouldMove.Invoke(MazeCellEdges.Bottom);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShouldMove.Invoke(MazeCellEdges.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShouldMove.Invoke(MazeCellEdges.Right);
        }
    }
}
