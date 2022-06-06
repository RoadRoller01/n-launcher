# n launcher
n-launcher is a launcher for any games that you publish on GitHub

# Setup
### download the latest release then edit config.json as such:

put the username of the account that has got your games in the place of "roadroller01" to see your games in the launcher  
https://github.com/RoadRoller01/n-launcher/blob/12e426abb66f0d60e32362838363f43d57190c73/n-launcher/config.json#L2

to add your games to the launcher, the game repository must have the "game" topic as its first topic as such.  
<img src="https://user-images.githubusercontent.com/48331562/172175903-b78fba23-a88b-4333-bc07-a26b86761619.png" width="385px" align="center">

## Games release
game release assets must have the game compressed with zip format as its first asset,  
the zip file structure must be like this 
```
+ *.zip
+--+ game name folder
|  +-- game files
|  +-- game name.exe
```
game name is the game repository name ("_" and "-" replaced with " "),  
[example](https://github.com/RoadRoller01/NSimulator/releases/latest).



# Development requirement
.net6  
having the will to live 

## Development ideas
launcher for the launcher
