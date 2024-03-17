using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility.DevMenu
{
    public class DevUtility : MonoBehaviour
    {
        [Inject] private Bank _bank;
        
        [SerializeField] private InputField goldInput;

        private void Awake()
        {
            goldInput.onSubmit.AddListener(TryAddGold);
        }

        private void OnDestroy()
        {
            goldInput.onSubmit.RemoveListener(TryAddGold);
        }

        private void TryAddGold(string amountGold)
        {
            if (amountGold.All(char.IsDigit))
            {
                int goldToSet = int.Parse(amountGold);
                _bank.Money = goldToSet;
                
                goldInput.text = "";
                goldInput.placeholder.GetComponent<Text>().text = "Gold added!";
            }
            else
            {
                goldInput.text = "";
                goldInput.placeholder.GetComponent<Text>().text = "Only digits!";
            }
        }
    }
}