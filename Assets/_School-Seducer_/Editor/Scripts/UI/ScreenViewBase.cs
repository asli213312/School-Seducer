using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public abstract class ScreenViewBase : MonoBehaviour
    {
        [SerializeField] private Button infoButton;
        [SerializeField] private Button storyButton;
        [SerializeField] private Button galleryButton;
        [SerializeField] private Text characterName;
    }
}