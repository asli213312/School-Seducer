using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine.UIElements;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public abstract class BaseModeContent
    {
        protected abstract bool Opened { get; }
        protected abstract ModeContentEnum ModeContent { get; }
    }
}