namespace PuzzleGame.Gameplay.Merged.Tasks.Models
{
    public interface IMiniGamesTaskUpdater { }

    public struct MiniGamesTaskGatherSetsUpdater : IMiniGamesTaskUpdater
    {
        public int numberOfSet;

        public MiniGamesTaskGatherSetsUpdater(int numberOfSet) 
        {
            this.numberOfSet = numberOfSet;
        }
    }
}