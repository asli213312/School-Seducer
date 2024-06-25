using _School_Seducer_.Editor.Scripts.Utility;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class GameMode : BaseModeContent, IModeContent
    {
        protected override bool Opened => _miniGame.gameObject.activeSelf;
        protected override ModeContentEnum ModeContent => ModeContentEnum.Game;
        
        private InfoMiniGameBase _miniGame;

        public GameMode(InfoMiniGameBase miniGame)
        {
            if (miniGame == null) return;

            _miniGame = miniGame;
        }

        public void OnClick()
        {
            if (_miniGame == null) return;

            _miniGame.StartGame();
        }
    }
}