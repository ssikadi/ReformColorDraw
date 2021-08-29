介绍
---
这个mod帮助你在星球上用地基画画. 最终效果如: https://www.bilibili.com/video/BV1Kf4y1H71w


准备工作：
---
1. 安装 `.net framework 4.6` （或更新版本）. 因为Win10 自带`.net framework 4.6`（或更新版本）所以你应该可以直接跳过这一步。
2. 准备一张`.jepg`, `.jpg` 或者 `.png`的图。最好清楚一些，虽然最后都一样糊。
3. 安装 BepInEx框架.
4. 准备一个铺好了红色地基的星球。只需铺 `北纬30 东经0` 和 `南纬30 东经110` 之间的长方形区域。（颜色本身无所谓，红色蓝色都可以。但一定要图上颜色。）


开始：
---
有两件事要做，第一件是处理图片数据，第二件是把数据导入框架。


处理图片数据:
---
1. 把准备好的图片放到`release`文件夹. 现在`release`文件夹内已经有 [azur.jpg](https://www.pixiv.net/artworks/63067913)， 我会用它做示范。
2. 在`release`文件夹内打开 Powershell。(按住 `shift`-> 右击文件夹->打开 Powershell). 
3. powershell 中输入命令`.\PhotoProcess.exe [image] [iter_num]`. 例如:
   
    ```shell
    .\PhotoProcess.exe .\azur.jpg 15
    ```
    iter_num（15） 限制 kmeans 算法的循环次数上限. 如果代码一直跑不完，可以把15改小一些（比如说5)。`.net framework` is required here.

4. `release\out` 文件夹内输出了4个东西. `preview_final.jpg` 是最终预览图。 `xml`文件不用碰。在绘制结束，存档完成，卸载好mod之前请不要删除`out`文件夹内的东西。
5. 打开 `release\yzanh.DSP.ReformColorDraw.cfg`, 把 `C:/asdas` 替换成 `out` 文件夹的绝对路径(从C或其他硬盘开始的路径). 例如我的是:
   
    ```
    Out_Foloder_Abs_Path = C:\Users\testuser\Download\release\out
    ```
6. 把`yzanh.DSP.ReformColorDraw.cfg`复制到 `BepInEx\config` 文件夹.
7. 把mod文件 `ReformColorDraw.dll` 复制到 `BepInEx\plugins`


启动戴森球
---
1. 飞到准备好的星球。
2. 键盘上敲`回车`
3. 成功的话，数据已经**部分**加载好了。保存一下存档，再读取一下使修改生效。


最后：
---
1. 记得卸载 .dll mod文件, 以免之后忘记了乱敲 `回车`。
2. 画完并存档后就可以把`out`文件夹删了。画之前不要删。
3. 由于地基颜色限制（只有16预设颜色+16自定义颜色），本mod只支持一副图片。


其他：
---
1. mod源代码在[这里](https://github.com/ssikadi/ReformColorDraw/tree/main/source_codes)。可以用来自己编译`.exe` or `.dll`。
2. 使用mod有风险。不保证以后更新是否会崩溃。也不保证上传电量正常。
3. 请随意使用或修改mod源代码。


