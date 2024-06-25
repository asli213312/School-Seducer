using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class AlignSizeDelta : MonoBehaviour
    {
        public RectTransform RectUpdate { get; set; }
        private RectTransform _rectOffsetToUpdate;
        private int _amountSymbols;
        
        private bool _aligned;

        public void Initialize(RectTransform rectNeedUpdate, RectTransform rectOffsetToUpdate, int amountSymbolsMsg)
        {
            RectUpdate = rectNeedUpdate;
            _rectOffsetToUpdate = rectOffsetToUpdate;
            _amountSymbols = amountSymbolsMsg;
        }

        public void Reset() => _aligned = false;

        private void Update()
        {
            if (RectUpdate != null && _rectOffsetToUpdate != null && !_aligned)
            {
                RectUpdate.sizeDelta = new Vector2(0, _rectOffsetToUpdate.sizeDelta.y / 7f / 13f);

                _aligned = true;
            }
        }
    }
}