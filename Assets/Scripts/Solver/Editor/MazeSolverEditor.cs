using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeSolver))]
public class MazeSolverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);

        var comp = target as MazeSolver;
        if (GUILayout.Button("Solve"))
        {
            comp.DoSolve();
        }

        EditorGUI.EndDisabledGroup();
    }
}
