using _School_Seducer_.Editor.Scripts.Chat;

namespace _School_Seducer_.Editor.Scripts
{
    public interface IInfoCharacterModule : IModule<InfoScreenSystem>, ICharacterSelected
    {
        void Initialize();
    }

    public interface IInfoScrollersModule : IModule<InfoScreenSystem>, ICharacterSelected
    {
        void Initialize();
    }

    public interface ICharacterSelected
    {
        void OnCharacterSelected(Character character);
    }
}