using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class PriorityIntersectionWindow : IntersectionWindowBase
    {

        private PriorityIntersectionSettings selectedPriorityIntersection;
        private float scrollAdjustment = 187;



        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            selectedIntersection = SettingsWindow.GetSelectedIntersection();
            selectedPriorityIntersection = selectedIntersection as PriorityIntersectionSettings;
            stopWaypoints = selectedPriorityIntersection.enterWaypoints;
            exitWaypoints = selectedPriorityIntersection.exitWaypoints;
            return base.Initialize(windowProperties, window);
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            Color oldColor;
            if (!addExitWaypoints)
            {
                DrawStopWaypointButtons(false);
            }
            if (!addWaypoints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Exit waypoints:","When a vehicle touches an exit waypoint, it is no longer considered inside intersection.\n" +
                    "For every lane that exits the intersection a single exit point should be marked"));
                EditorGUILayout.Space();

                for (int i = 0; i < exitWaypoints.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    exitWaypoints[i] = (WaypointSettings)EditorGUILayout.ObjectField(exitWaypoints[i], typeof(WaypointSettings), true);

                    oldColor = GUI.backgroundColor;
                    if (exitWaypoints[i].draw == true)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View"))
                    {
                        ViewWaypoint(exitWaypoints[i], i);
                    }
                    GUI.backgroundColor = oldColor;


                    if (GUILayout.Button("Delete"))
                    {
                        exitWaypoints[i].exit = false;
                        exitWaypoints.RemoveAt(i);
                        SceneView.RepaintAll();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (!addExitWaypoints)
                {
                    if (GUILayout.Button("Add Exit Waypoints"))
                    {
                        AddExitWaypoints();
                    }
                }
                oldColor = GUI.backgroundColor;
                if (intersectionSave.showExit == true)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View Exit Waypoints"))
                {
                    ViewAll();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();

                if (addExitWaypoints)
                {
                    if (GUILayout.Button("Done"))
                    {
                        Cancel();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();

        }


        private void ViewWaypoint(WaypointSettings waypoint, int index)
        {
            waypoint.draw = !waypoint.draw;
            if (waypoint.draw == false)
            {
                exitWaypoints[index].draw = false;
                intersectionSave.showExit = false;
            }
            SceneView.RepaintAll();
        }

        private void Cancel()
        {
            addExitWaypoints = false;
            SceneView.RepaintAll();
        }
    }
}
