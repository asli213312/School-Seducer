using UnityEngine;

namespace PuzzleGame.Gameplay.Merged
{
    public interface IModule<in TSystem> where TSystem : MonoBehaviour
    {
        void InitializeCore(TSystem system);
    }
}