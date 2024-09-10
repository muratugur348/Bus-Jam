using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        GridManager gridManager = (GridManager)target;


        if (gridManager.gridList != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50); // Space for y-axis labels
            for (int x = 0; x < gridManager.width; x++)
            {
                // Display x-axis labels
                EditorGUILayout.LabelField(x.ToString(), GUILayout.Width(30));
            }
            EditorGUILayout.EndHorizontal();

            for (int y = gridManager.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();

                // Display y-axis label
                EditorGUILayout.LabelField(y.ToString(), GUILayout.Width(40));

                for (int x = 0; x < gridManager.width; x++)

                {
                    gridManager.gridList[x].list[y] = EditorGUILayout.IntField(gridManager.gridList[x].list[y], GUILayout.Width(30));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /*  if (gridManager.gridObjectList != null && gridManager.gridObjectList.Count > 0)
          {

              EditorGUILayout.BeginHorizontal();
              GUILayout.Space(50); // Space for y-axis labels

              for (int x = 0; x < gridManager.gridObjectList[0].list.Count; x++)
              {
                  // Display x-axis labels
                  EditorGUILayout.LabelField(x.ToString(), GUILayout.Width(30));
              }
              EditorGUILayout.EndHorizontal();

              for (int y = 0; y < gridManager.gridObjectList.Count; y++)
              {
                  EditorGUILayout.BeginHorizontal();

                  // Display y-axis label
                  EditorGUILayout.LabelField(y.ToString(), GUILayout.Width(40));

                  for (int x = 0; x < gridManager.gridObjectList[y].list.Count; x++)
                  {
                      EditorGUILayout.IntField((gridManager.gridObjectList[y].list[x] == null ? 0 : 1)
                           , GUILayout.Width(30));
                  }
                  EditorGUILayout.EndHorizontal();
              }
          }*/
        // Save changes and repaint if necessary
        if (GUI.changed)
        {
            EditorUtility.SetDirty(gridManager);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
