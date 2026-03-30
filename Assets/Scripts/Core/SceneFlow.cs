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
        return QuestionScene;
    }

    public static string CompleteQuiz()
    {
        return PickMiniGame();
    }

    public static string CompleteMiniGame()
    {
        return QuestionScene;
    }

    static string PickMiniGame()
    {
        string scene = MiniGameScenes[nextMiniGameIndex];
        nextMiniGameIndex = (nextMiniGameIndex + 1) % MiniGameScenes.Length;
        return scene;
    }
}
