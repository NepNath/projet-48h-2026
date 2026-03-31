using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private float fadeOutDuration = 0.15f;
    [SerializeField] private float fadeInDuration = 0.08f;

    Canvas _canvas;
    Image _overlay;
    bool _isTransitioning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();
    }

    void BuildOverlay()
    {
        var canvasObject = new GameObject("TransitionCanvas");
        canvasObject.transform.SetParent(transform, false);

        _canvas = canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = short.MaxValue;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        var overlayObject = new GameObject("FadeOverlay");
        overlayObject.transform.SetParent(canvasObject.transform, false);

        _overlay = overlayObject.AddComponent<Image>();
        _overlay.color = new Color(0f, 0f, 0f, 0f);
        _overlay.raycastTarget = false;

        var rect = _overlay.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public static void LoadScene(string sceneName)
    {
        if (Instance == null)
        {
            var go = new GameObject("TransitionManager");
            Instance = go.AddComponent<TransitionManager>();
        }

        // Ensure SoundManager plays appropriate music for the target scene immediately
        if (SoundManager.Instance != null)
        {
            if (sceneName == SceneFlow.MainMenuScene)
                SoundManager.Instance.PlayMenuMusic();
            else if (sceneName == SceneFlow.QuestionScene)
                SoundManager.Instance.PlayQuestionMusic();
            else if (sceneName == SceneFlow.GameOverScene)
                SoundManager.Instance.PlayGameOverMusic();
            else
                SoundManager.Instance.PlayMiniGameMusic();
        }

        Instance.StartCoroutine(Instance.LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        if (_isTransitioning)
            yield break;

        _isTransitioning = true;
        yield return FadeTo(1f, fadeOutDuration);

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' not in Build Settings. Loading fallback scene.");
            sceneName = SceneFlow.QuestionScene;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
            yield return null;

        yield return null;
        yield return FadeTo(0f, fadeInDuration);
        _isTransitioning = false;
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (_overlay == null)
            yield break;

        float startAlpha = _overlay.color.a;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            _overlay.color = new Color(0f, 0f, 0f, targetAlpha);
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            _overlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        _overlay.color = new Color(0f, 0f, 0f, targetAlpha);
    }
}
