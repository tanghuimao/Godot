using Godot;

/// <summary>
/// 移动组件
/// </summary>
[GodotClassName("VelocityComponent")]
public partial class VelocityComponent : Node
{
    //导出基础速度
    [Export] public int BaseSpeed { get; set; }= 40;
    //导出加速度
    [Export] public float Accelerate { get; set; } = 5f;
    
    //声明初始速度
    private Vector2 _velocity;
    
    //加速移动向玩家
    public void AccelerateToPlayer()
    {
        //当前节点拥有者
        var ownerNode2d = Owner as Node2D;
        if (ownerNode2d == null)
        {
            return;
        }
        //获取玩家
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
        {
            return;
        }
        //获取玩家方向
        var direction = (player.GlobalPosition - ownerNode2d.GlobalPosition).Normalized();
        
        //移动到指定方向
        AccelerateToDirection(direction);
    }

    //加速移动到指定方向
    private void AccelerateToDirection(Vector2 direction)
    {
        //计算预期速度
        var desiredVelocity = direction * BaseSpeed;
        //计算移动速度
        _velocity = _velocity.Lerp(desiredVelocity, (float)(1 - Mathf.Exp(-Accelerate * GetProcessDeltaTime())));
    }
    
    //减速
    private void Decelerate()
    {
        AccelerateToDirection(Vector2.Zero);
    }
    
    //移动
    public void Move(CharacterBody2D characterBody2D)
    {
        //设置组件速度
        characterBody2D.Velocity = _velocity;
        //移动
        characterBody2D.MoveAndSlide();
        //重置速度等于当前节点速度
        _velocity = characterBody2D.Velocity;
    }
}
