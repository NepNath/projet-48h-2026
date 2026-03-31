Bon en gros il y a deux nouvelles fonctions dans notre jeux!

`SceneFlow.cs` sert à décider quelle scène doit venir après une autre c'est un peu elle qui fait la logique / boucle
`TransitionManager.cs` sert à faire l’animation visuelle entre les scènes


........`SceneFlow.cs`........

Ce script ne montre rien à l’écran, il sert juste à organiser la boucle du jeu. Il dit au jeu : “après cette scène, charge celle-là” pour faire simple (fin comment je comprends surtout mdr) Il évite de mettre la logique de navigation partout dans les mini-jeux.

`public static class SceneFlow`
La classe est `static`, donc on peut appeler ses fonctions directement depuis n’importe quel script, sans créer d’objet dans une scène

`MainMenuScene`
Contient le nom exact de la scène du menu principal

`QuestionScene`
Contient le nom exact de la scène du quiz

`MiniGameScenes`
Tableau qui contient les mini-jeux possibles Quand le quiz est fini le script prend une scène dans cette liste (btw a chaque nouveau jeux faut update)

`ResetRun()`
Remet la boucle à zéro comme un reset

`StartRun()`
Déclenche le lancement d’une nouvelle partie en gros la fonction remet la boucle à zéro puis renvoie `Question`, donc le jeu commence toujours par le quiz (ça peut changer j'ai juste check le diagramme)

`CompleteQuiz(bool correct)`
Appelée quand le joueur termine le quiz, Le paramètre `correct` indique si la réponse était juste ou non -> et donc va choiisir le prochain mini jeu

`CompleteMiniGame(bool win)`
Appelée quand un mini-jeu se termine. Le paramètre `win` indique si le joueur a gagné ou perdu, Dans la version actuelle, cette fonction renvoie simplement vers `Question` le goal c'est de faire un score et une vie ducoup.....

`PickMiniGame()`
Choisit quel mini-jeu lancer parmi ceux présents dans `MiniGameScenes` le choix tourne en boucle pour alterner entre les mini-jeux


........`TransitionManager.cs`........

Ce script gère l’effet de transition entre les scènes.il fait un fondu noir le fameux fade (in/out) avant et après le chargement pour que le changement soit plus propre (encore un peu bugué et pas si beau donc on peut le strike)

`public static TransitionManager Instance`
Garde une seule instance du manager pendant toute la partie Comme ca le système de transition continue même quand la scène change

`Awake()`
Cette fonction s’exécute dès que l’objet est créé elle vérifie qu’il n’existe pas déjà un autre `TransitionManager` puis elle garde celui-là avec `DontDestroyOnLoad`

`BuildOverlay()`
Crée un canvas et une image noire plein écran cette image sert de voile pour faire le fade

`LoadScene(string sceneName)`
Fonction publique appelée par les autres scripts elle lance le chargement d’une scène avec transition au lieu de faire un chargement brutal

`LoadRoutine(string sceneName)`
Coroutine principale du chargement Elle fait dans l’ordre : fade out noir, chargement de la scène, puis fade in (merci Nathan mdr)

`FadeTo(float targetAlpha, float duration)`
Anime la transparence de l’écran noir Si `targetAlpha` vaut 1 l’écran devient noir Si `targetAlpha` vaut 0 ’écran redevient visible

En résumé, `SceneFlow.cs` décide quoi charger et `TransitionManager.cs` rend le changement plus joli :DDD


DCP voici toute les caroutine que j'ai pu faire
TransitionManager.LoadRoutine(...)
TransitionManager.FadeTo(...)
QuestionSystem.Transition(...)
GameManager.LoadNextScene(...)
KeyManager.LoadNextScene(...)


voili voilu, je tiens juste a précisé que je n'ai pas tout codé a la main, j'ai check TUTO YT, doc, et directement des projets unity de jeu deja fait, et je me suis aidé de l'ia surtout pour build car j'ai bien galéré 