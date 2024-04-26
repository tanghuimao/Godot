using System;
using Godot;
/// <summary>
/// 生命值组件
/// </summary>
[GodotClassName("HealthComponent")]
public partial class HealthComponent : Node
{
	//自定义信号
	//死亡事件
	public event EventHandler Died;
	protected virtual void OnDied(object obj)
	{
		var handler = Died;
		handler?.Invoke(obj, EventArgs.Empty);
	}
	//生命值改变事件
	public event EventHandler HealthChanged;
	protected virtual void OnHealthChanged(object obj)
	{
		var handler = HealthChanged;
		handler?.Invoke(obj, EventArgs.Empty);
	}
	
	//导出基础生命值
	[Export] public double BaseHealth { get; set; } = 10f;
	
	//声明当前生命值
	private double _currentHealth;

	public override void _Ready()
	{
		_currentHealth = BaseHealth;
	}
	
	//获取当前生命值
	private double GetHealthPercent()
	{
		return _currentHealth <= 0 ? 0f : Mathf.Min(_currentHealth / BaseHealth, 1f);
	}
	
	//检测当前生命值
	private void CheckHealth()
	{
		//当生命值归0
		if (_currentHealth == 0)
		{
			//触发信号
			OnDied(this);
			//清除数据
			Owner.QueueFree();
		}
	}
	
	//扣减生命
	private void Damage(double damageAmount)
	{
		//计算当前生命值
		_currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);
		//触发生命值信号
		OnHealthChanged(this);
		//延迟调用
		Callable.From(CheckHealth).CallDeferred();
	}
}
