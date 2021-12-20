using System.Text;
using UnityEngine;
namespace LSystem
{
    public class LSystemGenerator : MonoBehaviour
    {
        private enum DrawMode { Debug, Mesh, Both }
        public LSystemGenerator instance;
        public Rule[] rules;
        public string rootSentence;
        [Range(0, 10)] public int iterationLimit = 1;
        public DebugVisualizer debugVisualizer;
        public Visualizer visualizer;

        [SerializeField] private DrawMode drawMode;
        [SerializeField] private bool ignorRuleModifer = false;
        [SerializeField, Range(0f, 1f)] private float chanceToIgnorRule = 0.3f;

        private void Awake()
        {
            if (instance)
            {
                DestroyImmediate(this);
            }
            else
            {
                instance = this;
            }

        }


        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            string sentence = GenerateSentence();
            switch (drawMode)
            {
                case DrawMode.Debug:
                    debugVisualizer.VisualizeSequenze(sentence);
                    break;
                case DrawMode.Mesh:
                    visualizer.VisualizeSequenze(sentence);
                    break;
                case DrawMode.Both:
                    visualizer.VisualizeSequenze(sentence);
                    debugVisualizer.VisualizeSequenze(sentence);
                    break;
            }
        }

        public string GenerateSentence(string word = null)
        {
            if (word == null)
            {
                word = rootSentence;
            }
            return GrowRecursive(word);
        }

        private string GrowRecursive(string word, int iterationIndex = 0)
        {
            if (iterationIndex >= iterationLimit)
            {
                return word;
            }
            StringBuilder newWord = new StringBuilder();

            foreach (char c in word)
            {
                newWord.Append(c);
                ProcessRulesRecursivelly(newWord, c, iterationIndex);
            }

            return newWord.ToString();
        }

        private void ProcessRulesRecursivelly(StringBuilder newWord, char c, int iterationIndex)
        {
            foreach (Rule rule in rules)
            {
                if (rule.letter == c.ToString())
                {
                    if (ignorRuleModifer && iterationIndex > 1)
                    {
                        if (Random.value < chanceToIgnorRule)
                        {
                            return;
                        }
                    }
                    newWord.Append(GrowRecursive(rule.GetResult(), iterationIndex + 1));
                }

            }
        }
    }
}