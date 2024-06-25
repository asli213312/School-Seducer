using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility.Components
{
    [AddComponentMenu("Sibling Setter")]
    public class SiblingSetter : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private Transform target;
        [SerializeField] private int index;

        private Transform _lastParent;
        private int _lastSiblingIndex;

        public void ChangeParent(Transform newParent) => parent = newParent;
        public void ChangeTarget(Transform newTarget) => target = newTarget;
        public void ChangeIndex(int newIndex) => index = newIndex;

        public void Execute()
        {
            _lastParent = target.parent;
            _lastSiblingIndex = target.parent.transform.GetChild(target.GetSiblingIndex()).GetSiblingIndex();
            
            target.SetParent(parent);
            target.SetSiblingIndex(index);
        }

        public void Undo()
        {
            if (_lastParent == null)
            {
                Debug.LogError("Can't undo setting sibling index for: " + target.name, target.gameObject);
                return;
            }
            
            target.SetParent(_lastParent);
            target.SetSiblingIndex(_lastSiblingIndex);
        }
    }
}