using UnityEngine.Events;
using UnityEngine.UI;

namespace _Kittens__Kitchen.Editor.Scripts.Utility.Extensions
{
    public static class ButtonExtensions
    {
        public static void AddListener(this Button button, UnityAction callBack)
        {
            button.onClick.AddListener(callBack);
        }
        
        public static void RemoveListener(this Button button, UnityAction callBack)
        {
            button.onClick.RemoveListener(callBack);
        }

        public static void RemoveAllListeners(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        public static void Active(this Button button)
        {
            button.interactable = true;
        }
        
        public static void InActive(this Button button)
        {
            button.interactable = false;
        }
    }
}