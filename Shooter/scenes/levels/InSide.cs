using Godot;
using System;

public partial class InSide : Level
{
    [Export]
    public string ChangeScene;
    private Area2D _area2D;
    public override void _Ready()
    {
        base._Ready();
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        _area2D.BodyEntered -= OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        var tween = CreateTween();
        tween.TweenProperty(Player, "_speed", 0, 0.5);
        // GetTree().ChangeSceneToFile(ChangeScene);
        // Callable.From(() => GetTree().ChangeSceneToFile(ChangeScene)).CallDeferred();
        // GetTree().ChangeSceneToPacked(ChangeScene);
        TransitionLayer.ChangeScene(ChangeScene);
    }
}
