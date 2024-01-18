using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Popups
{
    public abstract class Transition : MonoBehaviour
    {
        public abstract void SetData(ITransitionData data);
        public abstract void Transit();
        public abstract void SetDataParent(IDataParent dataRequestedParent);
    }
}