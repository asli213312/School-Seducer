using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public abstract class ScreenViewBase : MonoBehaviour
    {
        [SerializeField] protected Button infoButton;
        [SerializeField] protected Button storyButton;
        [SerializeField] protected Button galleryButton;
    }
}