using Godot;
using System;
using FlappyBird.Common;

public partial class GameOver : Control
{
    private Button _startButton;
    private Button _endButton;
    public override void _Ready()
    {
        _startButton = GetNode<Button>("Container/StartButton");
        _endButton = GetNode<Button>("Container/EndButton");

        Game.GameOverEvent += OnGameOverEvent;

        _startButton.Pressed += OnPressed;
        
        _endButton.Pressed += () =>
        {
            GetTree().Quit();
        };
    }

    private void OnPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://scene/Main.tscn");
    }

    private void OnGameOverEvent()
    {
        Visible = true;
        GetTree().Paused = true;
    }

    public override void _ExitTree()
    {
        Game.GameOverEvent -= OnGameOverEvent;
        _startButton.Pressed -= OnPressed;
    }
}
