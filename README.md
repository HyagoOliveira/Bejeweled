# Bejeweled

This is a Unity project for a game inspired by Bejeweled. You can play a WebGL build [here](https://simmer.io/@hyagogow/bejeweled#.YELHb50NQ6s.link).

* Unity version: **2019.4**
* Current version: **0.1.0**
* [Class Diagram](https://drive.google.com/file/d/1u3dMu-TQH_jV4uT2XrP4JqLsSN9waI-Y/view?usp=sharing)

![Starting the game](/Documentation/bejeweled-start-board.gif)

## How to Use

1. Clone the repo and open it with any **Unity 2019.4 LTS** version. 
2. Open any Scene and play it (all Scenes are playable).

## Game Features

1. The board can have any size or any number of different pieces;
	- The [BoardSettings ScriptObjects](/Assets/Settings/Boards) will hold this information and populate the board at runtime.
2. Player can swap vertically or horizontally 2 adjacent pieces;
3. If the swap results in 3 or more pieces of the same type in adjacent rows or columns, they disappear;
4. If the swap does not result in any match, the pieces should return to their previous positions;
5. When any piece disappears, the pieces above it will fall and new ones will appear from the top of the board, filling all empty spaces;
6. The pieces swap can be done by mouse Click or Drag.

![Gameplay](/Documentation/bejeweled-swapping-pieces.gif)

## Known Bugs
1. The **Hint** functionality is only working for some patterns.

## Disclaimer

Sprites and Audio Clips from [Unity Tiny Gems project](https://github.com/Unity-Technologies/ProjectTinySamples/tree/master/TinyGems)

---

**Hyago Oliveira**

[BitBucket](https://bitbucket.org/HyagoGow/) -
[Unity Connect](https://connect.unity.com/u/hyago-oliveira) -
<hyagogow@gmail.com>
