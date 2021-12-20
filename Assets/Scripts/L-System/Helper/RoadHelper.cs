using System;
using System.Collections.Generic;
using UnityEngine;


namespace LSystem
{
    public class RoadHelper : MonoBehaviour
    {
        public GameObject roadStraight, roadCorner, road3Way, road4Way, roadEnd;
        private Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
        private HashSet<Vector3Int> allRoadPositions = new HashSet<Vector3Int>();
        private HashSet<Vector3Int> fixRoadCandidates = new HashSet<Vector3Int>();

        public void PlaceRoadAt(Vector3 startPos, Vector3 directionIn, int length)
        {
            Quaternion rotation = Quaternion.identity;
            Vector3Int direction = Vector3Int.RoundToInt(directionIn);
            if (direction.x != 0)
            {
                rotation = Quaternion.Euler(0, 90, 0);
            }

            for (int i = 0; i <= length; i++)
            {
                Vector3Int position = Vector3Int.RoundToInt(startPos + direction * i);
                if (roadDictionary.ContainsKey(position)) continue;
                GameObject road = BuildRoad(roadStraight, position, rotation, transform);
                roadDictionary.Add(position, road);
                allRoadPositions.Add(position);
            }
        }

        public void FixRoads()
        {
            Debug.Log("Road fixing started");
            foreach (Vector3Int position in allRoadPositions)
            {
                List<Direction> neighborDirection = PlacementHelper.FindNeighbor(position, roadDictionary.Keys);
                FixRoad(neighborDirection, Quaternion.identity, position);
            }
            FixDoubleRoads();
        }

        private void FixRoad(List<Direction> neighborDirections, Quaternion rotation, Vector3Int position)
        {
            if (neighborDirections.Count == 1)
            {
                //spawn end
                DestroyImmediate(roadDictionary[position]);
                if (neighborDirections.Contains(Direction.up))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (neighborDirections.Contains(Direction.left))
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (neighborDirections.Contains(Direction.right))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                roadDictionary[position] = BuildRoad(roadEnd, position, rotation, transform);
            }
            else if (neighborDirections.Count == 3)
            {
                //spawn 3Way
                DestroyImmediate(roadDictionary[position]);
                if (neighborDirections.Contains(Direction.up) && neighborDirections.Contains(Direction.right) && neighborDirections.Contains(Direction.down))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                else if (neighborDirections.Contains(Direction.up) && neighborDirections.Contains(Direction.right) && neighborDirections.Contains(Direction.left))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (neighborDirections.Contains(Direction.up) && neighborDirections.Contains(Direction.left) && neighborDirections.Contains(Direction.down))
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                roadDictionary[position] = BuildRoad(road3Way, position, rotation, transform);
                fixRoadCandidates.Add(position);
            }
            else if (neighborDirections.Count == 4)
            {
                //spawn 4Way
                DestroyImmediate(roadDictionary[position]);
                roadDictionary[position] = BuildRoad(road4Way, position, Quaternion.identity, transform);
                fixRoadCandidates.Add(position);
            }
            else
            {
                //can be corner or straight
                if (neighborDirections.Contains(Direction.up) && neighborDirections.Contains(Direction.down) || neighborDirections.Contains(Direction.right) && neighborDirections.Contains(Direction.left))
                {
                    return;
                }
                else
                {
                    //Spawn Corner
                    DestroyImmediate(roadDictionary[position]);
                    if (neighborDirections.Contains(Direction.up) && neighborDirections.Contains(Direction.right))
                    {
                        rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (neighborDirections.Contains(Direction.up) && neighborDirections.Contains(Direction.left))
                    {
                        rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (neighborDirections.Contains(Direction.down) && neighborDirections.Contains(Direction.right))
                    {
                        rotation = Quaternion.Euler(0, -90, 0);
                    }
                    roadDictionary[position] = BuildRoad(roadCorner, position, rotation, transform);
                }
            }
        }

        private void FixDoubleRoads()
        {
            Debug.Log("Double Road fixing started");
            Debug.Log("Fixing " + fixRoadCandidates.Count + " roads");
            foreach (Vector3Int position in fixRoadCandidates)
            {
                List<Direction> neighborDirections = PlacementHelper.FindNeighbor(position, roadDictionary.Keys);

                //Inital Coord -19,0,-8
                if (neighborDirections.Contains(Direction.left) && neighborDirections.Contains(Direction.up))
                {
                    //Check Position -20,0,-7  ->  -1,0,1
                    if (allRoadPositions.Contains(position - new Vector3Int(-1, 0, 1)))
                    {
                        Debug.Log("Found Road Top Left from :" + position);
                        DestroyImmediate(roadDictionary[position]);
                        roadDictionary.Remove(position);
                        continue;
                    }
                }
                if (neighborDirections.Contains(Direction.left) && neighborDirections.Contains(Direction.down))
                {
                    //Check Position -20,0,-9  ->  -1,0,-1
                    if (allRoadPositions.Contains(position - new Vector3Int(-1, 0, -1)))
                    {
                        Debug.Log("Found Road Down Left from :" + position);
                        DestroyImmediate(roadDictionary[position]);
                        roadDictionary.Remove(position);
                        continue;
                    }
                }
                if (neighborDirections.Contains(Direction.right) && neighborDirections.Contains(Direction.down))
                {
                    //Check Position -19,0,-8  ->  1,0,-1
                    if (allRoadPositions.Contains(position - new Vector3Int(1, 0, -1)))
                    {
                        Debug.Log("Found Road Down right from :" + position);
                        DestroyImmediate(roadDictionary[position]);
                        roadDictionary.Remove(position);
                        continue;
                    }
                }
                if (neighborDirections.Contains(Direction.right) && neighborDirections.Contains(Direction.up))
                {
                    //Check Position -19,0,-6  ->  1,0,1
                    if (allRoadPositions.Contains(position - new Vector3Int(1, 0, 1)))
                    {
                        Debug.Log("Found Road up right from :" + position);
                        DestroyImmediate(roadDictionary[position]);
                        roadDictionary.Remove(position);
                        continue;
                    }
                }
            }
        }

        private GameObject BuildRoad(GameObject prefab, Vector3Int position, Quaternion rotation, Transform parent)
        {
            GameObject road = Instantiate(prefab, position, rotation, parent);
            road.name = "Road at: " + position.ToString();
            return road;
        }
        public void ClearRoads()
        {
            roadDictionary.Clear();
            fixRoadCandidates.Clear();

            int childCount = this.transform.childCount - 1;
            while (childCount != -1)
            {
                DestroyImmediate(this.transform.GetChild(childCount).gameObject);
                childCount = this.transform.childCount - 1;
            }
        }
    }
}