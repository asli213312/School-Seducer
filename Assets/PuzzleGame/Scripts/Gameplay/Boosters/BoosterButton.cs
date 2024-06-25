using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.Gameplay.Boosters
{
    public class BoosterButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] public Button infoButton;
        [SerializeField] private GameObject counter;
        [SerializeField] private GameObject counterEmpty;
        [SerializeField] private Text countText;
        [SerializeField] private Text emptyText;

        public event Action<BoosterPreset, bool> Select = delegate { };

        private BoosterPreset preset;
        private int highlightSortingOrder;
        private bool canBuy;
        private bool IsPurchased => BoostersController.Instance.IsBoosterPurchased(preset);

        public BoosterPreset Preset => preset;
    
        private void Awake()
        {
            BoostersController.Instance.BoosterPurchased += OnButtonUpdate;
            BoostersController.Instance.BoosterProceeded += OnButtonUpdate;
            BoostersController.Instance.BoosterUpdated += OnButtonUpdate;
        }

        private void OnDestroy()
        {
            BoostersController.Instance.BoosterPurchased -= OnButtonUpdate;
            BoostersController.Instance.BoosterProceeded -= OnButtonUpdate;
            BoostersController.Instance.BoosterUpdated -= OnButtonUpdate;
            
            infoButton.onClick.RemoveAllListeners();
        }

        public void Init(BoosterPreset preset, int highlightSortingOrder, bool canBuy)
        {
            this.preset = preset;
            this.highlightSortingOrder = highlightSortingOrder;
            this.canBuy = canBuy;
            icon.sprite = preset.Icon;
        
            UpdateButton();
        }

        public void SetRaycast(bool value)
        {
            button.interactable = value;
        }

        private void Highlight()
        {
            gameObject.AddComponent<Canvas>();
            gameObject.GetComponent<Canvas>().overrideSorting = true;

            gameObject.GetComponent<Canvas>().sortingOrder = highlightSortingOrder;
        }

        public void Deselect()
        {
            Destroy(gameObject.GetComponent<Canvas>());
            UpdateButton();
        }

        private void UpdateButton()
        {
            counter.gameObject.SetActive(IsPurchased || !canBuy);
            counterEmpty.gameObject.SetActive(!IsPurchased && canBuy);

            if (!IsPurchased && canBuy)
            {
                Debug.Log("Booster not purchased: " + preset.name);
                return;
            }
        
            int count = BoostersController.Instance.GetBoosterPurchaseCount(preset);
            countText.text = count.ToString();
            SetRaycast(count > 0);
        }

        public void OnClick()
        {
            if (IsPurchased)
            {
                Highlight();
                Debug.Log("Booster invoked: " + preset.name);                
            }
            else
                Debug.Log("Booster not purchased to invoke: " + preset.name);
            
            Select.Invoke(preset, IsPurchased);
        }

        private void OnButtonUpdate(BoosterPreset preset)
        {
            if(this.preset != preset) return;
        
            UpdateButton();
        }
    
        private void OnButtonUpdate(BoosterPreset arg1, bool value)
        {
            UpdateButton();
        }
    }
}
