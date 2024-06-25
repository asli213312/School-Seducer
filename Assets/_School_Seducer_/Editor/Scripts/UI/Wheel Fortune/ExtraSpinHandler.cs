using System.Collections;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class ExtraSpinHandler : SpinHandler
    {
        [Zenject.Inject] private GlobalSelectors _globalSelectors;
        [Zenject.Inject] private SpinHandlerUtility _spinHandlerUtility;

        [Header("Extra")] 
        [SerializeField] private Push extraWinPush;
        [SerializeField] private ScrollRect pushScroller;
        [SerializeField] private Transform contentSlots;
        [SerializeField] private Transform[] animatedSlots;
        [SerializeField] private WinSpinCharacterView winSpinCharacterPrefab;
        [SerializeField] private GiftPopup giftPopup;
        [SerializeField] private Button claimAllButton;

        [Header("Options")] 
        [SerializeField] private int multiplier;
        [SerializeField] private Vector3 giftPopupOffset;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float rotationSlowSpeed;
        [SerializeField] private float decelerationMin;
        [SerializeField] private float decelerationMax;
        [SerializeField] private float secondsSlowSpdSlots;

        private List<WheelSlot> _winSlots = new();
        private List<WheelSlot> _winCharacterSlots = new();
        private List<Character> _winCharacters = new();

        private new void Awake()
        {
            base.Awake();
            claimAllButton.AddListener(GiveGiftToWinCharacters);
            _globalSelectors.SelectedObjectEvent += GiftSelected;
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            claimAllButton.RemoveListener(GiveGiftToWinCharacters);
            _globalSelectors.SelectedObjectEvent -= GiftSelected;
        }

        protected override void InitParametersData()
        {
            RotationSpeed = rotationSpeed;

            Deceleration = Random.Range(decelerationMin, decelerationMax);
        }

        protected override bool TryBuySpin()
        {
            if (SpinHandler.Data.CanSpin(Bank.Data.Money, multiplier) == false)
            {
                Debug.LogWarning("Not enough money to spin!");
                return false;
            }

            if (SpinHandler.scrollCharactersContent.childCount > 0)
            {
                Bank.ChangeValueGold(-SpinHandler.Data.moneyForSpin * multiplier);
            }

            return true;
        }

        protected override bool ConditionToSpinning() => true;

        protected override IEnumerator ProcessButtonClicked()
        {
            yield return HandleSpinSlots();
            
            SpinHandler.charactersLockedSlot.gameObject.Deactivate();

            yield return HandleScrollCharacter();

            yield return HandleWinCharacter();

            SetSpinButtonSpinStatus();

            SpinHandler.spinCompleted?.Invoke();
        }

        protected override IEnumerator WaitForResult()
        {
            yield return new WaitForSeconds(secondsSlowSpdSlots);

            InvokeStopWheel();
        }

        protected override void InvokeCharactersMode()
        {
            DisableCharactersMode();
            InvokeStopWheel();
        }

        protected override IEnumerator InvokeHandleScrollCharacter()
        {
            yield return null;
        }

        protected override void SetupSpeedParametersCharacters() { }

        protected override void ActionGiftGot()
        {
            SpawnExtraContent();
            SpinHandler.ShowPush(extraWinPush, extraWinPush.Upscale);
            SpineUtility.InstallAnimation(extraWinPush.gameObject.GetComponent<SkeletonGraphic>());
            SpineUtility.StartupAnimation();
            SpineUtility.SetAnimationState(0);
            pushScroller.verticalNormalizedPosition = 1;
        }

        protected override void SetupExtraSlots()
        {
            ResetSlotsContent();
            SetupExtra(SpinHandler.slots, _winSlots);
            RotationSpeed = rotationSpeed;
        }

        protected override void SetupExtraCharacters() 
        {
            SetupExtraCharactersLoop();
        }

        private void GiftSelected(Transform gift) 
        {
            if (gift.gameObject.TryGetComponent(out WheelSlot slotView)) return;

            WinSpinCharacterView view = gift.gameObject.GetComponent<WinSpinCharacterView>();
            giftPopup.Render(view.SlotData.score, view.SlotData.iconInfo);
            giftPopup.SetOffset(giftPopupOffset);
        }

        private void GiveGiftToWinCharacters()
        {
            for (var i = 0; i < _winCharacters.Count && i < _winSlots.Count; i++)
            {
                SetupWinCharacter(_winCharacters[i], _winSlots[i].Data);
                Debug.Log($"Gift {i + 1} to " + _winCharacters[i].name + " " + _winSlots[i].Data.name);
            }
        }

        private void SetupExtraCharactersLoop()
        {
            if (_spinHandlerUtility.SelectedWinCharacters.Count > 0)
            {
                _winCharacterSlots.AddRange(_spinHandlerUtility.SelectedWinCharacters);
                return;
            }
            
            for (int i = 0; i < 20 * multiplier - 1; i++)
            {
                foreach (var character in SpinHandler.Previewer.Characters)
                {
                    WheelSlot slot = SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
                    
                    if (character.Data.name != slot.Data.name) continue;
                    
                    _winCharacterSlots.Add(slot);   
                }
            }
            
            Debug.Log("All slots in extra spin: " + _winSlots.Count);
        }

        private void SetupExtra(List<WheelSlot> typeSlots, List<WheelSlot> fillContentList)
        {
            for (int i = 0; i < multiplier - 1; i++)
            {
                WheelSlot slot = SpinHandler.FindSlotForProbability(typeSlots);
                fillContentList.Add(slot);   
            }
            
            fillContentList.Add(CurrentWinSlot);
            Debug.Log("All slots in extra spin: " + _winSlots.Count);
        }

        private void SpawnExtraContent()
        {
            int animatedSlotsIndex = 0;

            foreach(var slot in animatedSlots) 
            {
                if (slot.transform.childCount > 0)
                    slot.transform.GetChild(0).gameObject.Destroy();
            }

            for (int i = 0; i < _winSlots.Count && i < _winCharacterSlots.Count; i++)
            {
                Character winCharacter = null;
                
                for (int j = 0; j < SpinHandler.Previewer.Characters.Length; j++)
                {
                    if (SpinHandler.Previewer.Characters[j].Data.name != _winCharacterSlots[i].Data.name) continue;

                    winCharacter = SpinHandler.Previewer.Characters[j];
                }
                
                if (winCharacter == null) continue;

                WinSpinCharacterView view = Instantiate(winSpinCharacterPrefab, contentSlots);
                view.Initialize(_globalSelectors);
                view.Render(_winCharacterSlots[i].Data, _winSlots[i].Data);

                if (animatedSlotsIndex <= 3) 
                {
                    WinSpinCharacterView animSlotView = Instantiate(winSpinCharacterPrefab, animatedSlots[animatedSlotsIndex]);
                    animSlotView.Initialize(_globalSelectors);
                    animSlotView.Render(_winCharacterSlots[i].Data, _winSlots[i].Data);

                    animatedSlotsIndex++;
                }
                
                _winCharacters.Add(winCharacter);
                
                //SetupWinCharacter(winCharacter, _winSlots[i].Data);
            }
        }

        protected override void SubtractSpeedSlots()
        {
            RotationSpeed -= rotationSlowSpeed * Time.fixedDeltaTime;
        }

        protected override void ResetSpeedParameters()
        {
            RotationSpeed = rotationSpeed;
            Deceleration = Random.Range(decelerationMin, decelerationMax);
            
            DisableSpinning();
            DisableCharactersMode();
        }

        private void ResetSlotsContent()
        {
            if (contentSlots.childCount <= 0) return;
            
            _winSlots.Clear();
            _winCharacterSlots.Clear();
            _winCharacters.Clear();

            for (int i = 0; i < contentSlots.childCount; i++)
            {
                Destroy(contentSlots.GetChild(i).gameObject);
            }
        }
    }
}