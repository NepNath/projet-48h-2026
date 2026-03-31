using UnityEngine;

public static class SceneFlow
{
    public const string MainMenuScene = "MainMenu";
    public const string QuestionScene = "Question";
    public const string GameOverScene = "Perdu";

    static readonly string[] MiniGameScenes = { "KeyCard", "DigitCode", "SpeedBalatro" };
    static int nextMiniGameIndex;

    public static void ResetRun()
    {
        nextMiniGameIndex = 0;
    }

    public static string StartRun()
    {
        if (HealthManager.Instance == null)
        {
            var go = new GameObject("HealthManager");
            go.AddComponent<HealthManager>();
        }

        ResetRun();
        HealthManager.Instance?.ResetHealth();
        return PickMiniGame();
    }

    public static string CompleteQuiz(bool won)
    {
        if (won)
        {
            HealthManager.Instance?.GainLife(1);
            return PickMiniGame();
        }
        else
        {
            HealthManager.Instance?.LoseLife();
            if (HealthManager.Instance != null && HealthManager.Instance.GetCurrentLives() <= 0)
            {
                return GameOverScene;
            }
            return PickMiniGame();
        }
    }

    public static string CompleteMiniGame(bool won)
    {
        if (won)
        {
            return PickMiniGame();
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
