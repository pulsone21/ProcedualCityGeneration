using UnityEngine;
using UnityEditor;


namespace LSystem
{
    [CustomEditor(typeof(LSystemGenerator))]
    public class LSystemGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            LSystemGenerator lSystemGenerator = (LSystemGenerator)target;

            if (GUILayout.Button("Generate Sentence"))
            {
                lSystemGenerator.Generate();
            }
        }
    }
}