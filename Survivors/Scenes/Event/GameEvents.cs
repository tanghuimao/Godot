using System;
using Godot;
/// <summary>
/// 全局事件
/// </summary>
public partial class GameEvents : Node
{
	/// <summary>
	/// 经验收集事件
	/// </summary>
	public static event EventHandler<ExperienceVialCollectedEventArgs> ExperienceVialCollected;
	//事件参数
	public class ExperienceVialCollectedEventArgs : EventArgs
	{
		public short Number { get; }
	
		public ExperienceVialCollectedEventArgs(short number)
		{
			this.Number = number;
		}
	}
	
	//触发经验收集事件
	public static void OnExperienceVialCollected(object obj, short number)
	{
		var handler = ExperienceVialCollected;
		handler?.Invoke(obj, new ExperienceVialCollectedEventArgs(number));
	}
	
	/// <summary>
	/// 能力升级事件
	/// </summary>
	public static event EventHandler AbilityUpgradeAdded;
	//触发能力升级事件
	private static void OnAbilityUpgradeAdded()
	{
		var handler = AbilityUpgradeAdded;
		handler?.Invoke(null, EventArgs.Empty);
	}
}
