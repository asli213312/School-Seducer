using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    [Serializable]
    public struct DataParentImage : IDataParent
    {
        public Image image;
        public string fieldName;

        public DataParentImage(Image dataParent, string fieldName)
        {
            this.fieldName = fieldName;
            image = dataParent;  
        } 
    }
}