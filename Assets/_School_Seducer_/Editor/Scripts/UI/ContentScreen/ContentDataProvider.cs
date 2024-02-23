using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ContentDataProvider : MonoBehaviour, IContentDataProvider
    {
        [ShowInInspector] public List<IContent> ContentList { get; set; } = new();

        public void LoadContentData(List<IContent> contentList)
        {
            ContentList = contentList;
        }

        public void ResetContentList()
        {
            if (ContentList.Count > 0) ContentList.Clear();  
        } 
    }
}