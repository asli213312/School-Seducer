using _School_Seducer_.Editor.Scripts.Chat;
using UniRx;

namespace _School_Seducer_.Editor.Scripts
{
    public interface IInfoCharacterModule : IModule<InfoScreenSystem>, ICharacterSelected
    {
        void Initialize();
    }

    public interface IInfoScrollersModule : IModule<InfoScreenSystem>, ICharacterSelected
    {
        ReactiveProperty<Character> CurrentCharacter { get; }
        void Initialize();
    }

    public interface ICharacterSelected
    {
        void OnCharacterSelected(Character character);
    }
}