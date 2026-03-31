using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GestionnaireChoco : MonoBehaviour
{
    [Header("Chocolat")]
    public Image imageDuBouton;
    public Image BatonChocolatImage;
    public List<Sprite> etapesChocolat;
    private int indexActuel = 0;

    [Header("Timer Texte")]
    public TextMeshProUGUI texteTimer;
    public float tempsTotal = 10f;
    private float tempsRestant;
    private bool jeuFini = false;

    void Start()
    {
        tempsRestant = tempsTotal;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!jeuFini && tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;

            if (texteTimer != null)
                texteTimer.text = tempsRestant.ToString("f1") + "s";

            if (tempsRestant <= 0)
                TerminerJeu();
        }
    }

    public void MangerUnMorceau()
    {
        if (jeuFini) return;

        if (indexActuel < etapesChocolat.Count - 1)
        {
            indexActuel++;
            imageDuBouton.sprite = etapesChocolat[indexActuel];
        }
        else
        {
            Die();        // cache les images
            Finish(true); // ✅ victoire → mini-jeu suivant
        }
    }

    void TerminerJeu()
    {
        if (texteTimer != null) texteTimer.text = "0.0s";
        Finish(false); // ✅ défaite → QuestionScene
    }

    void Finish(bool win)
    {
        if (jeuFini) return;
        jeuFini = true;
        TransitionManager.LoadScene(SceneFlow.CompleteMiniGame(win));
    }

    void Die()
    {
        if (imageDuBouton != null)
        {
            imageDuBouton.gameObject.SetActive(false);
            BatonChocolatImage.gameObject.SetActive(false);
        }
    }
}