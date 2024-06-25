using System;
using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MonoController : MonoBehaviour
    {
        [SerializeField, Tooltip("Use to update group at once")] private MonoContainer[] groupsMono;
        [SerializeField] private MonoText[] monoTexts;
        private MonoText _lastMonoText;

        public void UpdateGroupByName(string nameId)
        {
            foreach (var monoGroup in groupsMono)
            {
                if (monoGroup.nameId == nameId) 
                    monoGroup.UpdateAll(); break;
            }
        }

        public void UpdateMonoByName(string nameId)
        {
            if (_lastMonoText != null && _lastMonoText.NameId == nameId)
            {
                _lastMonoText.UpdateMember();
                
                Report();
                return;
            }
            
            foreach (var monoText in monoTexts)
            {
                if (monoText.NameId == nameId)
                {
                    monoText.UpdateMember();
                    _lastMonoText = monoText;
                    
                    Report();
                    break;
                }
            }

            void Report() => Debug.Log("Update monoText completed! " + nameId);
        }

        [Serializable]
        private class MonoContainer
        {
            [SerializeField] public string nameId;
            [SerializeField] public List<MonoText> monoTexts;
            
            public void UpdateAll() => monoTexts.ForEach(x => x.UpdateMember());
        }
    }
}