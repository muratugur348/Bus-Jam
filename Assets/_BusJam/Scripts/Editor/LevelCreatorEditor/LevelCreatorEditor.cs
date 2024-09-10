using System;
using BusJam.Scripts.Runtime.LevelCreator;
using System.Collections.Generic;
using System.Linq;
using DG.DemiLib;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using BusJam.Scripts.Runtime;

namespace BusJam.Scripts.Editor.LevelCreatorEditor
{
    [CustomEditor(typeof(LevelCreator))]
    public class LevelCreatorEditor : UnityEditor.Editor
    {
        private LevelCreator _levelCreator;

        private Vector2 directionScrollPosition = Vector2.zero;
        private Vector2 objectTypeScrollPosition = Vector2.zero;
        private Vector2 spawnerGridScrollPos = Vector2.zero;

        private SerializedProperty busColors;

        private void OnEnable()
        {
            _levelCreator = (Runtime.LevelCreator.LevelCreator)target;

            busColors = serializedObject.FindProperty("busColors");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck(); // Start checking for changes
            _levelCreator.GenerateLevel();
            DrawGridProperties();

            if (!IsLevelDataAvailable())
            {
                EditorGUILayout.HelpBox("Please regenerate the Grid!", MessageType.Error);
                return;
            }

            DrawGrid();
            DrawSpawnerGrids();


            DrawSaveLoadButtons();
            if (EditorGUI.EndChangeCheck()) // Check if the value has changed
            {
                Undo.RecordObject(_levelCreator, "Change Level Index"); // Record the change for undo
                //  _levelCreator.levelIndex = newLevelIndex; // Apply the new value
                EditorUtility.SetDirty(_levelCreator); // Mark the object as dirty

                _levelCreator.GetLevelData().BusColors = _levelCreator.busColors;

                var busManager = FindObjectOfType<BusManager>();
                busManager.busColors = _levelCreator.busColors;
                EditorUtility.SetDirty(busManager);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsLevelDataAvailable()
        {
            // TODO: Implement Custom Checks
            var isLevelDataExist = _levelCreator.GetLevelData() != null;
            if (!isLevelDataExist)
                return false;

            var isLevelGridExist = _levelCreator.GetLevelData().GetGrid() != null;
            if (!isLevelGridExist)
            {
                _levelCreator.LoadLevel();
                return false;
            }

            var isGridBoundsCorrect = (_levelCreator.gridWidth * _levelCreator.gridHeight) ==
                                      _levelCreator.GetLevelData().GetGrid().Length;
            return isLevelDataExist && isGridBoundsCorrect;
        }

        private void DrawGridProperties()
        {
            EditorGUILayout.BeginVertical();
            // Display the enum list using Unity's default list GUI
            EditorGUILayout.PropertyField(busColors, true);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            GUILayout.Label("Directions", EditorStyles.boldLabel);
            DisplayEnumButtons<GridDirectionType>(_levelCreator, ref directionScrollPosition);
            EditorGUILayout.Space(10);
            GUILayout.Label("Color Type", EditorStyles.boldLabel);
            DisplayEnumButtons<ColorType>(_levelCreator, ref objectTypeScrollPosition);
            EditorGUILayout.Space(10);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Width");
            _levelCreator.gridWidth = EditorGUILayout.IntField(_levelCreator.gridWidth);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Height");
            _levelCreator.gridHeight = EditorGUILayout.IntField(_levelCreator.gridHeight);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Index");
            _levelCreator.levelIndex = EditorGUILayout.IntField(_levelCreator.levelIndex);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Match Area Size");
            _levelCreator.matchAreaSize = EditorGUILayout.IntField(_levelCreator.matchAreaSize);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Grid"))
            {
                _levelCreator.SpawnGrid();
            }

            if (GUILayout.Button("Reset"))
            {
                _levelCreator.ResetLevel();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button) { fontSize = 10 };
            var grid = _levelCreator.GetLevelData().GetGrid();
            if (ReferenceEquals(grid, null) || grid.Length.Equals(0))
            {
                return;
            }

            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);


            // Temporary colors for the subdivisions

            for (int y = _levelCreator.gridHeight - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                for (int x = 0; x < _levelCreator.gridWidth; x++)
                {
                    /*Debug.Log(_levelCreator.GetLevelData().GridCells.Length + " x ");
                    Debug.Log(_levelCreator.GetLevelData().Width + " w ");
                    Debug.Log(_levelCreator.GetLevelData().Height + " h ");*/
                    var cell = _levelCreator.GetLevelData().GetGridCell(x, y);

                    var cellText = $"{x}x{y}" + "\n" + cell.colorType;



                    // Set background color based on the object type
                    if (!ReferenceEquals(cell, null) && cell.colorType.Equals(ColorType.Wall))
                    {
                        GUI.backgroundColor = Color.black;
                    }
                    if (!ReferenceEquals(cell, null) && cell.colorType.Equals(ColorType.Spawner))
                    {
                        cellText += "\n" + cell.mySpawnerData.myDirection;
                        GUI.backgroundColor = Color.white;
                    }
                    else if (!ReferenceEquals(cell, null) && cell.colorType.Equals(ColorType.None))
                    {
                        GUI.backgroundColor = Color.gray;
                    }
                    else
                    {
                        GUI.backgroundColor = _levelCreator.gameColors.ActiveColors[((int)cell.colorType - 2) % Enum.GetValues(typeof(ColorType)).Length];
                    }

                    if (cell.isQuestionMark)
                        cellText += "\n?";



                    Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(cellText), style,
                        GUILayout.Width(70),
                        GUILayout.Height(70));

                    // Check for right-click
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1 &&
                        buttonRect.Contains(Event.current.mousePosition))
                    {
                        _levelCreator.GridRemoveButtonAction(x, y);
                        Event.current.Use(); // Consume the event
                    }

                    if (GUI.Button(buttonRect, cellText, style))
                    {
                        _levelCreator.GridButtonAction(x, y);
                    }

                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space();
        }

        private void DrawSpawnerGrids()
        {
            var levelData = _levelCreator.GetLevelData();

            for (int y = _levelCreator.gridHeight - 1; y >= 0; y--)
            {
                for (int x = 0; x < _levelCreator.gridWidth; x++)
                {
                    if (levelData.GetGridCell(x, y).colorType == ColorType.Spawner)
                    {
                        DrawSpawnerQueueGrid(x, y);
                    }
                }
            }
        }


        private void DrawSpawnerQueueGrid(int x, int y)
        {
            EditorGUILayout.Space(30);
            var levelData = _levelCreator.GetLevelData();

            var spawnCells = levelData.GetGridCell(x, y).mySpawnerData.stickmanInSpawner;
            int count = spawnCells.Count;

            string labelName = "Spawner Queue Of " + x + " x " + y;

            GUIStyle style = new GUIStyle(GUI.skin.button) { fontSize = 12 };
            EditorGUILayout.LabelField(labelName, EditorStyles.boldLabel);

            EditorGUILayout.LabelField("-------------------------------------------------------------------------");
            EditorGUILayout.Space(20);

            (bool changed, int newCount) =
                DrawPlusMinusButton(count, labelName); // End checking for changes and get if there was any change
            if (changed)
            {
                count = newCount;
                _levelCreator.ResetSpawnerCellLists(x, y, count);
                spawnCells = levelData.GetGridCell(x, y).mySpawnerData.stickmanInSpawner;
            }

            if (ReferenceEquals(spawnCells, null) || spawnCells.Count.Equals(0))
            {
                return;
            }

            spawnerGridScrollPos = EditorGUILayout.BeginScrollView(spawnerGridScrollPos,
                GUILayout.Height(120)); // Create a scroll view for all targets

            EditorGUILayout.BeginHorizontal();

            for (int i = spawnCells.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginVertical(GUILayout
                    .Width(75)); // Begin a new vertical group for each cell with fixed width

                var cellText = x + "x" + y;
                var cell = spawnCells[i];
                //  Debug.Log(cell.X + " x " + cell.Y);

                cellText = cell.colorType.ToString();

                if (!ReferenceEquals(cell, null) && spawnCells[i].colorType != ColorType.None)
                {
                    GUI.backgroundColor = _levelCreator.gameColors.ActiveColors[((int)spawnCells[i].colorType - 2) % Enum.GetValues(typeof(ColorType)).Length];
                }
                else
                {
                    GUI.backgroundColor = Color.gray;
                }


                Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(cellText), style,
                    GUILayout.Width(50),
                    GUILayout.Height(50));


                // Check for right-click
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 &&
                    buttonRect.Contains(Event.current.mousePosition))
                {
                    _levelCreator.SpawnerQueueRemoveButtonAction(x, y, i);
                    Event.current.Use(); // Consume the event
                }

                if (GUI.Button(buttonRect, cellText, style))
                {
                    _levelCreator.SpawnerQueueButtonAction(x, y, i);
                }


                // Add index number below each cell, center-aligned
                GUILayout.Label(i.ToString(), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

                EditorGUILayout.EndVertical(); // End the vertical group for each cell
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView(); // End the scroll view for all targets
        }


        private void DrawSaveLoadButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Don't forget the save grid!", MessageType.Warning);
            EditorGUILayout.LabelField("Save/Load", EditorStyles.boldLabel);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("Save"))
            {
                _levelCreator.SaveLevel();
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Load"))
            {
                _levelCreator.LoadLevel();
            }

            EditorGUILayout.EndHorizontal();
        }

        private (bool, int) DrawPlusMinusButton(int count, string labelName)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelName);
            EditorGUILayout.IntField(count);

            if (GUILayout.Button("-", GUILayout.Width(40)))
            {
                count = Mathf.Max(0, count - 1);
            }

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                count++;
            }

            EditorGUILayout.EndHorizontal();
            return (EditorGUI.EndChangeCheck(), count); // End checking for changes and get if there was any change
        }

        private void DisplayEnumButtons<T>(LevelCreator _levelCreator, ref Vector2 scrollPosition) where T : System.Enum
        {
            T[] enumValues = (T[])System.Enum.GetValues(typeof(T));
            float
                scrollViewHeight =
                    50; // Mathf.Min(enumValues.Length * 2, 100); // Adjust button height and max height as needed

            // Begin a scroll view with a vertical scrollbar
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollViewHeight));

            GUILayout.BeginHorizontal();

            foreach (T value in enumValues)
            {
                string buttonLabel = value.ToString();

                if (GUILayout.Button(buttonLabel))
                {
                    if (typeof(T) == typeof(ColorType))
                    {
                        _levelCreator.SetColorEnum((ColorType)(object)value);
                    }
                    else if (typeof(T) == typeof(GridDirectionType))
                    {
                        _levelCreator.SetDirectionEnum((GridDirectionType)(object)value);
                    }
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

    }
}