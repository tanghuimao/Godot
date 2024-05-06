using Godot;
using System;
/// <summary>
/// 敌人
/// </summary>
public partial class Enemy : Character
{
    //执行Ai逻辑
    private AI _ai;
    private Weapon _weapon;
    
    public override void _Ready()
    {
        _weapon = GetNode<Weapon>("Weapon");
        _ai = GetNode<AI>("AI");
        //初始化
        _ai.Initialize(this, _weapon);
        Hit += OnEnemyHit;
        Died += OnEnemyDied;
    }

    public override void _ExitTree()
    {
        Hit -= OnEnemyHit;
        Died -= OnEnemyDied;
    }

    /// <summary>
    /// 旋转到指定位置
    /// </summary>
    /// <param name="targetLocation"></param>
    public float RotateToTarget(Vector2 targetLocation)
    {
        var angle = GlobalPosition.DirectionTo(targetLocation).Angle();
        Rotation = Mathf.Lerp(Rotation, angle, 0.1f);
        return angle;
    }
    
    /// <summary>
    /// 移动到指定位置
    /// </summary>
    /// <param name="targetLocation"></param>
    public void MoveToTarget(Vector2 targetLocation)
    {
        // 计算速度
        Velocity = GlobalPosition.DirectionTo(targetLocation).Normalized() * CharacterData.Speed;
        // 移动
        MoveAndSlide();
    }

    /// <summary>
    /// 击中
    /// </summary>
    /// <param name="bullet"></param>
    private void OnEnemyHit(Bullet bullet)
    {
        CharacterData.SetHealth(bullet.BulletData.Damage);
        if (CharacterData.Health <= 0)
        {
            OnDied();
        }
        GD.Print($"敌人被击中, health:{CharacterData.Health}");
    }
    /// <summary>
    /// 死亡
    /// </summary>
    private void OnEnemyDied()
    {
        QueueFree();
    }
}
