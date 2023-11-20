using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [CreateAssetMenu]
    public class TestData : ScriptableObject
    {
        [SerializeField] public MessageData[] Messages;
    }
}