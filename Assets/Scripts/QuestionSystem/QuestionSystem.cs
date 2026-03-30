using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Random = System.Random;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

[System.Serializable]
public class QuestionEntry
{
    public int id;
    public string question;
}

[System.Serializable]
public class AnswerEntry
{
    public int id;
    public string answer;

    public bool IsCorrect; // Optional: Flag to indicate if this answer is correct
}

public class QuestionSystem : MonoBehaviour
{
    [Header("Register Question/Answers")]
    [SerializeField] private List<QuestionEntry> _questionEntries = new List<QuestionEntry>();
    [SerializeField] private List<AnswerEntry> _answers = new List<AnswerEntry>();

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _questionTitle;
    [SerializeField] private GameObject _answerBtnPrefab;
    [SerializeField] private Transform _answersContainer; // Parent container for buttons

    private Dictionary<int, string> _questions;
    private Dictionary<int, List<AnswerEntry>> _answersByQuestionId;
    private List<GameObject> _questionBtnObjects;
    private int _currentQuestionId;

    void InitDict()
    {
        _questions = new Dictionary<int, string>();
        foreach (var entry in _questionEntries)
            _questions[entry.id] = entry.question;

        _answersByQuestionId = new Dictionary<int, List<AnswerEntry>>();
        foreach (var entry in _answers)
        {
            if (!_answersByQuestionId.ContainsKey(entry.id))
                _answersByQuestionId[entry.id] = new List<AnswerEntry>();
            _answersByQuestionId[entry.id].Add(entry);
        }
        _questionBtnObjects = new List<GameObject>(); // Keep track of instantiated buttons for cleanup and if you want to add functionality like disabling them after selection
    }

    void Start()
    {
        Random random = new();
        InitDict();

        // Get random question
        List<int> questionKeys = new(_questions.Keys);
        _currentQuestionId = questionKeys[random.Next(questionKeys.Count)]; // Fixed: removed -1
        _questionTitle.text = _questions[_currentQuestionId];

        SpawnAnswerButtons(_currentQuestionId);
    }

    void SpawnAnswerButtons(int questionId)
    {
        if (!_answersByQuestionId.TryGetValue(questionId, out List<AnswerEntry> answersForQuestion))
        {
            Debug.LogWarning("No answers found for question id: " + questionId);
            return;
        }

        foreach (var answer in answersForQuestion)
        {
            // Instantiate button as child of the answers container
            GameObject answerBtn = Instantiate(_answerBtnPrefab);
            RectTransform answerRect = answerBtn.GetComponent<RectTransform>();
            if (answerRect != null)
            {
                answerRect.SetParent(_answersContainer, false);
                answerRect.localScale = Vector3.one;
            }
            else
            {
                answerBtn.transform.SetParent(_answersContainer, false);
            }
            
            // Set button text
            TextMeshProUGUI buttonText = answerBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = answer.answer;
            }
            
            // Store reference
            _questionBtnObjects.Add(answerBtn);
            
            // The prefab root may be a wrapper object, so search children too.
            Button button = answerBtn.GetComponentInChildren<Button>();
            if (button != null)
            {
                AnswerEntry selectedAnswer = answer; // Capture for closure
                GameObject selectedButtonRoot = answerBtn; // Capture for closure
                button.onClick.AddListener(() => OnAnswerClicked(selectedAnswer, button, selectedButtonRoot));
            }
            else
            {
                Debug.LogWarning("No Button component found on answer prefab instance.");
            }
        }
    }
    
    void OnAnswerClicked(AnswerEntry selectedAnswer, Button clickedButton, GameObject selectedButtonRoot)
    {
        Color resultColor = selectedAnswer.IsCorrect ? Color.green : Color.red;
        if (clickedButton != null)
        {
            Graphic buttonGraphic = clickedButton.targetGraphic;
            if (buttonGraphic != null)
                buttonGraphic.color = resultColor;

            clickedButton.interactable = false;
        }

        foreach (var btnObj in _questionBtnObjects)
        {
            if (btnObj != null && btnObj != selectedButtonRoot)
                HideButtonKeepLayout(btnObj);
        }

        if (selectedAnswer.IsCorrect)
            StartCoroutine(Victory());
        else
            StartCoroutine(Lose());
    }

    void HideButtonKeepLayout(GameObject buttonRoot)
    {
        CanvasGroup canvasGroup = buttonRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = buttonRoot.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Button[] buttons = buttonRoot.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
            button.interactable = false;
    }

    IEnumerator Victory()
    {
        Debug.LogWarning("Victory! Loading next scene...");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("DO NOT EDIT"); // Replace with your actual scene name
    }

    IEnumerator Lose()
    {
        yield return new WaitForSeconds(2f);
        Debug.LogWarning("Defeat! Loading next scene...");
        SceneManager.LoadScene("DO NOT EDIT"); // Replace with your actual scene name
    }
    void OnDestroy()
    {
        foreach (var btnObj in _questionBtnObjects)
        {
            if (btnObj != null)
                DestroyImmediate(btnObj);
        }
        _questionBtnObjects.Clear();
    }
}