using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Resources.MapData;
using Godot;

namespace ClassicRoguelikeCourse.Managers.MapManager;
/// <summary>
/// 地图管理器
/// </summary>
public partial class MapManager : Node, IManager
{
    //初始化事件  Callable方法
    public event Action<Vector2I, Callable> Initialized;
    
    //导出地图数据
    [Export]
    private MapData _mapData;
    
    //地图数据
    public MapData MapData { get => _mapData; }
    
    //地图生成器
    private IMapGenerator _mapGenerator;
    
    //角色出生位置 玩家、敌人
    private List<Vector2I> _characterCellAtSpawn = new();
    
    public void Initialize()
    {
        //如果没有地图生成器 抛出异常
        if (GetChildCount() != 1 || GetChild(0) is not IMapGenerator)
        {
            throw new System.Exception("MapManager must have one child and it must be IMapGenerator");
        }
        //获取地图生成器
        _mapGenerator = GetChild(0) as IMapGenerator;
        //初始化
        _mapGenerator.Initialize();
        //触发初始化事件
        Initialized?.Invoke(_mapGenerator.GetPlayerSpawnCell(), Callable.From(_mapGenerator.GetEnemySpawnCell));
    }

    public void Update(double delta)
    {
    }

    /// <summary>
    /// 尝试添加角色出生位置
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool TryAddCharacterCellAtSpawn(Vector2I cell)
    {
        if (_characterCellAtSpawn.Contains(cell)) return false;
        _characterCellAtSpawn.Add(cell);
        return true;
    }
}