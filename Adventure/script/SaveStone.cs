using Godot;

namespace Adventure.script;
/// <summary>
/// 存档点
/// </summary>
public partial class SaveStone : Interactable
{

    public AnimationPlayer AnimationPlayer;
    
    public override void _Ready()
    {
        base._Ready();
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        AnimationPlayer.Play("read");
    }
    
    public override void Interact()
    {
        base.Interact();
        //播放动画
        AnimationPlayer.Play("active");
        //存档
        GameGlobal.GetInstance().SaveGame();
        
    }
}