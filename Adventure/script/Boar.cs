using Adventure.script.Component;
using Godot;

namespace Adventure.script;
/// <summary>
/// 野猪
/// </summary>
public partial class Boar : Enemy
{
    public float KnockBackAmount = 512f; //击退力度
    public bool IsHurt; //是否受伤
    public bool IsDie; //是否死亡
    public RayCast2D WallChecker; //墙检测
    public RayCast2D PlayerChecker; //玩家检测
    public RayCast2D FloorChecker; //地面检测
    public Timer CalmDownTimer; //冷静时间检测  在计时器结束后进行walk状态
    public HitBox HitBox;  //攻击盒  记录哪里来的攻击
    public HurtBox HurtBox;  //伤害盒
    public HealthComponent HealthComponent; //健康组件
    public override void _Ready()
    {
        base._Ready();
        WallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        PlayerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
        FloorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        CalmDownTimer = GetNode<Timer>("CalmDownTimer");
        HurtBox = GetNode<HurtBox>("Graphics/HurtBox");
        HealthComponent = GetNode<HealthComponent>("HealthComponent");
        //初始化设置状态机野猪对象属性
        foreach (State.Boar.BoarState state in StateMachine.GetChildren()) {
            state.Boar = this;
        }

        //连接信号
        //受伤
        HurtBox.Hurt += OnHurt;
        //生命值变更
        HealthComponent.HealthChange += OnHealthChange;
        //死亡
        HealthComponent.Died += OnDied;
    }
    /// <summary>
    /// 攻击信号处理函数
    /// </summary>
    /// <param name="hurtbox"></param>
    private void OnHit(HurtBox hurtbox)
    {
        //受伤震动屏幕
        GameGlobal.OnCameraShark(2);
    }

    /// <summary>
    /// 受伤信号处理函数
    /// </summary>
    /// <param name="hitbox"></param>
    private void OnHurt(HitBox hitbox)
    {
        //扣减生命值
        HealthComponent.Damage(1D);
        HitBox = hitbox;
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
}