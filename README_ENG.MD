Intro
---
This MOD helps you draw "one" picture in your local planet. At the end, you'll achieve exactly what I did in this vedio: https://www.bilibili.com/video/BV1Kf4y1H71w


Before your start:
---
1. make sure `.net framework 4.6` or later version(s) is installed in your OS. `.net framework 4.6` or later version(s) is installed by default by Win10 so in general, you do not need to install anything in this step.
2. Prepare an image with good quality. My program accepts `.jepg`, `.jpg` and `.png`.
3. Install BepinEX.
4. Prepare a planet: flatten its terrain. The recetangle-area between `30N 0E` and `30S 110E` should be flattened and colored red. (The color itself is not important, either red or blue or whatever, but must be colored.) 


Ready to start:
---
Great, let's start! To draw a pic, we need to first process the original image and then load it to BepInEx.


Process an Image:
---
1. Put your picture under `release` folder.  For demo purpose, I use [azur.jpg](https://www.pixiv.net/artworks/63067913), which is already inside the folder.
2. Open Powershell inside `release` folder (Holding `shift`-> rightclick folder->Open Powershell Window here). Run command `.\PhotoProcess.exe [image] [iter_num]`. For example:
    ```shell
    .\PhotoProcess.exe .\azur.jpg 15
    ```
    iter_num specifies the maximum iterations used to run kmeans algorithm. If it runs slow (though it should not), change it to a smaller number (around 5, maybe). This step is where `.net framework 4.6` or later version(s) is required.
3. Now, 4 files are created inside `release\out` folder. `preview_final.jpg` is what you expect to see inside the game. Please do not modify `out` folder until you finish drawing and uninstall the mod.
4. Open the config file `release\yzanh.DSP.ReformColorDraw.cfg`, replace `C:/asdas` with the absolute path to `out` folder. Mine is:
```
Out_Foloder_Abs_Path = C:\Users\testuser\Download\release\out
```
5. Copy the config file `yzanh.DSP.ReformColorDraw.cfg` to `BepInEx\config` folder.
6. Copy the .dll file  `ReformColorDraw.dll` to `BepInEx\plugins`


Start the DSP Game.
---
1. Go to the planet you prepared.
2. Hit `Enter` in your keyboard.
3. You should see some color-changes already made to the terrain. Changes are not complete until you manually save the game and reload it. 
4. Now you should see the pic. Congrats.


After Drawing:
---
1. Please uninstall the .dll file, in case you mistakenly hit `Enter` later in the game.
2. It's safe to delete `out` folder after you finish drawing, but not before.
3. You won't be able to draw 2 pics. Only one. because of the restriction given by the number of available colors (16 pre-set colors + 16 customized colors)


Others:
---
1. All source codes used by this mod are available [here](). You can compile `.exe` or `.dll` by yourself using these source codes.
2. Please use this mod at your own risk. I have no idea if it will crash in later updates, and no idea if it affects your milkway data.
3. Feel free to copy or modify the source codes.

