using System.Collections.Generic;
using UnityEngine;

namespace LSystem
{
    public class DebugVisualizer : MonoBehaviour
    {
        [SerializeField] private LSystemGenerator lSystem;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Material lineMateiral;
        [SerializeField] private List<Vector3> position = new List<Vector3>();

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

        //TODO get some randomnes to the angle per rotation
        private float angle = 90;
        public float Angle { get => angle; set => angle = value; }

        private void Awake()
        {
            lSystem = lSystem.instance;
        }

        public void VisualizeSequenze(string sequence)
        {
            Debug.Log(sequence);
            Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
            Vector3 currentPostion = Vector3.zero;
            Vector3 direction = Vector3.forward;
            Vector3 tempPosition;

            position.Add(currentPostion);


            foreach (char letter in sequence)
            {
                EncodingLetters encoding = (EncodingLetters)letter;
                Debug.Log(letter.ToString());
                switch (encoding)
                {
                    case EncodingLetters.unknown:
                        break;
                    case EncodingLetters.save:
                        Debug.Log("saving");
                        AgentParameters agent = new AgentParameters(currentPostion, direction, Length);
                        Debug.Log(agent.ToString());
                        savePoints.Push(agent);
                        break;
                    case EncodingLetters.load:
                        if (savePoints.Count > 0)
                        {
                            Debug.Log("laoding");
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
                        Debug.Log("drawing");
                        tempPosition = currentPostion;
                        currentPostion += direction * Length;
                        DrawLine(tempPosition, currentPostion, Color.white);
                        Length += 1;
                        position.Add(currentPostion);
                        break;
                    case EncodingLetters.turnRight:
                        Debug.Log("turning right");
                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;
                    case EncodingLetters.turnLeft:
                        Debug.Log("turning left");
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;
                }
            }

            foreach (Vector3 pos in position)
            {
                Instantiate(prefab, pos, Quaternion.identity, this.transform);
            }

        }
        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            GameObject line = new GameObject("line");
            line.transform.SetParent(this.transform);
            line.transform.position = start;
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.sharedMaterial = lineMateiral;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.3f;
            lineRenderer.endWidth = 0.3f;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        public void ClearVisulization()
        {
            position.Clear();
            Length = 8;
            int childCount = this.transform.childCount - 1;
            while (childCount != -1)
            {
                DestroyImmediate(this.transform.GetChild(childCount).gameObject);
                childCount = this.transform.childCount - 1;
            }

        }
    }
}