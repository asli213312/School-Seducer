using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class InfiniteScrollView : MonoBehaviour
    {
        public GameObject slotPrefab;
        public Transform content;
        public int numberOfSlots = 4;
        public float slotHeight = 100f;

        private void Start()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < numberOfSlots * 2; i++)
            {
                CreateSlot(i);
            }
        }

        private void CreateSlot(int index)
        {
            GameObject slot = Instantiate(slotPrefab, content);
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            slotRect.localPosition = new Vector3(0, -index * slotHeight, 0);
        }
    }
}