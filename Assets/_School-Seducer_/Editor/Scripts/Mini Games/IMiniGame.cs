using System;
using System.Collections;

public interface IMiniGame
{
    event Action<bool> OnMiniGameFinished;
    bool IsGameFinished { get; }
    IEnumerator StartGame();
}