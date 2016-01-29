![](http://i.imgur.com/oYXoYMQ.png)

# HFAPI
HFAPI est une librairie .NET pour pouvoir utiliser certaines fonctions du forum hack-free dans vos programmes .NET

### Introduction
Pour utiliser HFAPI vous devez d'abord l'ajouter en référence : 
> using HFAPI;

Ensuite vous devez instanciez la classe : 
>User.Connect("Username", "Password");

Une fois fais, vous pouvez dès lors obtenir plusieurs informations sur l'User connecté en jouant avec les différantes propriétés disponibles.

### Fonctionnalités

>User.Bet(50);

Permet de parier 50 crédits contre le système

>User.Bet("Chewbaka", 50, BetType.Private);

Permet de parier 50 crédits contre Chewbaka

>User.PrivateMessage("Chewbaka", "Sujet", "Message", true, true, true);

Permet d'envoyer un message privé à Chewbaka dont le sujet est "Sujet", le message est "Message", vous voulez y afficher votre signature, sauvegarder une copie et recevoir un accusé de reception

>User.SendShout("Hello world !");

Permet d'envoyer "Hello world !" dans la shoutbox

>User.ReportProfile("1", "il est gay");

Permet de report le profile numéro 1, l'administrateur, parce que "il est gay"

