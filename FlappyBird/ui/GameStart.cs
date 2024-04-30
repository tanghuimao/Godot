using Godot;
using System;

public partial class GameStart : Control
{
    private Button _startButton;
    private Button _endButton;
    public override void _Ready()
    {
        _startButton = GetNode<Button>("Container/StartButton");
        _endButton = GetNode<Button>("Container/EndButton");

        _startButton.Pressed += () =>
        {
            GetTree().ChangeSceneToFile("res://scene/Main.tscn");
        };
        
        _endButton.Pressed += () =>
        {
            GetTree().Quit();
        };
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }
}
