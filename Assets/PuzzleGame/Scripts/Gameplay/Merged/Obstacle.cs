using UnityEngine;

namespace PuzzleGame.Gameplay.Merged
{
    public class Obstacle : NumberedBrick
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}