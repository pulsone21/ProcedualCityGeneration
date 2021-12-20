using UnityEngine;
using UnityEditor;

namespace LSystem
{
    [CustomEditor(typeof(DebugVisualizer))]
    public class DebugVisualizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DebugVisualizer visualizer = (DebugVisualizer)target;
            if (GUILayout.Button("Clear Visulization"))
            {
                visualizer.ClearVisulization();
            }
            base.OnInspectorGUI();

        }
    }
}