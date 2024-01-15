using System;
using TMPro;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    [Serializable]
    public class DataParentText : IDataParent
    {
        public TextMeshProUGUI textPro;

        public DataParentText(TextMeshProUGUI dataParent)
        {
            textPro = dataParent;
        }
    }
}