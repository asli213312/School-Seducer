namespace PuzzleGame.UI
{
    public class TopScoreCounter : ScoreCounter
    {
        public override int Value => currentGameState.TopScore;
    }
}