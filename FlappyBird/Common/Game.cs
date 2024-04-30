using System;

namespace FlappyBird.Common;

public class Game
{
    public static event Action GameOverEvent;
    public static event Action AddScoreEvent;
    public static event Action LevelUpEvent;

    public static void OnGameOverEvent()
    {
        GameOverEvent?.Invoke();
    }
    public static void OnAddScoreEvent()
    {
        AddScoreEvent?.Invoke();
    }
    
    public static void OnLevelUpEvent()
    {
        LevelUpEvent?.Invoke();
    }
}