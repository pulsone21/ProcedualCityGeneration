using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public class Visualizer : MonoBehaviour
    {
        [SerializeField] private LSystemGenerator lSystem;
        [SerializeField] private RoadHelper roadHelper;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private Color lineColor;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private List<Vector3Int> position = new List<Vector3Int>();
        [SerializeField] private List<GameObject> lineEndpoints = new List<GameObject>();
        private LineHelper lH;
        private int length = 1;
        public int Length
        {
            get
            {
                if (length < 1)
                {
                    return 1;
                }
                else
                {
                    return length;
                }
            }
            set
            {
                if (value < 0)
                {
                    length = 1;
                }
                else
                {
                    length = value;
                }
            }
        }

        private float angle = 90;
        public float Angle { get => angle; set => angle = value; }


        /// <summary>
        /// Main caller Function which kicks in the visualization process
        /// </summary>
        /// <param name="sequence"></param>
        public void VisualizeSequenze(string sequence)
        {
            ConvertSentenceToLinesAndPoints(sequence);
            FixLinesAndPoints();
        }

        //Step 1 Draw Lines and Points
        public void ConvertSentenceToLinesAndPoints(string sequence)
        {
            lH = new LineHelper(transform, lineMaterial, lineColor);
            Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
            Vector3 currentPostion = Vector3.zero;
            Vector3 direction = Vector3.forward;
            Vector3 tempPosition;

            //adds starting point of drawing 
            position.Add(Vector3Int.RoundToInt(currentPostion));

            foreach (char letter in sequence)
            {
                EncodingLetters encoding = (EncodingLetters)letter;
                switch (encoding)
                {
                    case EncodingLetters.unknown:
                        break;
                    case EncodingLetters.save:
                        AgentParameters agent = new AgentParameters(currentPostion, direction, Length);
                        savePoints.Push(agent);
                        break;
                    case EncodingLetters.load:
                        if (savePoints.Count > 0)
                        {
                            AgentParameters agentParameters = savePoints.Pop();
                            currentPostion = agentParameters.position;
                            direction = agentParameters.direction;
                            Length = agentParameters.length;
                        }
                        else
                        {
                            throw new System.Exception("SavePoint Stack is empty, can not Load.");
                        }
                        break;
                    case EncodingLetters.draw:
                        tempPosition = currentPostion;
                        currentPostion += direction * Length;
                        lH.DrawLine(Vector3Int.RoundToInt(tempPosition), Vector3Int.RoundToInt(currentPostion));
                        Length += 1;
                        position.Add(Vector3Int.RoundToInt(currentPostion));
                        break;
                    case EncodingLetters.turnRight:
                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;
                    case EncodingLetters.turnLeft:
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;
                }
            }
            foreach (Vector3 pos in position)
            {
                lineEndpoints.Add(Instantiate(pointPrefab, pos, Quaternion.identity, transform));
            }
        }

        //Step 2 Fix Lines -> Clear Paralelle lines, snap Endpoints to Roads if needed.

        private void FixLinesAndPoints()
        {
            lH.MergeAlignedLines(lH.GetVerticalLines());
            lH.MergeAlignedLines(lH.GetHorizontalLines());
            //FixPoints();

        }

        private void FixPoints()
        {
            foreach (GameObject gO in lineEndpoints)
            {
                Vector3Int currPos = Vector3Int.RoundToInt(gO.transform.position);


                //do something
            }
            //Loop through all Points an Check if there is an Line in the nearer radius - maybe 2 Vector3Ints 
            //Then Build a new Line towards that direction
            //Can LineHelper knows all line Question is how do we know the coords between the start and end point of that specific line
        }


        //Step 3 Place Basic Roads

        //Step 4 Fix Roads

        // public void VisualizeSequenze(string sequence)
        // {
        //     Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        //     Vector3 currentPostion = Vector3.zero;
        //     Vector3 direction = Vector3.forward;
        //     Vector3 tempPosition;

        //     position.Add(currentPostion);
        //     foreach (char letter in sequence)
        //     {
        //         EncodingLetters encoding = (EncodingLetters)letter;

        //         switch (encoding)
        //         {
        //             case EncodingLetters.unknown:
        //                 break;
        //             case EncodingLetters.save:

        //                 AgentParameters agent = new AgentParameters(currentPostion, direction, Length);
        //                 savePoints.Push(agent);
        //                 break;
        //             case EncodingLetters.load:
        //                 if (savePoints.Count > 0)
        //                 {

        //                     AgentParameters agentParameters = savePoints.Pop();
        //                     currentPostion = agentParameters.position;
        //                     direction = agentParameters.direction;
        //                     Length = agentParameters.length;
        //                 }
        //                 else
        //                 {
        //                     throw new System.Exception("SavePoint Stack is empty, can not Load.");
        //                 }
        //                 break;
        //             case EncodingLetters.draw:

        //                 tempPosition = currentPostion;
        //                 currentPostion += direction * Length;
        //                 roadHelper.PlaceRoadAt(tempPosition, direction, Length);
        //                 Length += 1;
        //                 position.Add(currentPostion);
        //                 break;
        //             case EncodingLetters.turnRight:

        //                 direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
        //                 break;
        //             case EncodingLetters.turnLeft:

        //                 direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
        //                 break;
        //         }
        //     }
        //     roadHelper.FixRoads();
        // }

        public void ClearHelperGameObjects()
        {
            int childCount = this.transform.childCount - 1;
            while (childCount != -1)
            {
                DestroyImmediate(this.transform.GetChild(childCount).gameObject);
                childCount = this.transform.childCount - 1;
            }
        }

        public void ClearVisulization()
        {
            position.Clear();
            Length = 8;
            ClearHelperGameObjects();
            roadHelper.ClearRoads();
        }
    }
}
