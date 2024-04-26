using Godot;

namespace Adventure.script.Component;
/// <summary>
/// 能量组件
/// </summary>
public partial class EnergyComponent : Node
{
    //能量变更信号
    [Signal]
    public delegate void EnergyChangeEventHandler(double energy);
    //导出基础能量
    [Export] public double MaxEnergy { get; set; } = 100D;
    //每秒恢复能量
    [Export] public double RecoverEnergy { get; set; } = 10D;
    
    //声明当前能量
    public double CurrentEnergy;
    
    public override void _Ready()
    {
        CurrentEnergy = MaxEnergy;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentEnergy < MaxEnergy)
        {
            //能量恢复
            CurrentEnergy += RecoverEnergy * delta;
            EmitSignal(SignalName.EnergyChange, CurrentEnergy);
        }
    }

    /// <summary>
    /// 获取当前能量
    /// </summary>
    /// <returns></returns>
    public double GetEnergy()
    {
        return CurrentEnergy;
    }
    
    /// <summary>
    /// 补满能量
    /// </summary>
    /// <returns></returns>
    public void FullEnergy()
    {
        CurrentEnergy = MaxEnergy;
    }
    /// <summary>
    /// 设置当前能量
    /// </summary>
    /// <param name="energy"></param>
    public  void SetEnergy(double energy)
    {
        CurrentEnergy -= energy;
        EmitSignal(SignalName.EnergyChange, CurrentEnergy);
    }
}