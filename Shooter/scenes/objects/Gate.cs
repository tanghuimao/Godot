using Godot;
using System;

public partial class Gate : StaticBody2D
{	
	//事件
	public event Action<Node2D> InGateEvent;
	//区域
	private Area2D _area2D;
	
	public override void _Ready()
	{
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.BodyEntered += OnGateBodyEntered;
		
	}

	public override void _ExitTree()
	{
		_area2D.BodyEntered -= OnGateBodyEntered;
	}

	private void OnGateBodyEntered(Node2D body)
	{
		InGateEvent?.Invoke(body);
	}
}
