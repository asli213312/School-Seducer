using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class AlignSizeDelta : MonoBehaviour
    {
        private RectTransform _rectUpdate;
        private RectTransform _rectOffsetToUpdate;
        private int _amountSymbols;
        
        private bool _aligned;

        public void Initialize(RectTransform rectNeedUpdate, RectTransform rectOffsetToUpdate, int amountSymbolsMsg)
        {
            _rectUpdate = rectNeedUpdate;
            _rectOffsetToUpdate = rectOffsetToUpdate;
            _amountSymbols = amountSymbolsMsg;
        }

        private void Update()
        {
            if (_rectUpdate != null && _rectOffsetToUpdate != null && !_aligned)
            {
                _rectUpdate.sizeDelta = new Vector2(0, _rectOffsetToUpdate.sizeDelta.y / 7f / 13f);

                _aligned = true;
            }
        }
    }
}