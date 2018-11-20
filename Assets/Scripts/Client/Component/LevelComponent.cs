using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GraphGame.Client
{
    public class LevelComponent
        : MonoBehaviour
    {
        [SerializeField]
        private GameObject LevelNode;

        private void ShowLevels()
        {
            var count = ConfigMgr.Instance.LevelCount;
            for(var i = 0; i < count; ++i)
            {
                this.AddLevelNode(i);
            }
        }

        private List<GameObject> Nodes = new List<GameObject>();
        private void AddLevelNode(int id)
        {
            var go = Instantiate(this.LevelNode);
            go.transform.SetParent(this.transform);
            go.SetActive(true);

            var c = go.GetComponent<LevelNodeComponent>();
            c.Setup(id, -1);
        }

        private void RemoveAllNode()
        {
            foreach (var go in this.Nodes)
                Destroy(go);

            this.Nodes.Clear();
        }

        private void OnEnable()
        {
            this.ShowLevels();
        }

        private void OnDisable()
        {
            this.RemoveAllNode();
        }
    }
}
