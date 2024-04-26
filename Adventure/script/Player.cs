using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adventure.script.Component;
using Adventure.script.State;
using Godot;

namespace Adventure.script;

/// <summary>
/// 玩家
/// </summary>
public partial class Player : CharacterBody2D
{
    //是否可以连击
    [Export] public bool CanCombo;
    //玩家默认方向
    [Export] public Direction DefaultDirection = Direction.Right;

    //重力加速度
    public float DefaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
    //基础移动速度
    public float BaseSpeed = 160.0f;
    //跳跃高度
    public float JumpVelocity = -360.0f;
    //蹬墙跳
    public Vector2 WallJumpVelocity = new Vector2(400, -360);
    //空气加速度
    public float AirAcceleration = 800.0f; // RunSpeed / 0.2f
    //地面加速度
    public float FloorAcceleration = 1600.0f; // RunSpeed / 0.1f
    //击退力度
    public float KnockBackAmount = 512f; 
    //滑铲时间
    public float SlidingTime = 0.3f;
    //滑铲速度
    public float SlidingSpeed = 256f;
    //滑铲消耗能量
    public float SlidingEnergy = 40f;
    //玩家着陆高度
    public float LandingHeight = 100f;
    //玩家下落时高度
    public float FallingHeight;
    //引入节点
    public Node2D Graphics;
    public AnimationPlayer AnimationPlayer;
    public Timer CoyoteTimer; //悬空一段时间 可以操作
    public Timer JumpRequestTimer; //跳跃计算 实现二级跳、三级跳
    public Timer ComboTimer; //攻击时间  实现连击
    public Timer InvincibleTimer; //无敌时间  受到攻击无敌
    public Timer SlideRequestTimer; //滑铲时间
    public StateMachine StateMachine; //状态机
    public RayCast2D HandChecker; //手碰撞检查 控制滑墙进入时机
    public RayCast2D FootChecker; //脚碰撞检查
    public HitBox HitBox;  //攻击盒 
    public HitBox HitBoxed;  //被攻击盒  记录哪里来的攻击
    public HurtBox HurtBox;  //伤害盒
    public HealthComponent HealthComponent; //健康组件
    public EnergyComponent EnergyComponent; //能量组件
    public AnimatedSprite2D InteractionIcon; //互动图标
    public GameOverScreen GameOverScreen; //死亡界面
    public PauseScreen PauseScreen; //暂停界面
    public Camera2D Camera2D; //相机
    //是否第一帧
    public bool IsFirstTick = false;
    //是否移动 默认 false 
    public bool IsMove = false;
    //是否跳跃
    public bool IsJump = false;
    //是否 是滑墙跳
    public bool IsWallJump = false;
    //是否受伤
    public bool IsHurt; 
    //是否死亡
    public bool IsDie;
    //互动对象
    public readonly List<Interactable> Interactables = new();
    
    /// <summary>
    /// 初始化
    /// </summary>
    /// </summary>
    public override void _Ready()
    {
        //获取全局节点
        //获取节点
        Graphics = GetNode<Node2D>("Graphics");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        CoyoteTimer = GetNode<Timer>("CoyoteTimer");
        JumpRequestTimer = GetNode<Timer>("JumpRequestTimer");
        ComboTimer = GetNode<Timer>("ComboTimer");
        InvincibleTimer = GetNode<Timer>("InvincibleTimer");
        SlideRequestTimer = GetNode<Timer>("SlideRequestTimer");
        StateMachine = GetNode<StateMachine>("StateMachine");
        HandChecker = GetNode<RayCast2D>("Graphics/HandChecker");
        FootChecker = GetNode<RayCast2D>("Graphics/FootChecker");
        HitBox = GetNode<HitBox>("Graphics/HitBox");
        HurtBox = GetNode<HurtBox>("Graphics/HurtBox");
        // HealthComponent = GetNode<HealthComponent>("HealthComponent");
        HealthComponent = GameGlobal.Instance.PlayerHealthComponent;
        // EnergyComponent = GetNode<EnergyComponent>("EnergyComponent");
        EnergyComponent = GameGlobal.Instance.PlayerEnergyComponent;
        InteractionIcon = GetNode<AnimatedSprite2D>("InteractionIcon");
        GameOverScreen = GetNode<GameOverScreen>("CanvasLayer/GameOverScreen");
        PauseScreen = GetNode<PauseScreen>("CanvasLayer/PauseScreen");
        Camera2D = GetNode<Camera2D>("%Camera2D");
        //初始化设置状态机玩家对象属性
        foreach (State.Player.PlayerState state in StateMachine.GetChildren()) {
            state.Player = this;
        }
        //连接信号
        //攻击
        HitBox.Hit += OnHit;
        //受伤
        HurtBox.Hurt += OnHurt;
        //生命值变更
        HealthComponent.HealthChange += OnHealthChange;
        //死亡
        HealthComponent.Died += OnDied;
        //能量变更
        EnergyComponent.EnergyChange += OnEnergyChange;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //玩家按下交换键
        if (@event.IsActionPressed("interact") && Interactables.Count > 0)
        {
            //取出第一个
            Interactables.First().Interact();
        }
        //玩家按下暂停键
        if (@event.IsActionPressed("pause"))
        {
            //显示暂停界面
            PauseScreen.ShowPause();
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="delta">增量</param>
    /// <param name="gravity">重力加速度</param>
    /// <param name="acceleration">加速度</param>
    public void Move(float delta, float gravity, float acceleration)
    {
        
        //移动方向
        var direction = Input.GetAxis("move_left", "move_right");
        //设置移动速度 Mathf.MoveToward加速减速效果  
        Velocity = new Vector2(
            (float)Mathf.MoveToward(Velocity.X, direction * BaseSpeed, acceleration * delta),
            Velocity.Y + gravity * delta
        );
        //方向不为0 设置反转 设置移动状态
        if (!Mathf.IsZeroApprox(direction))
        {
            // Graphics.Scale = new Vector2(direction < 0 ? -1 : 1, 1) ;
            SetDirection(direction < 0 ? Direction.Left : Direction.Right);
            IsMove = true;
        }
        else
        {
            IsMove = false;
        }
        //移动
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
        Graphics.Scale = new Vector2((float)DefaultDirection, 1);
    }
    
    /// <summary>
    /// 站立
    /// </summary>
    /// <param name="delta">增量</param>
    /// <param name="gravity">重力加速度</param>
    /// <param name="acceleration">加速度</param>
    public void Stand(float delta, float gravity, float acceleration)
    {
        //移动方向
        //var direction = Input.GetAxis("move_left", "move_right");
        //设置移动速度 Mathf.MoveToward加速减速效果  
        Velocity = new Vector2(
            Mathf.MoveToward(Velocity.X, 0.0f, acceleration * delta),
            Velocity.Y + gravity * delta
        );
        //移动
        MoveAndSlide();
    }
    
    /// <summary>
    /// 滑铲
    /// </summary>
    /// <param name="delta">增量</param>
    public void Sliding(float delta)
    {
        //设置玩家速度   方向 Graphics.Scale.X  滑铲速度 SlidingSpeed  x移动值
        Velocity = new Vector2(
            Graphics.Scale.X * SlidingSpeed,
            Velocity.Y + DefaultGravity * delta
        );
        //移动
        MoveAndSlide();
    }

    /// <summary>
    /// 是否进入滑墙状态
    /// </summary>
    /// <returns></returns>
    public bool CanWallSliding()
    {
        //玩家贴墙 并且 手脚检测点都贴墙
        return IsOnWall() && HandChecker.IsColliding() && FootChecker.IsColliding();
    }
    
    /// <summary>
    /// 是否进入滑铲状态
    /// </summary>
    /// <returns></returns>
    public bool CanSliding()
    {
        //能量大于滑铲能量
        if (EnergyComponent.GetEnergy() < SlidingEnergy) return false;
        //玩家不贴墙 并且 脚检测点不贴墙
        return !IsOnWall() && !FootChecker.IsColliding() && SlideRequestTimer.TimeLeft > 0;
    }
    
    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="hurtbox"></param>
    private async void OnHit(HurtBox hurtbox)
    {
        GameGlobal.OnCameraShark(2);
        
        //顿帧
        //设置游戏时间
        Engine.TimeScale = 0.01f;
        //延迟
        await Task.Delay(100);
        //恢复
        Engine.TimeScale = 1;
    }
    
    
    /// <summary>
    /// 受伤信号处理函数
    /// </summary>
    /// <param name="hitbox"></param>
    private void OnHurt(HitBox hitbox)
    {
        //处于无敌时间直接返回
        if (InvincibleTimer.TimeLeft > 0) return;   
        
        //扣减生命值
        HealthComponent.Damage(1D);
        //设置受伤来源攻击
        HitBoxed = hitbox;
        //受伤震动屏幕
        GameGlobal.OnCameraShark(4);
    }
    /// <summary>
    /// 生命值变更
    /// </summary>
    /// <param name="health"></param>
    private void OnHealthChange(double health)
    {
        IsHurt = true;
    }
    /// <summary>
    /// 死亡
    /// </summary>
    private void OnDied()
    {
        IsDie = true;
    }

    /// <summary>
    /// 死亡方法  仅仅做学习用 动画方法调用
    /// </summary>
    public void Died()
    {
        GameOverScreen.ShowGameOver();
    }
    /// <summary>
    /// 能量变更
    /// </summary>
    /// <param name="energy"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnEnergyChange(double energy)
    {
        // GD.Print("energy:", energy);
    }
}