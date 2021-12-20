using UnityEngine;


namespace LSystem
{
    [CreateAssetMenu(menuName = "ScripableObjects/L_System Rule")]
    public class Rule : ScriptableObject
    {
        public string letter;
        [SerializeField] private string[] results = null;
        [SerializeField] private bool randomResult = true;

        public string GetResult()
        {
            if (randomResult)
            {
                return results[GetRandomIndex()];
            }
            return results[0];
        }

        private int GetRandomIndex()
        {
            return UnityEngine.Random.Range(0, results.Length);
        }
    }
}
