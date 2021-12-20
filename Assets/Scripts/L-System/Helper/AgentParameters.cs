using UnityEngine;

namespace LSystem
{
    internal class AgentParameters
    {
        public Vector3 position;
        public Vector3 direction;
        public int length;

        public AgentParameters(Vector3 myPostion, Vector3 direction, int length)
        {
            this.position = myPostion;
            this.direction = direction;
            this.length = length;
        }

        public override string ToString()
        {
            return $"Pos: {position}, direction: {direction}, length: {length}";
        }
    }
}