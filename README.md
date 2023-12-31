# CloneCrash
## Screenshots
<div align="center">
 
</div>
<div align="center">
 <img src="https://github.com/FikretGezer/CloneCrash/assets/64322071/fbcaaab3-0772-40ff-9539-2dfa7a5ccb63" width="270" height="480">
 <img src="https://github.com/FikretGezer/CloneCrash/assets/64322071/1ab63a6d-4dec-4eb9-80c9-032e6751588f" width="270" height="480">
 <img src="https://github.com/FikretGezer/CloneCrash/assets/64322071/da6b7c68-0a98-43f3-9f77-9efdfa0f61bd" width="270" height="480"> 
</div>

## What is about?
Clone Crash is basically a Match 3 game that I tried to build after seeing these kinda games being popular around the world.

## How the game works?
### Detecting Matches
* I added tags each one the pieces.
* When players swipe towards any direction, I searched horizontal and vertical lines that contains the swiped pieces and if there is any matches after swiping, I added them in a matched list, if they're match of 4 or 5, I spawn special pieces like color, row, column bomb and I destroyed the matched ones.
* Afte the matches, I moved the board and spawned new pieces. (This could be done with object pooling for the better performance.)
* If there is not a match, I swiped back the pieces.
* But there was an issue which is what if players matched 4 or 5 pieces when they swiped but matches are not in the same line. I wrote another code block for this to be able to detect if matches are in the same line. And if they are, I'd spawned a special piece.
* During this process I made sure that players won't be able to swipe another piece while this process on going.
* Also sometimes there was another issue appearing whichi is there were no possible move on the board. If this occurs, I wrote a code to suffle the board.
* And lastly write a code block to detect possible matches to show players if they stuck during the game.


### Levels
* I created scriptable objects for levels and also another scriptable object to control levels easily while loading or doing something else.
* Each level object contains;
  - Which pieces can be spawned because pieces spawns randomly,
  - Which tiles can be spawned like **concrete** (Breakable and doesn't contain any piece inside of it), **ice** (Breakable and contains a piece inside of it but pieces can't be moved), **jelly** (Breakable and contains a piece inside of it and pieces can be moved) or **empty** tiles,
  - What's the layout of the board (5x5, 5x7, etc.),
  - How many moves players have,
  - What are the objectives.

### Loading and Saving
* I created a class to hold level indexes and other things like sound effects, musics and their volumes, etc.
* I created an instance from this class.
* If players passes the level, I was getting the current level's index and increasing it one using the instance of the class (this will only works if there is another level in level container scriptable object, if there is no other level, there won't be any increasing).
* I did the same thing for the sounds too.
* After all of this, I created a script to save this data on a local path using IO operations and Binary Formatters.
* This saving was working when application closed or players switched between different scenes. And everytime players opens the game, game was detecting the save file and loading on the start.
