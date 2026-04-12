# WzComparerR2-CMS
- 这是一个专为CMS设计的冒险岛提取工具。
- 与其他客户端合作，例如 KMS、GMS、CMS。

# UIRender
- 模拟游戏UI，目前已实现道具栏、装备栏、职业选择窗口、技能窗、冒险岛联盟和世界档案
  
# 帕普拉图斯主页
- 融合[帕普拉图斯主页](https://jancy-49.github.io/Papulatus-Daily/index.html)，实现资讯版块跟进功能。

# Tooltip
- EasyCompare可输出Skilltooltip、Itemtooltip、Eqptooltip、MapTooltip、Mobtooltip、Npctooltip、QuestTooltip和AchvTooltip
- 数据库可批量导出SkillTooltip

# Modules
- **WzComparerR2** 主程序
- **WzComparerR2.Common** 一些通用类
- **WzComparerR2.PluginBase** 插件管理器
- **WzComparerR2.WzLib** wz文件读取相关
- **CharaSimResource** 用于装备模拟的资源文件
- **WzComparerR2.Updater** 程序更新器(未完成)
- **WzComparerR2.LuaConsole** (可选插件)Lua控制台
- **WzComparerR2.MapRender** (可选插件)地图仿真器
- **WzComparerR2.Avatar** (可选插件)纸娃娃
- **WzComparerR2.MonsterCard** (可选插件)怪物卡(已废弃)

# Usage
- **2.x**: Win7+/.net4.8+/dx11.0

# NX OpenAPI
- [了解如何获取API密钥。](https://openapi.nexon.com/guide/prepare-in-advance/)
- 只能用KMS、MSEA和TMS地区的ID。
- [了解有关OpenAPI功能的更多信息。](https://openapi.nexon.com/game/maplestory/)

### SkillID to NX OpenAPI SkillIcon Filename
|   |1st |2nd |3rd |4th |5th |6th |7th |8th |9th|10th|
|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
|0  |K   |F   |P   |C   |L   |H   |O   |B   |M   |A   |
|1  |   |E   |O  |D   |K   |G   |E/F/P   |A   |N   |B   |
|2  |   |H   |N   |A   |J   |F   |M   |D   |O   |C   |
|3  |   |G   |M   |B   |I   |E   |N   |C   |P   |D   |
|4  |   |B   |L   |G   |P   |D   |K   |F   |I   |E   |
|5  |   |A   |K   |H   |O   |C   |E/F?   |E   |J   |F   |
|6  |   |   |J   |E   |N   |B   |E/F?   |H   |K   |G   |
|7  |   |   |I   |F   |M   |A   |J   |G   |L   |H   |
|8  |   |   |H   |K   |   |P   |G   |J   |E   |I   |
|9  |   |   |G   |I   |   |O   |H   |I   |F   |J   |

例如，以下SkillIcon URL 表示技能ID 80000047。
```
https://open.api.nexon.com/static/maplestory/skill/icon/KFHCLHOBIH
```

### ItemID to NX OpenAPI ItemIcon Filename
|   |1st |2nd |3rd |4th |5th |6th |7th |
|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
|0  |    |P   |C   |L   |H   |O   |B   |
|1  |E   |O   |D   |A   |G   |P   |A   |
|2  |H   |N   |A   |J   |F   |M   |D   |
|3  |G   |M   |B   |I   |E   |N   |C   |
|4  |B   |L   |G   |P   |D   |K   |F   |
|5  |A   |K   |H   |O   |C   |L   |E   |
|6  |    |J   |E   |N   |B   |I   |H   |
|7  |    |I   |F   |M   |A   |J   |G   |
|8  |    |H   |K   |D   |P   |G   |J   |
|9  |    |G   |I   |C   |O   |H   |I   |

例如，以下ItemIcon URL 表示道具ID 1802767。
```
https://open.api.nexon.com/static/maplestory/ItemIcon/KEHCJAIG.png
```


# Compile
- vs2026 or higher/.net 8 SDK

# Credits
- **Fiel** ([Southperry](http://www.southperry.net))  wz文件读取代码改造自WzExtract 以及WzPatcher
- **Index** ([Exrpg](http://bbs.exrpg.com/space-uid-137285.html)) MapRender的原始代码 以及libgif
- **[DotNetBar](http://www.devcomponents.com/)**
- **[IMEHelper](https://github.com/JLChnToZ/IMEHelper)**
- **[Spine-Runtime](https://github.com/EsotericSoftware/spine-runtimes)**
- **[EmptyKeysUI](https://github.com/EmptyKeys)**
- **[libvpx](https://www.webmproject.org/code/) & [libyuv](https://chromium.googlesource.com/libyuv/libyuv/)** for video decoding
- **[@KENNYSOFT](https://github.com/KENNYSOFT)** and his WcR2-KMS version.
- **[@Kagamia](https://github.com/Kagamia)** and her WcR2-CMS version.
- **[@Spadow](https://github.com/Sunaries)** for providing his WcR2-GMS version.
- **[@PirateIzzy](https://github.comPirateIzzy)** for providing the basis of this fork.
- **[@seotbeo](https://github.com/seotbeo)** for providing Skill comparison feature.
