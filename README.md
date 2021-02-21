# CatAndDungeon
喵喵与地下城是一个以开源（非商用）为目的开发，自创背景和规则的TRPG游戏。在设计时参考了DND和Pathfinder 的游戏规则与世界观、炉石传说和WOW的职业设定，以及Thaumcraft和Minecraft的美术风格，并从简化规则的角度出发对相关内容进行了简化。 游戏以DM（游戏主持人）-PC（玩家）形式进行，主持人在其对应的DM模式下对游戏全局拥有完全的控制能力。游戏构建了 进行一场TRPG冒险所需的地图、角色、导航、法术、背包、战斗、随机数值等功能系统，网络联机功能也在开发之中
# 地图编辑器
在地图编辑器中，你可以任意编辑9个由1600块可扩展编辑地块的小型地图，并使用数字键在任意模式下进行无缝切换。  

每个地块可独立选择材质和颜色。编辑器已经内置了9级地形设计，方便地图创建者实现三维地形。  

游戏内置了25种以奇幻/Minecraft为背景设计场景物件（模型借助ProBuilder自行设计，之后会加入更多）  。

通过图块设置面板可自由设置部件上每个部分的材质和颜色。在编辑地图的同时，系统将使用NavMesh进行寻路网络的实时生成，使设计者无需担心自己设计的复杂地形中角色的导航问题。

在游戏模式下，DM（主持人）可通过游戏上的世界时间刻度调整游戏内的时间，以获得不同的光照和阴影效果，或是通过洞穴/野外按钮进行光照模式的切换。

# 角色与职业
游戏制作了独立设计外观和动作的11个RPG职业，这些职业可按其选择的阵营分为58个阵营职业分支名称，每个职业拥有两种可影响角色成长或冒险故事的职业天赋，
根据职业和出身，玩家可以从每个职业9种不同的皮肤中选择适合自己的角色模型。

职业包括3种：  
法系职业：法师擅长功能系和AOE法术，萨满掌控着元素之力，术士沟通虚空与黑暗  
神职职业：牧师擅长治疗和守护，侍僧拥有古神的威能，圣骑士成为团队的剑与盾，黑骑士依靠绝对的力量释放怒火  
其他职业：战士擅长各种直接进攻的方式，德鲁伊擅长利用自然的力量，猎人与弓箭和动物为伴，成为强大的独行者，游荡者穿梭于暗影之中，使用利刃和陷阱为敌人带来毁灭  

玩家和管理员可以通过角色卡建立界面依照向导建立完整的角色卡，选择其职业和天赋、通过掷骰或DM直接修改的方式获得其基础属性，
根据选择的出身调整角色外观和角色语音，并丰富角色的法术书，在创建角色结束后，还可将其保存为文件进行快速备份。

游戏提供了能够显示角色所有自身状态数据的角色面板、法术面板和背包面板，并接受任何形式的编辑调整，主持人可从角色面板位置
快速使用无来源治疗/伤害/法术回复/法术伤害/调整数值/获取经验值等功能

# 法术书系统
无论是强大的AOE法术，还是精准的单体射击，或是改变战局的召唤技能，游戏中的107种法术原型均由元素（代表12种法术属性）和领域（9种释放方式）为核心构建而成。

通过自定义法术轮盘，玩家可将任意一种元素与领域组合在一起，形成一种法术原型，每种法术还可根据玩家投入的元素和领域不同拥有不同的法术强度和法术消耗。

借助游戏内置的法术动态执行链，主持人或玩家可以脱离内置法术的限制，以全中文描述或缩写脚本的方式自由构建法术原型，
决定法术的施法方式、目标选择和法术效果，甚至创建连环法术，或为同一个法术原型创建多种衍生效果。（具体修改规则见工程目录下的default\skills.skillset）

# 状态与召唤物
从传统RPG的燃烧、冰冻到法师护甲和神圣护佑，再到改变战局的终极技能，游戏构建了52种自带特效的角色附加状态，这些角色状态均可
由修改规则文件的形式被自由调用，并获得自定义的强度和持续时长。在角色进行存档时，这些Buff也可以与角色一同存储。

游戏内置了21种外形、属性各异的召唤生物，这些神秘异界生物往往具有独一无二的强大能力：冰冻受到伤害的对手，释放大范围的寒冰爆炸
、对友军群体的法术回复，或是在召唤时直接压倒敌人。

此外，游戏还预置了18种静态召唤物体，它们是掌控元素之力的图腾，也是散发着圣光之力的神龛；是热气腾腾的岩浆地面，也是冰冷刺骨的
寒冰风暴；它们象征着死亡，掌控着虚空，也预示着生机与幻影。

所有的召唤物和静态召唤物体都是与法术动态执行核心兼容的，因此，玩家或主持人可以使用这些召唤物的名称创建自己任意的自定义法术类型。

# 战斗系统
游戏建立了完整的回合制CRPG战斗流程，在战斗开始时，DM可通过界面上方的先攻检定面板进行检定，获取本轮的行动顺序。在战斗模式下，各个战斗角色
拥有自身HP、MP等属性，并可根据自身本轮剩余行动点数进行动作规划。

在角色准备移动时，可由地形上方显示的预测路径预览行动的本轮消耗，并可根据预测路径的颜色获知剩余行动力情况；在角色使用技能或进行攻击动作时，
也可快速预览角色的施法或攻击距离，便于玩家快速进行决策。

玩家的施法和攻击行动可任意调取游戏内置的DICE或COMBAT系统，使用者可快速设置其基础值、加值和环境加值，并根据需求进行若干次掷骰得到结果。

# 关于仓库
由于主要功能的开发刚刚完成，代码还没有进行整理，资源也没有去重合归类，等开发全部完成就会将这些事项逐步补上，所以不建议现在Clone或者下载。

# 版权相关

**版权规则（版权所属部分）：禁止商用，也不能用来交大作业，其他用途随意取用，本人保留软件著作权，引用代码请署名，欢迎二次开发**

引用他人版权部分  
策划部分：  
一部分世界观或设定来源于Pathfinder（我买了一部核心规则书来着），炉石传说（老玩家了）和魔兽世界。

程序部分：
使用Unity引擎作为基础。

美术部分：  
大部分地块、以及元素/领域图标来自Minecraft和Thaumcraft 6（然而作者自己停更了），如果有版权需要的话我之后会尝试自己画替代这些内容。  
字体使用方正像素体和阿里巴巴普惠体，前面那个还没买版权。 
所有静态模型都是作者自己使用ProBuilder建的模（省钱了），可以拿走用。  
一些UI资源来自RPG Maker MV（有正版资源使用权）  
所有Buff图标都是喵喵画的（一些是以RPG Maker MV 图标为基础，所以如果想拿走用要先买一份RPG Maker）  
大部分看起来面数就很多的人物模型都来自Synty Studios™创作的POLYGON系列，我买了一部分正版，然后买不起了，剩下的来自淘宝盗版店铺，对于此事
我会心存愧疚，等我攒够了钱就还上。

音乐部分：  
展示时所用地图BGM来自RPG Maker MV提供的音乐和魔兽世界的BGM，男/女角色语音使用了炉石卡牌中一些非主角英雄牌的配音，这个目前没什么解决方法，头疼。
