using Godot;
using System;
/// <summary>
/// 敌人
/// </summary>
public partial class Enemy : Character
{
    public override void HandleHit()
    {
        CharacterData.Health -= 20;
        if (CharacterData.Health <= 0)
        {
            QueueFree();
        }
        GD.Print($"敌人被击中, health:{CharacterData.Health}");
    }
}
