using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GraphGame.Client
{
    [RequireComponent(typeof(SpriteRef))]
    public class LevelNodeComponent
        : MonoBehaviour
        , IPointerDownHandler
    {
        [SerializeField]
        private List<Image> Stars;
        [SerializeField]
        private GameObject StarNode;
        [SerializeField]
        private GameObject QuestionNode;

        private SpriteRef refs;
        private void Awake()
        {
            this.refs = this.GetComponent<SpriteRef>();
        }

        private int LevelID;
        public void Setup(int levelID, int starValue)
        {
            this.LevelID = levelID;
            this.InitUserStatus(starValue);
        }

        private void InitUserStatus(int starValue)
        {
            if (starValue < 0)
            {
                this.ShowStar(false);
                return;
            }

            foreach (var img in this.Stars)
            {
                img.sprite = this.refs["Gray"];
            }

            for (var i = 0; i < starValue; ++i)
            {
                this.Stars[i].sprite = this.refs["Yellow"];
            }

            this.ShowStar(true);
        }

        private void ShowStar(bool flag)
        {
            this.StarNode.SetActive(flag);
            this.QuestionNode.SetActive(!flag);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 开始游戏
            EntryComponent.Instance.MenuMgr.Execute(new StartGameMenu(this.LevelID));
        }
    }
}
