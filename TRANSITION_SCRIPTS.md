# Explication des deux scripts

`SceneFlow.cs` sert à décider quelle scène doit venir après une autre.
`TransitionManager.cs` sert à faire l’animation visuelle entre les scènes.

`SceneFlow.cs`

Ce script ne montre rien à l’écran, il sert juste à organiser la boucle du jeu. Il dit au jeu : “après cette scène, charge celle-là”. Il évite de mettre la logique de navigation partout dans les mini-jeux.

`public static class SceneFlow`
La classe est `static`, donc on peut appeler ses fonctions directement depuis n’importe quel script, sans créer d’objet dans une scène.

`MainMenuScene`
Contient le nom exact de la scène du menu principal. Ça évite les fautes de frappe dans le code.

`QuestionScene`
Contient le nom exact de la scène du quiz.

`MiniGameScenes`
Tableau qui contient les mini-jeux possibles. Quand le quiz est fini, le script prend une scène dans cette liste.

`ResetRun()`
Remet la boucle à zéro. Ici, ça remet juste l’index des mini-jeux au début pour repartir proprement.

`StartRun()`
Déclenche le lancement d’une nouvelle partie. Cette fonction remet la boucle à zéro puis renvoie `Question`, donc le jeu commence toujours par le quiz.

`CompleteQuiz(bool correct)`
Appelée quand le joueur termine le quiz. Le paramètre `correct` indique si la réponse était juste ou non. La fonction choisit ensuite le mini-jeu suivant dans la liste.

`CompleteMiniGame(bool win)`
Appelée quand un mini-jeu se termine. Le paramètre `win` indique si le joueur a gagné ou perdu. Dans la version actuelle, cette fonction renvoie simplement vers `Question`.

`PickMiniGame()`
Choisit quel mini-jeu lancer parmi ceux présents dans `MiniGameScenes`. Ici, le choix tourne en boucle pour alterner entre les mini-jeux.

`TransitionManager.cs`

Ce script gère l’effet de transition entre les scènes. Son rôle est de faire un fondu noir avant et après le chargement pour que le changement soit plus propre.

`public static TransitionManager Instance`
Garde une seule instance du manager pendant toute la partie. Comme ça, le système de transition continue même quand la scène change.

`Awake()`
Cette fonction s’exécute dès que l’objet est créé. Elle vérifie qu’il n’existe pas déjà un autre `TransitionManager`, puis elle garde celui-là avec `DontDestroyOnLoad`.

`BuildOverlay()`
Crée un canvas et une image noire plein écran. Cette image sert de voile pour faire le fade.

`LoadScene(string sceneName)`
Fonction publique appelée par les autres scripts. Elle lance le chargement d’une scène avec transition au lieu de faire un chargement brutal.

`LoadRoutine(string sceneName)`
Coroutine principale du chargement. Elle fait dans l’ordre : fade out noir, chargement de la scène, puis fade in.

`FadeTo(float targetAlpha, float duration)`
Anime la transparence de l’écran noir. Si `targetAlpha` vaut 1, l’écran devient noir. Si `targetAlpha` vaut 0, l’écran redevient visible.

En résumé, `SceneFlow.cs` décide quoi charger et `TransitionManager.cs` rend le changement plus joli.

Petite notion utile : la coroutine

Une coroutine, c’est une fonction spéciale qui peut se mettre en pause puis reprendre plus tard. C’est pratique pour faire attendre le jeu sans bloquer toute l’application. Dans Unity, on l’utilise souvent pour les transitions, les délais avant de changer de scène, ou les petites animations temporisées.

Exemple simple :

```csharp
using System.Collections;

IEnumerator ThrowbackToMainMenu(int seconds)
{
    yield return new WaitForSecondsRealtime(seconds);
    SceneLoader.LoadMainMenuScene();
}
```

Ici, la coroutine reçoit un paramètre `seconds`. Quand on l’appelle avec `StartCoroutine(ThrowbackToMainMenu(3));`, elle s’exécute jusqu’au `yield return`, puis elle s’arrête pendant 3 secondes. Après ça, elle reprend et charge la scène du menu.

Autre exemple :

```csharp
IEnumerator Counter()
{
    int count = 0;

    while (count < 10)
    {
        Debug.Log(count);
        count++;
        yield return new WaitForSecondsRealtime(1f);
    }

    Debug.Log("finito");
}
```

Là, la boucle tourne 1 fois par seconde. À chaque tour, `count` augmente, puis la coroutine attend 1 seconde avant de reprendre. Quand `count` arrive à 10, la boucle s’arrête et le message `finito` s’affiche.

Quelques `yield return` utiles :

```csharp
yield return null;                        // attend la prochaine frame
yield return new WaitForSeconds(1f);      // attend X secondes (lié à Time.timeScale)
yield return new WaitForSecondsRealtime(1f); // attend X secondes réelles
yield return new WaitForFixedUpdate();     // attend le prochain FixedUpdate
yield return new WaitForEndOfFrame();      // attend la fin de la frame
yield return new WaitUntil(() => condition == true); // attend qu'une condition soit vraie
yield return new WaitWhile(() => condition == true); // attend qu'une condition soit fausse
yield return StartCoroutine(AutreCoroutine()); // attend la fin d'une autre coroutine
```
