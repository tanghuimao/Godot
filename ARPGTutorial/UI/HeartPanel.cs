using Godot;
using System;
/// <summary>
/// 红心
/// </summary>
public partial class HeartPanel : Panel
{

    private Sprite2D _sprite2D;
    
    public override void _Ready()
    {
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
    }
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="whole">是否满血</param>
    public void Update(bool whole)
    {
        if (whole)
        {
            _sprite2D.Frame = 4;
        }
        else
        {
            _sprite2D.Frame = 0;
        }
    }
}
