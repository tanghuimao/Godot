using Godot;
/// <summary>
/// 游戏相机
/// </summary>
public partial class Camera : Camera2D
{
	//相机初始位置
	private Vector2 _targetPosition = Vector2.Zero;

	public override void _Ready()
	{
		//设置为活动相机
		MakeCurrent();
	}

	public override void _Process(double delta)
	{
		//获取目标位置
		AcquireTarget();
		//设置相机位置 线性插入
		GlobalPosition = GlobalPosition.Lerp(_targetPosition, (float)(1 - Mathf.Exp(-delta * 30)));
	}
	
	//获取目标位置
	private void AcquireTarget()
	{
		//从节点树获取分组内第一个节点  判断类型
		if (GetTree().GetFirstNodeInGroup("player") is Node2D player)
		{
			_targetPosition = player.GlobalPosition;
		}
	}
}
