using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public class LineHelper
    {
        private float lineWidth;
        private Transform parent;
        private Material lineMaterial;
        private Color color;
        private List<Line> lineList;

        public LineHelper(Transform parent, Material lineMaterial, Color color, float lineWidth = 0.3f)
        {
            this.lineWidth = lineWidth;
            this.parent = parent;
            this.lineMaterial = lineMaterial;
            this.color = color;
        }

        public void DrawLine(Vector3Int start, Vector3Int end)
        {
            GameObject line = new GameObject("Line: " + start.ToString() + " - " + end.ToString());
            line.transform.SetParent(parent);
            line.transform.position = start;
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.sharedMaterial = lineMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineList.Add(new Line(start, end, line));
        }



        public struct Line
        {
            public Vector3Int startPos;
            public Vector3Int endPos;
            public GameObject gameObject;

            public Line(Vector3Int startPos, Vector3Int endPos, GameObject gameObject)
            {
                this.startPos = startPos;
                this.endPos = endPos;
                this.gameObject = gameObject;
            }
        }
    }
}