using ClassicRoguelikeCourse.Managers.Fsm;
using Godot;

namespace ClassicRoguelikeCourse.Scenes;
/// <summary>
/// 主场景 不直接使用由其他场景继承
/// </summary>
public partial class Main : Node
{
    //有限状态机
    private Fsm _fsm;
    
    public override void _Ready()
    {
        base._Ready();
        _fsm = GetNode<Fsm>("%Fsm");
        _fsm.Initialize();
    }
    
    public override void _Process(double delta)
    {
        _fsm.Update(delta);
    }
}