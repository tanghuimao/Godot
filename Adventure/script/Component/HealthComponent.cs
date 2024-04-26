using Godot;

namespace Adventure.script.Component;
/// <summary>
/// 健康组件
/// </summary>
public partial class HealthComponent : Node
{
	
	//自定义信号
	[Signal]
	public delegate void HealthChangeEventHandler(double health);
	[Signal]
	public delegate void DiedEventHandler();
	
	//导出生命值
	[Export] public double BaseHealth { get; set; } = 10D;
	
	//声明当前生命值
	public double CurrentHealth;

	public override void _Ready()
	{
		CurrentHealth = BaseHealth;
	}
	
	/// <summary>
	/// 获取当前生命值
	/// </summary>
	/// <returns></returns>
	public double GetCurrentHealth()
	{
		return CurrentHealth;
	}
	
	/// <summary>
	/// 补满生命值
	/// </summary>
	/// <returns></returns>
	public void FullHealth()
	{
		CurrentHealth = BaseHealth;
	}
	
	/// <summary>
	/// 检查生命值
	/// </summary>
	private void CheckHealth()
	{
		if (CurrentHealth <= 0)
		{
			//触发死亡信号
			EmitSignal(SignalName.Died);
			//释放节点
			// Owner.QueueFree();
		}
	}
	
	/// <summary>
	/// 伤害
	/// </summary>
	/// <param name="damage"></param>
	public void Damage(double damage)
	{
		//计算当前生命值
		CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
		//触发生命值变更信号
		EmitSignal(SignalName.HealthChange, CurrentHealth);
		//延迟调用
		Callable.From(CheckHealth).CallDeferred();
	}
}