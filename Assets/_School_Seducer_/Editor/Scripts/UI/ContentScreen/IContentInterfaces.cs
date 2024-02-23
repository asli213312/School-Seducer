using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Chat;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public interface IContentDisplay
    {
        OpenContentBase CurrentContent { get; set; }
        void Initialize(IContentDataProvider dataProvider);
        void ShowContent(OpenContentBase content);
        void SwitchToNextContent();
        void SwitchToPreviousContent();
    }

    public interface IContentAnimation
    {
        void EnableAnimation();
    }

    public interface IContentUserInteraction
    {
        void Initialize(IContentDisplay displayModule, IContentAnimation contentAnimation);
        void HandleUserInteraction();
    }

    public interface IContentDataProvider
    {
        List<IContent> ContentList { get; set; }
        void LoadContentData(List<IContent> contentList);
        void ResetContentList();
    }

    public interface IContentScreenModules
    {
        IContentDisplay GetContentDisplay();
        IContentDataProvider GetContentDataProvider();
        IContentUserInteraction GetContentUserInteraction();
    }
}