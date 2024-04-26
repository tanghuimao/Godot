using Godot;
/// <summary>
/// 白老鼠敌人
/// </summary>
public partial class GrayMouseEnemy : CharacterBody2D
{
    //引入节点
    private Node2D _visual;
    private VelocityComponent _velocityComponent;
    //初始化
    public override void _Ready()
    {
        //引入节点
        _visual = GetNode<Node2D>("Visuals");
        _velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
    }
    //每帧处理
    public override void _Process(double delta)
    {
        //调用移动组件方法
        _velocityComponent.AccelerateToPlayer();
        //移动
        _velocityComponent.Move(this);
        //控制敌人移动方向
        var sign = Mathf.Sign(Velocity.X);
        if (sign != 0)
        {
            _visual.Scale = new Vector2(sign, 1);
        }
    }
}
