using Adventure.script.Component;
using Godot;

namespace Adventure.script;
/// <summary>
/// 玩家状态栏
/// </summary>
public partial class StatusPanel : HBoxContainer
{
    //导出当前玩家血量组件
    [Export] public HealthComponent HealthComponent;
    [Export] public EnergyComponent EnergyComponent;
    //生命值
    public TextureProgressBar HealthBar;
    //带平滑效果的生命值
    public TextureProgressBar EasedHealthBar;
    //能量条
    public TextureProgressBar EnergyBar;
    //是否播放
    public bool IsPlay;

    public override void _Ready()
    {
        //获取全局节点
        //获取节点
        HealthBar = GetNode<TextureProgressBar>("VBoxContainer/HealthBar");
        EasedHealthBar = GetNode<TextureProgressBar>("VBoxContainer/HealthBar/EasedHealthBar");
        EnergyBar = GetNode<TextureProgressBar>("VBoxContainer/EnergyBar");
        //没有指定组件使用 全局组件
        if (HealthComponent == null)
        {
            HealthComponent = GameGlobal.Instance.PlayerHealthComponent;
        }

        if (EnergyComponent == null)
        {
            EnergyComponent = GameGlobal.Instance.PlayerEnergyComponent;
        }
        //修复死亡生命值不补满情况
        Recovery();
        //连接信号
        HealthComponent.HealthChange += OnHealthChange;
        //设置初始生命值
        OnHealthChange(HealthComponent.GetCurrentHealth());
        //能量变更
        EnergyComponent.EnergyChange += OnEnergyChange;
        OnEnergyChange(EnergyComponent.GetEnergy());
    }
    public override void _ExitTree()
    {
        //断开信号
        HealthComponent.HealthChange -= OnHealthChange;
        EnergyComponent.EnergyChange -= OnEnergyChange;
    }
    /// <summary>
    /// 恢复生命值
    /// </summary>
    public void Recovery()
    {
        if (HealthComponent.GetCurrentHealth() <= 0)
        {
            HealthComponent.FullHealth();
            EnergyComponent.FullEnergy();
        }
    }
    /// <summary>
    ///   能量变更
    /// </summary>
    /// <param name="energy"></param>
    private void OnEnergyChange(double energy)
    {
        
        var energyValue = energy / EnergyComponent.MaxEnergy;
        EnergyBar.Value = energyValue;
        // GD.Print("energy:", energy, "energyValue:", energyValue);
    }

    /// <summary>
    /// 生命值变更信息
    /// </summary>
    /// <param name="health"></param>
    private void OnHealthChange(double health)
    {
        // GD.Print("health = ", health);
        // GD.Print("GetCurrentHealth = ", HealthComponent.GetCurrentHealth());
        // GD.Print("BaseHealth = ", HealthComponent.BaseHealth);
        //计算生命值百分比
        var healthComponentBaseHealth = health / HealthComponent.BaseHealth;
        //设置生命值进度条值
        HealthBar.Value = healthComponentBaseHealth;
        if (IsPlay)
        {
            //设置平滑生命值效果动画  设置EasedHealthBar节点 value属性的值 为healthComponentBaseHealth  时间 0.5s
            CreateTween().TweenProperty(EasedHealthBar, "value", healthComponentBaseHealth, 0.5f);
        }
        else
        {
            EasedHealthBar.Value = healthComponentBaseHealth;
            IsPlay = true;
        }
        
    }
}