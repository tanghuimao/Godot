using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// 玩家
/// </summary>
public partial class Player : CharacterBody2D
{
    //生命值变化事件
    public event Action<int> HealthChangedEvent;

    //速度
    [Export] public int Speed = 100;

    //击退速度
    [Export] public int KnockBackSpeed = 500;

    //最大生命值
    [Export] public int MaxHealth = 10;
    
    //玩家背包
    [Export] public Inventory Inventory;

    //当前生命值
    private int _currentHealth;

    //动画播放器
    private AnimationPlayer _animationPlayer;
    private AnimationPlayer _effectsAnimationPlayer;
    private Area2D _hurtArea2D;
    private Timer _hurtTimer;
    private Node2D _weapon;

    private Vector2 _currentDirection = Vector2.Down;
    private bool _isAttack;
    private bool _canHurt = true;
    


    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _effectsAnimationPlayer = GetNode<AnimationPlayer>("EffectAnimationPlayer");
        _hurtArea2D = GetNode<Area2D>("HurtArea2D");
        _hurtArea2D.BodyEntered += OnHurtBodyEntered;
        _hurtArea2D.BodyExited += OnHurtBodyExited;
        _hurtTimer = GetNode<Timer>("HurtTimer");
        _hurtTimer.Timeout += OnPlayerHurtTimerTimeout;
        _weapon = GetNode<Node2D>("Weapon");
        WeaponDisable();
    }


    public override void _PhysicsProcess(double delta)
    {
        Attack();
        if (_isAttack) return;
        Move();
        //伤害
        if (_canHurt)
        {
            //获取区域内所有节点
            foreach (var body in _hurtArea2D.GetOverlappingBodies())
            {
                if (body is Slime slime)
                {
                    Hurt(slime);
                }
            }
        }
    }
    /// <summary>
    /// 攻击
    /// </summary>
    private async void Attack()
    {
        if (Input.IsActionPressed("attack"))
        {
            WeaponEnable();
            _isAttack = true;
            var animationName = AnimationName(_currentDirection, "attack_");
            _animationPlayer.Play(animationName);
            await ToSignal(_animationPlayer, "animation_finished");
            WeaponDisable();
            _isAttack = false;
        }

    }

    private void WeaponEnable()
    {
        _weapon.Visible = true;
        _weapon.GetNode<Sword>("Sword").Enable();
    }
    
    private void WeaponDisable()
    {
        _weapon.Visible = false;
        _weapon.GetNode<Sword>("Sword").Disable();
    }
    
    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        //输入方向
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        //设置当前方向
        if (direction != Vector2.Zero)
        {
            _currentDirection = direction;
        }
        //更新动画
        UpdateAnimation(direction);
        //设置速度
        Velocity = direction * Speed;
        //平滑移动
        MoveAndSlide();
    }

    /// <summary>
    /// 更新动画
    /// </summary>
    /// <param name="direction"></param>
    private void UpdateAnimation(Vector2 direction)
    {
        if (direction == Vector2.Zero)
        {
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Stop();
            }
        }
        else
        {
            var animationName = AnimationName(direction, "walk_");
            _animationPlayer.Play(animationName);
        }
    }
    /// <summary>
    /// 动画名称
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="pre"></param>
    /// <returns></returns>
    private static string AnimationName(Vector2 direction, string pre)
    {
        var animationName = pre;

        if (direction.Y < 0)
        {
            animationName += "up";
        }
        else if (direction.Y > 0)
        {
            animationName += "down";
        }
        else if (direction.X < 0)
        {
            animationName += "left";
        }
        else if (direction.X > 0)
        {
            animationName += "right";
        }

        return animationName;
    }

    private void Hurt(Slime slime)
    {
        //减少生命值
        _currentHealth -= 1;
        //变更状态
        _canHurt = false;
        _hurtTimer.Start();
        //触发事件
        HealthChangedEvent?.Invoke(_currentHealth);
        //击退
        KnockBack(slime.Velocity);
        //播放特效
        _effectsAnimationPlayer.Play("hurt");
        //死亡
        if (_currentHealth <= 0)
        {
            QueueFree();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    private void OnHurtBodyEntered(Node2D body)
    {
        if (body is Collectable collectable)
        {
            collectable.Collect(Inventory);
        }
    }

    /// <summary>
    /// 命中框离开
    /// </summary>
    /// <param name="body"></param>
    private void OnHurtBodyExited(Node2D body)
    {
    }

    /// <summary>
    /// 击退
    /// </summary>
    /// <param name="velocity"></param>
    private void KnockBack(Vector2 velocity)
    {
        //击退方向
        var direction = (velocity - Velocity).Normalized();
        //设置速度
        Velocity = direction * KnockBackSpeed;
        MoveAndSlide();
    }

    /// <summary>
    /// 玩家受伤计时器超时
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnPlayerHurtTimerTimeout()
    {
        _canHurt = true;
    }
}