using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MonoController : MonoBehaviour
    {
        [SerializeField] private MonoText[] monoTexts;
        private MonoText _lastMonoText;

        public void UpdateMonoByName(string nameId)
        {
            if (_lastMonoText != null && _lastMonoText.NameId == nameId)
            {
                _lastMonoText.UpdateText();
                
                Report();
                return;
            }
            
            foreach (var monoText in monoTexts)
            {
                if (monoText.NameId == nameId)
                {
                    monoText.UpdateText();
                    _lastMonoText = monoText;
                    
                    Report();
                    break;
                }
            }

            void Report() => Debug.Log("Update monoText completed! " + nameId);
        }
    }
}