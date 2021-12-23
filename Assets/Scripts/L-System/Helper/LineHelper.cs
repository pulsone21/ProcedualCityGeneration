using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public class LineHelper : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float lineWidth;
        [SerializeField] private Transform parent;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private Color color;
        [SerializeField] private List<Line> lineList = new List<Line>();

        #endregion

        #region Public Functions

        public List<Line> GetAllLines()
        {
            return lineList;
        }

        public void DrawLine(Vector3Int start, Vector3Int end)
        {
            GameObject lineGo = new GameObject("Line: " + start.ToString() + " - " + end.ToString());
            lineGo.transform.SetParent(parent);
            lineGo.transform.position = start;
            LineRenderer lineRenderer = lineGo.AddComponent<LineRenderer>();
            lineRenderer.generateLightingData = true;
            lineRenderer.sharedMaterial = lineMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            Line line = new Line(start, end, lineGo, CalculateLineDirection(start, end));
            lineList.Add(line);
        }

        public bool FixLines()
        {
            MergeAlignedLines(Line.LineDirection.horizontal);
            MergeAlignedLines(Line.LineDirection.vertical);
            CleanUpNeighboredLinesInRadius(Line.LineDirection.horizontal);
            CleanUpNeighboredLinesInRadius(Line.LineDirection.vertical);
            CalculateIntersections(lineList);
            //TODO Delete parallel lines 

            CleanUpParalleLines(Line.LineDirection.horizontal);
            CleanUpParalleLines(Line.LineDirection.vertical);
            CalculateIntersections(lineList);
            ClearLinesWithoutIntersections(lineList);
            return true;
        }

        public void ClearLineList()
        {
            lineList.Clear();
        }


        #endregion

        #region Main Functions

        private void ClearLinesWithoutIntersections(List<Line> lineList)
        {
            List<Line> linesToDelete = new List<Line>();
            foreach (Line line in lineList)
            {
                if (line.intersectionCount == 0)
                {
                    linesToDelete.Add(line);
                }
            }
            foreach (Line line in linesToDelete)
            {
                DestroyImmediate(line.gameObject);
                lineList.Remove(line);
            }
        }

        private void MergeAlignedLines(Line.LineDirection lineDirection)
        {
            List<Line> lines = GetLinesByDirection(lineDirection);
            bool needMerging = true;
            int mergingIteration = 0;
            while (needMerging && mergingIteration < 5)
            {
                needMerging = false;
                HashSet<Line> linesToDelete = new HashSet<Line>();
                foreach (Line line in lines)
                {
                    if (!linesToDelete.Contains(line))
                    {
                        HashSet<Line> linesToMerge = new HashSet<Line>();
                        List<Vector3Int> positions = new List<Vector3Int>();
                        switch (line.lineDirection)
                        {
                            case Line.LineDirection.horizontal:
                                positions.Add(new Vector3Int(line.endPos.x + 1, 0, line.endPos.z));
                                positions.Add(new Vector3Int(line.endPos.x - 1, 0, line.endPos.z));
                                positions.Add(new Vector3Int(line.startPos.x + 1, 0, line.startPos.z));
                                positions.Add(new Vector3Int(line.startPos.x - 1, 0, line.startPos.z));
                                break;
                            case Line.LineDirection.vertical:
                                positions.Add(new Vector3Int(line.endPos.x, 0, line.endPos.z - 1));
                                positions.Add(new Vector3Int(line.endPos.x, 0, line.endPos.z + 1));
                                positions.Add(new Vector3Int(line.startPos.x, 0, line.startPos.z + 1));
                                positions.Add(new Vector3Int(line.startPos.x, 0, line.startPos.z - 1));
                                break;
                        }
                        foreach (Line l in lines)
                        {
                            if (l != line && !linesToDelete.Contains(l))
                            {
                                foreach (Vector3Int pos in positions)
                                {
                                    if (l.IsPositionInTheLine(pos))
                                    {
                                        linesToMerge.Add(l);
                                    };
                                }
                            }
                        }
                        if (linesToMerge.Count > 0)
                        {
                            foreach (Line lineToMerge in linesToMerge)
                            {
                                MergeLines(line, lineToMerge);
                                linesToDelete.Add(lineToMerge);
                            }
                            linesToDelete.Add(line);
                        }
                    }
                }

                if (linesToDelete.Count > 0)
                {
                    foreach (Line line in linesToDelete)
                    {
                        DestroyImmediate(line.gameObject);
                        lineList.Remove(line);
                    }
                    lines = GetLinesByDirection(lineDirection);
                    needMerging = true;
                    mergingIteration++;
                }
            }
        }

        private void CleanUpNeighboredLinesInRadius(Line.LineDirection lineDirection, int radius = 2)
        {
            List<Line> linesToMerge = GetLinesByDirection(lineDirection);
            bool mergedLines = true;
            while (mergedLines)
            {
                mergedLines = false;
                List<Line> generatedLineList = new List<Line>();
                List<Line> allreadyLookedUpLine = new List<Line>();
                foreach (Line l in linesToMerge)
                {
                    allreadyLookedUpLine.Add(l);
                    if (!generatedLineList.Contains(l))
                    {
                        foreach (Vector3Int pos in l.GetAllLinePoints())
                        {
                            for (int i = -radius; i < radius; i++)
                            {
                                if (i == 0) continue;
                                foreach (Line line in lineList)
                                {
                                    if (line.lineDirection == lineDirection && !allreadyLookedUpLine.Contains(line))
                                    {
                                        if (line.IsPositionInTheLine(GetVector3IntWithOffset(lineDirection, pos, i)))
                                        {
                                            if (line.lineLength > l.lineLength)
                                            {
                                                if (!generatedLineList.Contains(l)) generatedLineList.Add(l);
                                            }
                                            else
                                            {
                                                if (!generatedLineList.Contains(line)) generatedLineList.Add(line);
                                            }

                                        }
                                    }

                                }
                            }
                        }
                    }

                }
                if (generatedLineList.Count > 0)
                {
                    Debug.Log($"GeneratedList Contains {generatedLineList.Count}, LineList Contains: {lineList.Count}");
                    foreach (Line gl in generatedLineList)
                    {
                        DestroyImmediate(gl.gameObject);
                        lineList.Remove(gl);
                    }
                    mergedLines = true;
                    linesToMerge = GetLinesByDirection(lineDirection);
                    generatedLineList.Clear();
                    allreadyLookedUpLine.Clear();
                }
            }
        }

        private void CleanUpParalleLines(Line.LineDirection lineDirection, int radius = 1)
        {
            List<Line> lines = GetLinesByDirection(lineDirection);
            Line.LineDirection oppsiteDirection = Line.LineDirection.vertical;
            if (lineDirection == oppsiteDirection)
            {
                oppsiteDirection = Line.LineDirection.horizontal;
            }
            if (lines[0].intersectionsCalculated)//? checking if we have calculated the intersections
            {
                bool needToCheck = true;
                while (needToCheck)
                {
                    needToCheck = false;
                    HashSet<Line> linesToCleanUp = new HashSet<Line>();
                    HashSet<Line> allreadyLookedUpLine = new HashSet<Line>();
                    foreach (Line l in lines)
                    {
                        foreach (Vector3Int pos in l.GetAllLinePoints())
                        {
                            for (int i = -radius; i < radius; i++)
                            {
                                if (i == 0) continue;
                                foreach (Line line in lines)
                                {
                                    if (!allreadyLookedUpLine.Contains(line))
                                    {
                                        if (line.IsPositionInTheLine(GetVector3IntWithOffset(oppsiteDirection, pos, i)))
                                        {
                                            if (line.intersectionCount > l.intersectionCount)
                                            {
                                                linesToCleanUp.Add(l);
                                            }
                                            else
                                            {
                                                linesToCleanUp.Add(line);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        allreadyLookedUpLine.Add(l);
                    }
                    if (linesToCleanUp.Count > 0)
                    {
                        foreach (Line gl in linesToCleanUp)
                        {
                            DestroyImmediate(gl.gameObject);
                            lineList.Remove(gl);
                        }
                        needToCheck = true;
                        lines = GetLinesByDirection(lineDirection);
                        linesToCleanUp.Clear();
                        allreadyLookedUpLine.Clear();
                    }
                }
            }
        }

        #endregion

        #region Helper Functions

        public List<Line> GetLinesByDirection(Line.LineDirection lineDirection)
        {
            List<Line> linesToMerge = new List<Line>();
            switch (lineDirection)
            {
                case Line.LineDirection.vertical:
                    linesToMerge = GetVerticalLines(lineList);
                    break;
                case Line.LineDirection.horizontal:
                    linesToMerge = GetHorizontalLines(lineList);
                    break;
            }

            return linesToMerge;
        }

        private Vector3Int GetVector3IntWithOffset(Line.LineDirection lineDirection, Vector3Int intialPos, int offset)
        {
            switch (lineDirection)
            {
                case Line.LineDirection.vertical:
                    return new Vector3Int(intialPos.x, 0, intialPos.z + offset);
                default:
                    return new Vector3Int(intialPos.x + offset, 0, intialPos.z);
            }
        }

        private void CalculateIntersections(List<Line> lines)
        {
            foreach (Line line in lines)
            {
                line.CalculateIntersections(lines);
            }
        }

        private List<Line> GetVerticalLines(List<Line> lines)
        {
            List<Line> linesOut = new List<Line>();
            foreach (Line line in lines)
            {
                if (line.lineDirection == Line.LineDirection.vertical)
                {
                    linesOut.Add(line);
                }
            }
            return linesOut;
        }

        private List<Line> GetHorizontalLines(List<Line> lines)
        {
            List<Line> linesOut = new List<Line>();
            foreach (Line line in lines)
            {
                if (line.lineDirection == Line.LineDirection.horizontal)
                {
                    linesOut.Add(line);
                }
            }
            return linesOut;
        }

        private void MergeLines(Line line, Line lineToMerge)
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

        #endregion
    }
}