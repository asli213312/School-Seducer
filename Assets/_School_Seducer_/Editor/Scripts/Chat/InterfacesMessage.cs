using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public interface IMessage
    {
        void Initialize(OptionButton[] optionButtons);
        void RenderGeneralData(MessageData data, Sprite actorLeft, Sprite actorRight, Sprite storyTeller, bool needIconStoryTeller);
	    void SetNameActors(string leftActor, string rightActor, string storyTeller);
    }

    public interface IMessageProxy
    {
        void RenderGeneralData(MessageData data, ChatConfig chatConfig, Transform contentMsg);
    }
    
    public interface IMessageBase
    {
        
    }

    public interface IMessageCondition
    {
        bool CheckCondition();
        void ResolveCondition();
    }

    public interface IMessageName
    {
        string MsgNameText { get; set; }
    }
}