using Adventure.script.State;
using Godot;

namespace Adventure.script;
//方向枚举
public enum Direction
{
    Left = -1,
    Right = 1
}
/// <summary>
/// 敌人抽象父类
/// </summary>
public partial class Enemy : CharacterBody2D
{
    //导出默认方向
    [Export] public Direction DefaultDirection = Direction.Left;
    //导出初始速度
    [Export] public float MaxSpeed = 180;
    //导出加速度
    [Export] public float Acceleration = 2000;
    //重力加速度
    public float DefaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

    //引入节点
    public Node2D Graphics;
    public AnimationPlayer AnimationPlayer;
    public StateMachine StateMachine; //状态机
    public override void _Ready()
    {
        //加入组
        AddToGroup("enemies");
        //获取节点
        Graphics = GetNode<Node2D>("Graphics");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        StateMachine = GetNode<StateMachine>("StateMachine");
    }
    
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="delta">增量</param>
    /// <param name="speed">加速度</param>
    public void Move(float delta, float speed)
    {
        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, speed * (float)DefaultDirection, Acceleration * delta);
        velocity.Y += DefaultGravity * delta;
        Velocity = velocity;
        
        MoveAndSlide();
    }

    /// <summary>
    /// 设置方向
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(Direction direction)
    {
        DefaultDirection = direction;
        //调整方向  取当前方向反方向
        Graphics.Scale = new Vector2(-(float)DefaultDirection, 1);
    }
}