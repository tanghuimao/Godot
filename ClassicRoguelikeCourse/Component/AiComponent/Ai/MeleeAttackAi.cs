using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.CombatManager;
using ClassicRoguelikeCourse.Managers.MapManager;
using Godot;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Component.AiComponent.Ai;

/// <summary>
/// 近战攻击AI
/// </summary>
public partial class MeleeAttackAi : Node, IAi
{
    // 地图管理器
    private MapManager _mapManager;
    
    // 玩家
    private Player _player;

    // 敌人
    private Enemy _enemy;

    // 战斗管理器
    private CombatManager _combatManager;
    
    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _enemy = GetParent().GetParent<Enemy>();
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager>("%CombatManager");
    }

    public bool Execute()
    {
        // 计算当前敌人和玩家的距离
        var distancePlayer =
            _enemy.GetDistanceTo(
                (Vector2I)(_player.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize
            );
        // 如果距离大于1，则进行不攻击
        if (distancePlayer > 1) return false;
        
        //攻击将纳入战斗结算
        _combatManager.AddCombatant(_enemy, _player);
        GD.Print($"{_enemy.CharacterData.Name}近战攻击了{_player.CharacterData.Name}");
        return true;
    }
}