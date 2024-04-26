using Godot;

namespace Adventure.script;
/// <summary>
/// 攻击框
/// </summary>
[GlobalClass]
[GodotClassName("HitBox")]
public partial class HitBox : Area2D
{
    //自定义信号
    [Signal]
    public delegate void HitEventHandler(HurtBox hurtBox);

    public override void _Ready()
    {
        //区域进入信号  委托方法处理
        AreaEntered += OnAreaEntered;
    }
    //处理方法
    private void OnAreaEntered(Area2D area)
    {
        var hurtBox = (HurtBox)area;
        // GD.Print($"[hit] {Owner.Name} => {hurtBox.Owner.Name}");

        //触发信号
        EmitSignal(SignalName.Hit, hurtBox);
        hurtBox.EmitSignal(HurtBox.SignalName.Hurt, this);
    }
}