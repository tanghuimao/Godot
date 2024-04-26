using Godot;
/// <summary>
/// 玩家
/// </summary>
public partial class Player : CharacterBody2D
{
    //引入节点
    private AnimationPlayer _animationPlayer;
    private Node2D _visual;
    
    //最大速度
    private const short MaxSpeed = 120;

    //加速度
    private const short AccelerationSmoothing = 15;

    //初始化方法
    public override void _Ready()
    {
        //获取节点
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _visual = GetNode<Node2D>("Visuals");
    }

    //每一帧处理
    public override void _Process(double delta)
    {
        //移动向量
        var movementVector = GetMovementVector();
        //方向
        var direction = movementVector.Normalized();
        //计算目标速度
        var targetVelocity = direction * MaxSpeed;
        //计算实际速度
        Velocity = Velocity.Lerp(targetVelocity, (float)(1 - Mathf.Exp(- delta * AccelerationSmoothing)));
        //移动
        MoveAndSlide();
        //播放动画
        if (movementVector.X != 0 || movementVector.Y != 0)
        {
            _animationPlayer.Play("walk");
        }
        else
        {
            _animationPlayer.Play("RESET");
        }
        //控制人物反转
        var moveSign = Mathf.Sign(movementVector.X);
        if (moveSign != 0)
        {
            _visual.Scale = new Vector2(moveSign, 1);
        }

    }
    
    //获取玩家移动
    private static Vector2 GetMovementVector()
    {
        var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        return new Vector2(xMovement, yMovement);
    }
}