using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.UI.LabUI
{
    public class TechTree : MonoBehaviour
    {
        public List<Node> treeNodes;
        public List<Node> researchedTreeNodes;

        protected virtual void Awake()
        {
            researchedTreeNodes = new List<Node>();
        }

        public void CloseDescriptions()
        {
            foreach(Node node in treeNodes)
            {
                node.CloseDescription();
            }
        }
    }
}
