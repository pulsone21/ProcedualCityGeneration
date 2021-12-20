using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public static class PlacementHelper
    {
        public static List<Direction> FindNeighbor(Vector3Int position, ICollection<Vector3Int> collection)
        {
            List<Direction> neighborDirection = new List<Direction>();
            if (collection.Contains(position + Vector3Int.right)) neighborDirection.Add(Direction.right);
            if (collection.Contains(position + Vector3Int.left)) neighborDirection.Add(Direction.left);
            if (collection.Contains(position + new Vector3Int(0, 0, 1))) neighborDirection.Add(Direction.up);
            if (collection.Contains(position + new Vector3Int(0, 0, -1))) neighborDirection.Add(Direction.down);
            return neighborDirection;
        }
    }
}