using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class OpenContentGame : OpenContentBase
    {
        public InfoMiniGameBase InstanceGame => _miniGame;
        
        protected override IModeContent ModeContent => new GameMode(_miniGame);
        protected override bool NeedContentScreen => false;

        private InfoMiniGameBase _miniGame;
        private InfoMiniGameDataBase _data;
        private Transform _spawnPoint;

        public void Initialize(InfoMiniGameDataBase miniGameData, Transform spawnPoint)
        {
        	_data = miniGameData;
            _spawnPoint = spawnPoint;
            
            CreateMiniGame();
        }

        protected override void InstallComponents()
        {
            
        }

        protected override void OnDestroyContent()
        {
            base.OnDestroyContent();
            
            if (_miniGame != null)
                Destroy(_miniGame.gameObject);    
        }

        private void CreateMiniGame()
        {
            InfoMiniGameBase view = Instantiate(_data.prefab, _spawnPoint);
            view.Initialize(_spawnPoint);

            _miniGame = view;
        }
    }
}