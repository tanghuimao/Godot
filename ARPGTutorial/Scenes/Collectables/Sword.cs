using Godot;
using System;

public partial class Sword : Collectable
{

    private AnimationPlayer _animationPlayer;
    private CollisionShape2D _collisionShape;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override async void Collect(Inventory inventory)
    {
        _animationPlayer.Play("spin");
        await ToSignal(_animationPlayer, "animation_finished");
        base.Collect(inventory);
    }

    public void Enable()
    {
        _collisionShape.Disabled = false;
    }
    
    public void Disable()
    {
        _collisionShape.Disabled = true;
    }
}
