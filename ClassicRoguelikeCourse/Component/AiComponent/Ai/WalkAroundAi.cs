using System;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Entities.Characters.Player;
using ClassicRoguelikeCourse.Managers.AStarGridManager;
using ClassicRoguelikeCourse.Managers.MapManager;
using Godot;

namespace ClassicRoguelikeCourse.Component.AiComponent.Ai;

/// <summary>
/// 随机移动Ai行为
/// </summary>
public partial class WalkAroundAi : Node, IAi
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
        // 如果距离小于视野距离，则不执行
        if (distancePlayer <= _enemy.CharacterData.Sight) return false;
        // 取消单元格Solid状态
        _aStarGridManager.AStarGrid2D.SetPointSolid(
            (Vector2I)(_enemy.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize, //当前敌人单元格位置
                false
            );
        // 随机移动（闲逛）
        var direction = new Vector2I(GD.RandRange(-1, 1), GD.RandRange(-1, 1));
        // 执行事件
        ExecutedEvent?.Invoke(direction);
        return true;
    }
}