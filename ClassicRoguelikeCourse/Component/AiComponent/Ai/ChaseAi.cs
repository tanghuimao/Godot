using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.AStarGridManager;
using ClassicRoguelikeCourse.Managers.MapManager;
using Godot;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Component.AiComponent.Ai;
/// <summary>
/// 追击AI行为
/// </summary>
public partial class ChaseAi : Node, IAi
{
    // 执行事件
    public event Action<Vector2I> ExecutedEvent;
    
    // 地图管理器
    private MapManager _mapManager;

    // A*网格管理器
    private AStarGridManager _aStarGridManager;

    // 玩家
    private Player _player;

    // 敌人
    private Enemy _enemy;

    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _aStarGridManager = GetTree().CurrentScene.GetNode<AStarGridManager>("%AStarGridManager");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _enemy = GetParent().GetParent<Enemy>();
    }

    public bool Execute()
    {
        // 计算当前敌人和玩家的距离
        var distancePlayer =
            _enemy.GetDistanceTo(
                (Vector2I)(_player.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize
            );
        // 如果距离大于于视野距离 或者 距离在1格以内，则不执行
        if (distancePlayer > _enemy.CharacterData.Sight || distancePlayer == 1) return false;
        
        //计算敌人和玩家单元格位置
        var enemyCell = (Vector2I)(_enemy.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize;
        var playerCell = (Vector2I)(_player.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize;
        
        // 取消单元格Solid状态
        _aStarGridManager.AStarGrid2D.SetPointSolid(
            (Vector2I)(_enemy.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize, //当前敌人单元格位置
            false
        );
        //计算追击路径   GetIdPath 返回一个数组，其中包含形成 AStar2D 在给定点之间找到的路径的点的 ID。该数组从路径的起点到终点排序。
        var pathCells = _aStarGridManager.AStarGrid2D.GetIdPath(enemyCell, playerCell);
        // 如果路径长度小于2，追击路径不成立
        if (pathCells.Count < 2) return false;
        // 计算目标单元格  追击路径迈出的第一个单元格
        var targetCell = pathCells[1];
        // 计算目标方向 
        var direction = targetCell - enemyCell;
        // 执行事件
        ExecutedEvent?.Invoke(direction);
        return true;
    }
}