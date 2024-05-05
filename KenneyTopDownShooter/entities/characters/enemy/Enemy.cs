using Godot;
using System;
/// <summary>
/// 敌人
/// </summary>
public partial class Enemy : Character
{
    public override void _Ready()
    {
        Hit += OnEnemyHit;
        Died += OnEnemyDied;
    }
    
    private void OnEnemyHit()
    {
        CharacterData.SetHealth(20);
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
