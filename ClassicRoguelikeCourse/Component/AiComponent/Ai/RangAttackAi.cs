using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.CombatManager;
using ClassicRoguelikeCourse.Managers.MapManager;
using Godot;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Component.AiComponent.Ai;
/// <summary>
/// 远程攻击AI
/// </summary>
public partial class RangAttackAi : Node, IAi
{
    // 地图管理器
    private MapManager _mapManager;
    
    // 玩家
    private Player _player;

    // 敌人
    private Enemy _enemy;
    // 线段
    private Line2D _line2D;
    // 战斗管理器
    private CombatManager _combatManager;
    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _enemy = GetParent().GetParent<Enemy>();
        _line2D = GetNode<Line2D>("Line2D");
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager>("%CombatManager");
    }

    public bool Execute()
    {
        // 计算当前敌人和玩家的距离
        var distancePlayer =
            _enemy.GetDistanceTo(
                (Vector2I)(_player.GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize
            );
        // 如果距离玩家大于视野，则不执行
        if (distancePlayer > _enemy.CharacterData.Sight) return false;
        
        //攻击将纳入战斗结算
        _combatManager.AddCombatant(_enemy, _player);
        GD.Print($"{_enemy.CharacterData.Name}远程攻击了{_player.CharacterData.Name}");
        // 显示远程攻击线
        ShowRangeAttackLine(_enemy.GlobalPosition, _player.GlobalPosition);
        return true;
    }

    /// <summary>
    /// 显示远程攻击线
    /// </summary>
    /// <param name="start">开始位置</param>
    /// <param name="target">目标位置</param>
    private async void ShowRangeAttackLine(Vector2 start, Vector2 target)
    {
        // 显示线段
        _line2D.SetPointPosition(0, start);
        _line2D.SetPointPosition(1, target);
        
        //等待一帧
        await  ToSignal(GetTree(), "process_frame");
        
        // 不显示线段
        _line2D.SetPointPosition(0, Vector2.Zero);
        _line2D.SetPointPosition(1, Vector2.Zero);
    }
}