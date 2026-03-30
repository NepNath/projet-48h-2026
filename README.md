# projet 48h 2026

### Convention de nommage des commits : 

**X.X.X-branche/fonctionnalité - contenu du commit**

Exemple : 

*0.2.5-Player_Movement - adding dash for character*

Pour la création de branche, il faut créer une branche en se basant sur la branche "*0.0.0-main_branch*".

Les noms de branches/fonctionnalité doivent **TOUJOURS** être en snake case et le contenu de commit **TOUJOURS** en anglais, si vous galérez à faire la trad, utilisez deepl.

Pour le versionning : 
* le premier numéro indique la version de release (1.0.0 = jeu finit et sorti)
* le second numéro indique la version majeur du projet (un ajout de feature par exemple)
* le troisième numéro indique les hotfix pour la version actuelle.

Merci de ne rien push sur les branches principales et de rester dans vos branches de dev ! c'est moi (NepNath) qui gère les versions et les merge sur la branche principal, si vous avez une question quelconque, venez me demander ! 

Si vous voulez créer une branche de dev, créez la à partir de la dernière branche de version du jeu. 

Dans vos branches de dev, créez une scène en plus de celle de base et ne faite AUCUN travail sur la scène "*main*" pour pas engendrer de conflict lors des merge. Nommez également vos scène avec le nom de votre branche. Par exemple si vous travaillez sur les "Player_Movement" nommez votre scène "Player_Movement".
