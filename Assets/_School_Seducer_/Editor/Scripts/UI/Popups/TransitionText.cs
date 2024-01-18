using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    public class TransitionText : Transition, ITransition
    {
        [SerializeField, Space(10)] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI _dataParent;
        [SerializeField, ShowIf(nameof(needConstBaseText))] private TextMeshProUGUI changeText;
        [SerializeField, ShowIf(nameof(needConstBaseText))] private string baseText;
        
        [InfoBox("If need base text + changed text at update")]
        [SerializeField] private bool needConstBaseText;
        
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
            if (needConstBaseText == false)
            {
                if (_data != null) text.text = _data;
                else if (_dataParent != null) text.text = _dataParent.text;   
            }
            else
            {
                text.text = baseText + " " + changeText.text;
            }
        }

        public override void SetDataParent(IDataParent dataRequestedParent)
        {
            if (_dataParent != null || needConstBaseText) return;
            
            if (dataRequestedParent is DataParentText textDataParent)
            {
                if (textDataParent.textPro.name == name)
                    _dataParent = textDataParent.textPro;
                else
                    Debug.LogError($"DataParentText name is not equal to TransitionText name = {textDataParent.textPro.name}", gameObject);
            }
            
            Transit();
        }
    }
}