using TMPro;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    public class TransitionText : Transition, ITransition
    {
        [SerializeField] private TextMeshProUGUI text;
        private TextMeshProUGUI _dataParent;
        private string _data;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        public override void SetData(ITransitionData data)
        {
            if (data is TransitionDataText newData)
            {
                _data = newData.dataText;
            }
            else Debug.LogError("Data for TransitionText is not TransitionDataText", gameObject);
        }

        public override void Transit()
        {
            if (_data != null) text.text = _data;
            else if (_dataParent != null) text.text = _dataParent.text;
        }

        public override void SetDataParent(IDataParent dataParent)
        {
            if (dataParent is DataParentText textDataParent)
            {
                if (textDataParent.textPro.name == name)
                    _dataParent = textDataParent.textPro;
                else
                    Debug.LogError($"DataParentText name is not equal to TransitionText name = {textDataParent.textPro.name}", gameObject);
            }
        }
    }
}