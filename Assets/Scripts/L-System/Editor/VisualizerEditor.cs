using UnityEngine;
using UnityEditor;

namespace LSystem
{
    [CustomEditor(typeof(Visualizer))]
    public class VisualizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Visualizer visualizer = (Visualizer)target;
            if (GUILayout.Button("Clear Visulization"))
            {
                visualizer.ClearVisulization();
            }
            if (GUILayout.Button("Get Child Count"))
            {
                visualizer.GetChildCount();
            }

            base.OnInspectorGUI();

        }
    }
}