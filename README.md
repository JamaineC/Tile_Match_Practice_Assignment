# Tile-Matching Game – Unity & C#  
## Project Overview  
This project is a **Computer Games and Graphics** assignment from **Aberystwyth University**, focusing on **game development using Unity and C#**. The project involves developing a **tile-matching puzzle game** using **Unity’s Tilemap system** with features such as **sand mechanics, power-ups, level transitions, and an interactive UI**. The game includes at least **two levels of varying difficulty**, and a **Game Controller** that manages game logic, tile interactions, and special effects like bombs and color-matching power-ups.  

## Prerequisites  
Unity 2022.3 or later, Visual Studio Code with Unity and C# extensions installed.  

## Installation & Setup  
Clone the repository: `git clone https://github.com/your-repo/tile-match-unity.git && cd tile-match-unity`  
Open the project in Unity Editor: `Open Unity Hub > Open Project > Select tile-match-unity`  
Run the game in **Play Mode** to start testing.  

## Game Features  
The game features **grid-based tile matching**, where players interact with tiles using **mouse clicks** to swap and remove matching sets. The **GameController** script manages **tile initialization, power-ups, and grid refilling mechanics**. **Sand mechanics dynamically adjust falling tiles** after matches. A **pause menu and rules screen** are implemented but currently experience UI interaction issues.  

## Core Components  
The **GameController** manages **game states, tile removal, and power-ups**, while the **SceneLoader** ensures **smooth scene transitions between menus and levels**. The **GridManager** stores data between scenes to maintain **game state persistence**.  

## Troubleshooting  
If **UI elements are unresponsive**, check **panel visibility settings and event handling**. If **tile interactions do not work**, verify **power-up logic and grid refilling mechanisms**. If **game logic does not transition correctly**, ensure **GameState enum updates properly**.  

## Future Improvements  
Enhancements could include **fixing UI interactions**, **improving pause menu functionality**, **adding more power-ups**, and **expanding level designs**.  

## License  
This project is released for **educational and research purposes**. Contributions and improvements are welcome!  
