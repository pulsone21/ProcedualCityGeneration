using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public class LineHelper
    {
        public enum LineDirection { vertical, horizontal }
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
            this.lineList = new List<Line>();
        }

        public void DrawLine(Vector3Int start, Vector3Int end)
        {
            GameObject line = new GameObject("Line: " + start.ToString() + " - " + end.ToString());
            line.transform.SetParent(parent);
            line.transform.position = start;
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.generateLightingData = true;
            lineRenderer.sharedMaterial = lineMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineList.Add(new Line(start, end, line, CalculateLineDirection(start, end)));
        }

        private Direction CalculateLineDirection(Vector3Int startPos, Vector3Int endPos)
        {
            Direction newDirection;
            if (startPos.x > endPos.x)
            {
                newDirection = Direction.left;
            }
            else if (startPos.x < endPos.x)
            {
                newDirection = Direction.right;
            }
            else if (startPos.y > endPos.y)
            {
                newDirection = Direction.down;
            }
            else
            {
                newDirection = Direction.up;
            }
            return newDirection;
        }

        public List<Line> GetVerticalLines()
        {
            List<Line> lines = new List<Line>();

            foreach (Line line in lineList)
            {
                if (line.lineDirection == LineDirection.vertical)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        public List<Line> GetHorizontalLines()
        {
            List<Line> lines = new List<Line>();

            foreach (Line line in lineList)
            {
                if (line.lineDirection == LineDirection.horizontal)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        public void MergeAlignedLines(List<Line> lines)
        {
            List<Line> linesToDelete = new List<Line>();
            foreach (Line line in lines)
            {
                List<Vector3Int> positions = new List<Vector3Int>();
                switch (line.lineDirection)
                {
                    case LineDirection.horizontal:
                        positions.Add(new Vector3Int(line.endPos.x + 1, 0, line.endPos.z));
                        positions.Add(new Vector3Int(line.endPos.x - 1, 0, line.endPos.z));
                        break;
                    case LineDirection.vertical:
                        positions.Add(new Vector3Int(line.endPos.x, 0, line.endPos.z - 1));
                        positions.Add(new Vector3Int(line.endPos.x, 0, line.endPos.z + 1));
                        break;
                }
                List<Line> linesToMerge = new List<Line>();
                foreach (Line l in lines)
                {
                    foreach (Vector3Int pos in positions)
                    {
                        if (l.IsPositionInTheLine(pos))
                        {
                            linesToMerge.Add(l);
                        };
                    }
                }
                foreach (Line lineToMerge in linesToMerge)
                {
                    if (line.IsPositionInTheLine(lineToMerge.startPos))
                    {
                        //startPoint is in the current Line -> ENDPOINT MUSST be out side of the line 
                        if (Vector3Int.Distance(line.startPos, lineToMerge.endPos) > Vector3Int.Distance(line.endPos, lineToMerge.endPos))
                        {
                            DrawLine(line.startPos, lineToMerge.endPos);
                        }
                        else
                        {
                            DrawLine(line.endPos, lineToMerge.endPos);
                        }
                    }
                    else
                    {
                        if (Vector3Int.Distance(line.startPos, lineToMerge.startPos) > Vector3Int.Distance(line.endPos, lineToMerge.startPos))
                        {
                            DrawLine(line.startPos, lineToMerge.startPos);
                        }
                        else
                        {
                            DrawLine(line.endPos, lineToMerge.startPos);
                        }
                    }
                    linesToDelete.Add(lineToMerge);
                }
                linesToDelete.Add(line);
            }

            foreach (Line line in linesToDelete)
            {
                GameObject.DestroyImmediate(line.gameObject);
                lineList.Remove(line);
            }
        }

        private List<Line> CheckIfPositionHasLine(Vector3Int pos)
        {
            List<Line> lines = new List<Line>();
            foreach (Line line in lineList)
            {
                if (line.IsPositionInTheLine(pos))
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        // private Line GetLineWithEndPos(Vector3Int pos){
        //     Line outLine;
        //     foreach (Line line in lineList)
        //     {
        //         if(line.endPos == pos){
        //             outLine = line;
        //         }
        //     }
        //     return outLine;
        // }

        public struct Line
        {
            public Vector3Int startPos;
            public Vector3Int endPos;
            public GameObject gameObject;
            public Direction direction;
            public LineDirection lineDirection;
            private int lineLength;
            readonly List<Vector3Int> linePoints;

            public Line(Vector3Int startPos, Vector3Int endPos, GameObject gameObject, Direction newDirection)
            {
                this.startPos = startPos;
                this.endPos = endPos;
                this.gameObject = gameObject;
                this.lineLength = Mathf.RoundToInt(Vector3Int.Distance(startPos, endPos));
                this.linePoints = new List<Vector3Int>();
                this.direction = newDirection;
                for (int i = 0; i < lineLength; i++)
                {
                    int x = (Mathf.Abs(startPos.x - endPos.x) / lineLength) * i + endPos.x;
                    int z = (Mathf.Abs(startPos.z - endPos.z) / lineLength) * i + endPos.z;
                    linePoints.Add(new Vector3Int(x, 0, z));
                }
                this.lineDirection = direction == Direction.right || direction == Direction.left ? LineDirection.horizontal : LineDirection.vertical;
            }

            public bool IsPositionInTheLine(Vector3Int position)
            {
                foreach (Vector3Int linePos in linePoints)
                {
                    if (linePos == position) return true;
                }
                return false;
            }
        }
    }
}