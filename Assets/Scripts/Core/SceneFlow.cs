using UnityEngine;

public static class SceneFlow
{
    public const string MainMenuScene = "MainMenu";
    public const string QuestionScene = "Question";

    static readonly string[] MiniGameScenes = { "KeyCard", "DigitCode" };
    static int nextMiniGameIndex;

    public static void ResetRun()
    {
        nextMiniGameIndex = 0;
    }

    public static string StartRun()
    {
        ResetRun();
        HealthManager.Instance?.ResetHealth();
        return PickMiniGame();
    }

    public static string CompleteQuiz(bool won)
    {
        if (won)
        {
            return PickMiniGame();
        }
        else
        {
            HealthManager.Instance?.LoseLife();
            return QuestionScene;
        }
    }

    public static string CompleteMiniGame(bool won)
    {
        if (won)
        {
            return QuestionScene;
        }
        else
        {
            return QuestionScene;
        }
    }

    static string PickMiniGame()
    {
        string scene = MiniGameScenes[nextMiniGameIndex];
        nextMiniGameIndex = (nextMiniGameIndex + 1) % MiniGameScenes.Length;
        return scene;
    }
}
