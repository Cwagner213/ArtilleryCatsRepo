Artillery Cats Unity Project Files
-----------------------------------------------------
Artillery Cats has to be set up in Unity in order to make changes to it.  Its written in C# and uses minimal 
plugins but the ones that are used will be listed in their own section at the bottom.  This project uses the standard Unity gitignore.


***The Player class is a bit jumbled because Commands to the server need to be sent through the Player GameObject, a remedy would be to break it down into files all attached the same GameObject which I will get around to eventually.  ***

The game is Online Multiplayer only, I made it specifically to learn how Online Multiplayer works with Unity with the purpose of using this knowledge on the next game I am making.  New levels should be easy enough to make and I will post a small guide on how to do it a bit further down.  But the majority of it at this point can be done with drag and dropping tiles down assuming you attach the right class to the tile and label it correctly.  

New cats can be made as well but you will have to follow the basic setup I have laid out for their animations unless you want to delve quite deep into how the game is setup (which feel free I'm just saying that might be painful)  To add a special ability to a new cat you will have to add a method to the Player class next to the rest of the special abilities.  Make sure to add the ability to the Ability Directory with a number corresponding to the number of the cat you are making.  (reasonably your first new cat would use the number 9 therefore the ability would have to also be 9)

