using System.Collections;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Services.Tutorial
{
    public class TutorialContractMap : TutorialContractBase
    {
        [SerializeField] private Previewer previewer;
        [SerializeField] private Chat.Chat chat;
        
        protected override IEnumerator Process()
        {
            yield return new WaitUntil(() => previewer.gameObject.activeSelf);
        }
    }
}