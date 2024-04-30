using System;
using ClassicRoguelikeCourse.Entities.Characters.Player;
using Godot;

namespace ClassicRoguelikeCourse.UI.DefeatWindow;

/// <summary>
/// 失败窗口
/// </summary>
public partial class DefeatWindow : CanvasLayer, IUi
{
    public void Initialize()
    {
        GetTree().CurrentScene.GetNode<Player>("%Player").DiedForSure += () => Visible = true;
    }

    public void Update(double delta)
    {
    }
}