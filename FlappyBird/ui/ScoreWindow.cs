using Godot;
using System;
using FlappyBird.Common;

/// <summary>
/// 分数窗口
/// </summary>
public partial class ScoreWindow : Node
{
    private int _score = 0;

    private Label _scoreLabel;
    public override void _Ready()
    {
        _scoreLabel = GetNode<Label>("%ScoreLabel");
        Game.AddScoreEvent += OnAddScoreEvent;
    }

    private void OnAddScoreEvent()
    {
        _score++;
        _scoreLabel.Text = _score.ToString();
        if (_score % 5 == 0)
        {
            Game.OnLevelUpEvent();
        }
    }

    public override void _ExitTree()
    {
        Game.AddScoreEvent -= OnAddScoreEvent;
    }
}