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
