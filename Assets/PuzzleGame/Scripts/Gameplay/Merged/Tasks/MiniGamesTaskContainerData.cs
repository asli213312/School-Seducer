using UnityEngine;

namespace PuzzleGame.Gameplay.Merged
{
    [CreateAssetMenu(fileName = "TaskConfig", menuName = "Game/Data/MiniGames/TaskConfig", order = 0)]
    public class MiniGamesTaskContainerData : ScriptableObject
	{
		[SerializeField] public int maxTasks;
		[SerializeField, SerializeReference] public MiniGamesAbstractTaskData[] tasks;
	}
}