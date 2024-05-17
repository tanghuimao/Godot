using Godot;
using System;
/// <summary>
/// 生命值控制器
/// </summary>
public partial class HealthControl : HBoxContainer
{
    private PackedScene _heart;
    
    public override void _Ready()
    {
        _heart = GD.Load<PackedScene>("res://UI/HeartPanel.tscn");
    }
    /// <summary>
    /// 设置最大生命值
    /// </summary>
    /// <param name="maxHealth"></param>
    public void SetHealth(int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            AddChild(_heart.Instantiate());
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        var children = GetChildren();
        // 更新生命值
        for (int i = 0; i < currentHealth; i++)
        {
            if (children[i] is HeartPanel heartPanel)
            {
                heartPanel.Update(true);
            }
        }
        // 死亡
        for (int i = currentHealth; i < children.Count; i++)
        {
            if (children[i] is HeartPanel heartPanel)
            {
                heartPanel.Update(false);
            }
        }
    }
}
