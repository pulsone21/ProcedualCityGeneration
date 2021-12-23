using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    [System.Serializable]
    public class Line
    {
        public enum LineDirection { vertical, horizontal }
        [SerializeField] public Vector3Int startPos;
        [SerializeField] public Vector3Int endPos;
        [SerializeField] public GameObject gameObject;
        public Direction direction { get; protected set; }
        public LineDirection lineDirection { get; protected set; }
        public bool intersectionsCalculated { get; protected set; }
        [SerializeField] public int intersectionCount;
        public int lineLength { get; protected set; }
        readonly List<Vector3Int> linePoints;
        private List<Line> connectedTo = new List<Line>();

        public Line(Vector3Int startPos, Vector3Int endPos, GameObject gameObject, Direction newDirection)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            this.gameObject = gameObject;
            this.lineLength = Mathf.RoundToInt(Vector3Int.Distance(startPos, endPos));
            this.linePoints = new List<Vector3Int>();
            this.direction = newDirection;
            this.lineDirection = direction == Direction.right || direction == Direction.left ? LineDirection.horizontal : LineDirection.vertical;
            this.intersectionsCalculated = false;
            for (int i = 0; i < lineLength; i++)
            {
                int x = (Mathf.Abs(startPos.x - endPos.x) / lineLength) * i + endPos.x;
                int z = (Mathf.Abs(startPos.z - endPos.z) / lineLength) * i + endPos.z;
                linePoints.Add(new Vector3Int(x, 0, z));
            }
        }

        public void CalculateIntersections(List<Line> lines)
        {
            connectedTo.Clear();
            foreach (Vector3Int pos in linePoints)
            {
                foreach (Line line in lines)
                {
                    if (line != this && !this.connectedTo.Contains(line))
                    {
                        if (line.IsPositionInTheLine(pos))
                        {
                            connectedTo.Add(line);
                            intersectionCount++;
                        }
                    }
                }
            }
            intersectionsCalculated = true;
        }

        public List<Vector3Int> GetAllLinePoints()
        {
            return linePoints;
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