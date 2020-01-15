# DEEP (b0.5.1)

Deep is a game developed by Fellowship of the Game.

---

# Changelog

* Fixed being able to pause after dying or winning the game
* Added options to pause menu 
* Added death animation
* Removed non-explosive barrels with explosive barrel textures
* Replaced placeholder props in the large red office
* Removed some collectible itens from the map and moved others
* Small changes to the scenery
* Fixed bullets not going exactly to the crosshair
* Fixed FishHeads never appearing with blue armor versions of skins
* Quality settings changed a bit, also set default quality to High on Windows and Linux (previously Low)
* Reduced distance at which enemy sounds are heard
* Changed MeatShield sounds
* * Improved trident explosion effect
* Improved doors
    - Improved door graphics
    - Doors now close after some time if no one is around
    - Fixed bullets being blocked when doors are opening or closing
* Changes to the AI
    - They now react a bit better when target can't be reached.
* Changes to enemy animations
* Better weapon alignment for enemies with shotguns
* Melee attacks now have a delay before doing damage
* Balance changes:
    - Adjusted weapon damage
    - Adjusted shotgun aim cone
    - Adjusted enemy attack range
    - Changed some enemies weapons and placement
    - Added some new enemies
* Reduced lightmap scale on some props and scenery (decreases build sizes at very little quality loss for shadows)
* Experiments with improved occlusion culling and batching: apparently FPS increases in lower end systems (with bad or no discrete GPUs), but might decrease a bit in other systems (this can happen due to increased CPU usage, should be small but if you notice this change decreases your FPS too much, please leave a comment in the game's page)
* Unity:
    - Updated engine version (this causes the initial configuration dialog to be removed)
    - Updated from LWRP to the new URP
    - Enabled experimental garbage collection
    - Now using IL2CPP
* Experimental WebGL version

---

## Credits

* Abner Santos
* Daniel Sá
* Eleazar Fernando
* Luís Eduardo Rozante
* Óliver Becker
* Vinícius Carvalho

---

## Installation

    - Extract the game files
    - Execute the game (You may need to give the file execution permissions on Linux)

---

## Recommended Requirements

    Requires a 64-bit processor and operating system
    OS: 
    - Windows 7 or later
    - Linux Ubuntu 16.04 or later [other main stream distros are probably compatible]
    Processor: 2C/2T 2.0 Ghz Processor or better
    Memory: 2GB RAM
    Graphics: 2nd Gen Intel HD Graphics or better
    Storage: 110MB available space
    Other: 1024x768 minimum supported resolution

---

## Acknowledgements and Disclaimers

* Some textures from textures.com were used
* The following sounds were used in the game:
    - https://freesound.org/people/LeMudCrab/sounds/163456/
    - https://freesound.org/people/michorvath/sounds/427603/
    - https://freesound.org/people/tommccann/sounds/235968/

---

## Additional Notes

The game will run by default using the DirectX11 and OpenGL graphics APIs in Windows and Linux respectively. It's possible on both systems to run the game with Vulkan by executing it in a terminal with the argument "-force-vulkan".
