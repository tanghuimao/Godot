using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adventure.script.Component;
using Godot;
using Newtonsoft.Json;

namespace Adventure.script;
/// <summary>
/// 全局脚本 项目设置中设置自动加载  选择场景
/// </summary>
public partial class GameGlobal : Node2D
{
    //相机震动事件
    public static event  Action<float> CameraShark;
    //调用事件
    public static void OnCameraShark(float obj)
    {
        CameraShark?.Invoke(obj);
    }
    
    //存档文件路径
    // private const string ArchivePath = "user://adventure/archive.json";
    private const string ArchivePath = "user://savegame.save";
    //配置文件路径
    private const string ConfigPath = "user://config.ini";

    //玩家位置点
    private string _entryPoint;
    //玩家位置
    private Vector2 _playerGlobalPosition;
    //玩家方向
    private Vector2 _playerDirection;  
    //生命组件 
    public HealthComponent PlayerHealthComponent;
    //能量组件
    public EnergyComponent PlayerEnergyComponent;
    //专场蒙层
    public ColorRect ColorRect;
    
    public static GameGlobal Instance;
    
    //场景状态保存
    public  Dictionary<string, WorldState> SceneState = new();
    
    public override void _Ready()
    {
        PlayerHealthComponent = GetNode<HealthComponent>("HealthComponent");
        PlayerEnergyComponent = GetNode<EnergyComponent>("EnergyComponent");
        ColorRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
        Instance = GetNode<GameGlobal>("/root/GameGlobal");
        
        //初始化配置文件
        LoadConfig();
    }
    public static GameGlobal GetInstance()
    {
        return Instance;
    }
    

    /// <summary>
    /// 切换场景函数
    /// </summary>
    /// <param name="path">场景文件路径</param>
    /// <param name="entryPoint">玩家位置点</param>
    /// <param name="playerGlobalPosition"></param>
    /// <param name="playerDirection"></param>
    public async void ChangeScene(string path, string entryPoint, Vector2? playerGlobalPosition, Direction? playerDirection, double duration = 0.5D)
    {
        _entryPoint = entryPoint;
        //获取场景树
        var sceneTree = GetTree();
        //保存场景状态
        SaveState(sceneTree);
        //转场开始动画
        await Start(sceneTree, duration);
        
        //切换场景
        sceneTree.ChangeSceneToFile(path);
        //信号等待 场景 加载完成
        var signalAwaiter = ToSignal(sceneTree, "tree_changed");
        await  signalAwaiter;
        if (signalAwaiter.IsCompleted)
        {
            //加载场景状态信息
            LoadState(sceneTree);
            if (string.IsNullOrEmpty(_entryPoint))
            {
                if (playerGlobalPosition != null) _playerGlobalPosition = (Vector2)playerGlobalPosition;
                if (playerDirection != null) _playerDirection = new Vector2((float)playerDirection, 1);
            }
            else
            {
                //遍历节点  EntryPoints分组下节点
                foreach (var node in sceneTree.GetNodesInGroup("EntryPoints"))
                {
                    var entryPointNode = node as EntryPoint;
                    if (entryPointNode.Name == _entryPoint)
                    {
                        _playerGlobalPosition = entryPointNode.GlobalPosition;
                        _playerDirection = new Vector2((float)entryPointNode.Direction, 1);
                        break;
                    }
                }
            }

            if (_playerGlobalPosition != Vector2.Zero || _playerDirection != Vector2.Zero)
            {
                //获取当前场景  调用SetPlayerPosition函数
                sceneTree.CurrentScene.Call("SetPlayerPosition", _playerGlobalPosition, _playerDirection);
            }
            End(sceneTree, duration);
            _playerGlobalPosition = Vector2.Zero;
            _playerDirection = Vector2.Zero;
        }
    }

    /// <summary>
    /// 保存场景信息
    /// </summary>
    /// <param name="sceneTree"></param>
    private void SaveState(SceneTree sceneTree)
    {
        //获取场景名称
        // var baseName = sceneTree.CurrentScene.SceneFilePath.GetFile().GetBaseName();
        var baseName = sceneTree.CurrentScene.Name;
        //移除场景状态
        SceneState.Remove(baseName);
        //敌人信息 敌人路径
        List<string> enemies = new();
        var nodesInGroup = sceneTree.GetNodesInGroup("enemies");
        foreach (var node in nodesInGroup)
        {
            //获取敌人路径
            enemies.Add(GetPathTo(node));
        }
        //添加场景状态
        SceneState.Add(baseName, new WorldState{ SceneName = baseName, Enemies = enemies});
    }
    /// <summary>
    /// 加载场景信息
    /// </summary>
    /// <param name="sceneTree"></param>
    private void LoadState(SceneTree sceneTree)
    {
        //获取场景名称
        // var baseName = sceneTree.CurrentScene.SceneFilePath.GetFile().GetBaseName();
        var baseName = sceneTree.CurrentScene.Name;
        if( !SceneState.ContainsKey(baseName)) return;
        var nodesInGroup = sceneTree.GetNodesInGroup("enemies");
        foreach (var node in nodesInGroup)
        {   
            //获取敌人路径
            var nodePath = GetPathTo(node);
            //集合内不包含敌人信息 则敌人被杀死
            if (!SceneState[baseName].Enemies.Contains(nodePath))
            {
                //释放节点
                node.QueueFree();
            }
        }
    }
    
    /// <summary>
    /// 转场开始动画
    /// </summary>
    private async Task Start(SceneTree sceneTree, double duration = 0.5D)
    {
        //转场时玩家不能操作  简单处理暂停游戏
        sceneTree.Paused = true;
        //创建转成动画
        var tween = CreateTween();
        //设置tween暂停模式  无论 SceneTree 是否被暂停，该 Tween 都会处理
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        //设置 ColorRect color 透明度  color:a  color对象中的a属性
        tween.TweenProperty(ColorRect, "color:a", 1, duration);
        //等待信号
        await ToSignal(tween, "finished");
    }
    
    /// <summary>
    /// 转场结束动画
    /// </summary>
    /// <param name="sceneTree"></param>
    private void End(SceneTree sceneTree, double duration = 0.5D)
    {
        //完成转场
        var tween = CreateTween();
        //设置tween暂停模式  无论 SceneTree 是否被暂停，该 Tween 都会处理
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(ColorRect, "color:a", 0, duration);
        //继续游戏
        sceneTree.Paused = false;
    }

    /// <summary>
    /// 新游戏
    /// </summary>
    public  void NewGame()
    {
        SceneState.Clear();
        GetTree().ChangeSceneToFile("res://scenes/Worlds/Forest.tscn");
    }
    
    /// <summary>
    /// 游戏存档
    /// </summary>
    public void SaveGame()
    {
        //存档数据
        var archive = new Archive();
        //场景树
        var sceneTree = GetTree();
        var sceneTreeCurrentScene = sceneTree.CurrentScene;
        //玩家数据
        var playerData = new PlayerData();
        playerData.BaseHealth = PlayerHealthComponent.BaseHealth;
        playerData.CurrentHealth = PlayerHealthComponent.GetCurrentHealth();
        playerData.MaxEnergy = PlayerEnergyComponent.MaxEnergy;
        playerData.CurrentEnergy = PlayerEnergyComponent.GetEnergy();
        var player = sceneTreeCurrentScene.GetNode<Player>("Player");
        playerData.GlobalPosition = player.GlobalPosition;
        playerData.DefaultDirection = player.DefaultDirection;
        //场景路径
        playerData.CurrentScene = sceneTreeCurrentScene.SceneFilePath;
        
        
        //场景数据
        SaveState(sceneTree);
        
        //封装数据
        archive.PlayerData = playerData;
        archive.WorldState = SceneState[sceneTreeCurrentScene.Name];

        //打开存档文件
        // var file = FileAccess.Open(ArchivePath, FileAccess.ModeFlags.Write);
        // if (file == null)
        // {
        //     GD.Print("无法打开存档文件");
        //     return;
        // }
        // //写入数据
        // file.StoreString(JsonSerializer.Serialize(archive));
        // //关闭文件
        // file.Close();
        
        using var saveGame = FileAccess.Open(ArchivePath, FileAccess.ModeFlags.Write);
        
        saveGame.StoreString(JsonConvert.SerializeObject(archive));
    }

    /// <summary>
    /// 加载存档
    /// </summary>
    public void LoadGame()
    {
        // var file = FileAccess.Open(ArchivePath, FileAccess.ModeFlags.Read);
        //
        // if (file == null)
        // {
        //     GD.Print("无法打开存档文件");
        //     return;
        // }
        
        //读取存档数据
        // var archiveJson = file.GetAsText();
        //读取存档数据
        // var archiveJson = File.ReadAllText(ArchivePath);

        if (!HasSave())
        {
            return;
        }
        using var saveGame = FileAccess.Open(ArchivePath, FileAccess.ModeFlags.Read);

        var archiveJson = saveGame.GetAsText();
        
        if (!string.IsNullOrEmpty(archiveJson))
        {
            var archive = JsonConvert.DeserializeObject<Archive>(archiveJson);
            if (archive != null)
            {
                //设置玩家状态
                PlayerHealthComponent.BaseHealth = archive.PlayerData.BaseHealth;
                PlayerHealthComponent.CurrentHealth = archive.PlayerData.CurrentHealth;
                PlayerEnergyComponent.MaxEnergy = archive.PlayerData.MaxEnergy;
                PlayerEnergyComponent.CurrentEnergy = archive.PlayerData.CurrentEnergy;
                //转换场景
                ChangeScene(archive.PlayerData.CurrentScene, string.Empty, archive.PlayerData.GlobalPosition, archive.PlayerData.DefaultDirection);
                //设置场景信息
                SceneState[archive.WorldState.SceneName] = archive.WorldState;
            }
        }
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        GetTree().Quit();
    }
    
    /// <summary>
    /// 设置
    /// </summary>
    public void OpenSetting()
    {
        ChangeScene("res://Ui/SetScreen.tscn", null, null, null);
    }
    
    /// <summary>
    /// 是否有存档
    /// </summary>
    /// <returns></returns>
    public static bool HasSave()
    {
        return FileAccess.FileExists(ArchivePath);
    }
    
    /// <summary>
    /// 返回首页
    /// </summary>
    public void BackTitle()
    {
        ChangeScene("res://Ui/TitleScreen.tscn", null, null, null);
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    public static void SaveConfig()
    {
        //配置文件
        var configFile = new ConfigFile();
        //设置配置文件值
        configFile.SetValue("audio", SoundBus.Master.ToString(), SoundManager.GetVolume(SoundBus.Master));
        configFile.SetValue("audio", SoundBus.Sfx.ToString(), SoundManager.GetVolume(SoundBus.Sfx));
        configFile.SetValue("audio", SoundBus.Bgm.ToString(), SoundManager.GetVolume(SoundBus.Bgm));
        //写入配置文件
        configFile.Save(ConfigPath);
    }
    
    /// <summary>
    /// 加载配置文件
    /// </summary>
    public void LoadConfig()
    {
        //配置文件
        var configFile = new ConfigFile();
        //读取配置文件
        configFile.Load(ConfigPath);

        //设置相关属性
        SoundManager.SetVolume(SoundBus.Master, (float)configFile.GetValue("audio", SoundBus.Master.ToString(), 1.0f));
        SoundManager.SetVolume(SoundBus.Sfx, (float)configFile.GetValue("audio", SoundBus.Sfx.ToString(), 1.0f));
        SoundManager.SetVolume(SoundBus.Bgm, (float)configFile.GetValue("audio", SoundBus.Bgm.ToString(), 1.0f));
    }
}