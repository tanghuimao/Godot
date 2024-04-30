using System;
using System.IO;
using System.Linq;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Managers.SaveLoadManager;
/// <summary>
/// 保存加载管理器
/// </summary>
public partial class SaveLoadManager : Node, IManager
{
    //地图管理器
    private MapManager.MapManager _mapManager;
    //玩家
    private Player _player;
    //保存路径
    private string _savePath = "user://classic_roguelike.sav";
    //加载数据
    private Dictionary<string, Variant> _loadedData;
    public Dictionary<string, Variant> LoadedData => _loadedData;

    /// <summary>
    /// 节点通知
    /// </summary>
    /// <param name="what"></param>
    public override void _Notification(int what)
    {
        // 关闭窗口
        if (what == NotificationWMCloseRequest)
        {
            TryDeleteSaveFile();
        }
    }

    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _loadedData = Load();
    }

    public void Update(double delta)
    {
    }

    public void TryDeleteSaveFile()
    {
        if (!FileAccess.FileExists(_savePath)) return;
        
        // 打开文件
        var savedFileForRead = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);

        var pathAbsolute = savedFileForRead.GetPathAbsolute();

        savedFileForRead.Dispose();
        GD.Print($"删除存档文件： {pathAbsolute}");
        //删除文件
        File.Delete(pathAbsolute);
    }
    
    /// <summary>
    /// 保存
    /// </summary>
    public void Save()
    {
        //  保存数据
        var dataForSave = new Dictionary<string, Variant>();
        // {
        //     {"Player", [Dictionary]},
        //     {"Maps", [Array<Dictionary>]}
        // };
        // 检查数据
        CheckDataBeforeSave(ref dataForSave);
        // 打开文件
        var savedFileForWrite = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
        // 获取保存数据
        var mapDataForSave = _mapManager.GetDataForSave();
        var playerDataForSave = _player.GetDataForSave();

        if (!dataForSave.ContainsKey("Maps"))
        {
            dataForSave.Add("Maps", new Array<Dictionary<string, Variant>>());
        }
        
        dataForSave["Maps"].AsGodotArray<Dictionary<string, Variant>>().Add(mapDataForSave);
        dataForSave["Player"] = playerDataForSave;
        // 写入数据
        savedFileForWrite.StoreVar(dataForSave, true);
        // 关闭文件
        savedFileForWrite.Dispose();
    }

    /// <summary>
    /// 检查数据  ref 引用传递
    /// </summary>
    /// <param name="dataForSave"></param>
    private void CheckDataBeforeSave(ref Dictionary<string, Variant> dataForSave)
    {
        // 检查文件
        if (FileAccess.FileExists(_savePath))
        {
            // 打开文件
            var savedFileForRead = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);

            //数据长度大于0  有存档
            if (savedFileForRead.GetLength() > 0)
            {
                //读取存档数据
                dataForSave = savedFileForRead.GetVar(true).AsGodotDictionary<string, Variant>();
                
                if (dataForSave.ContainsKey("Maps"))
                {
                    // 获取地图数据
                    var maps = dataForSave["Maps"].AsGodotArray<Dictionary<string, Variant>>();
                    for (int i = 0; i < maps.Count; i++)
                    {
                        //获取地图场景名称
                        var sceneName = maps[i]["SceneName"].AsString();
                        //名称 和 当前保存场景相同
                        if (sceneName == GetTree().CurrentScene.Name)
                        {
                            //移除 避免重复保存
                            dataForSave["Maps"].AsGodotArray().RemoveAt(i);
                        }
                    }
                }
            }
            // 关闭文件
            savedFileForRead.Dispose();
        }
    }

    public Dictionary<string, Variant> Load()
    {
        // 检查文件
        if (!FileAccess.FileExists(_savePath)) return null;
        // 打开文件
        var savedFileForRead = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
        // 数据长度等于0 没有存档信息
        if (savedFileForRead.GetLength() == 0) return null;
        // 读取存档数据
        var loadData = savedFileForRead.GetVar(true).AsGodotDictionary<string, Variant>();
        savedFileForRead.Dispose();
        return loadData;
    }
}