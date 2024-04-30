using Godot;
using System;
using FlappyBird.Common;

/// <summary>
/// 管道管理器
/// </summary>
public partial class PipeManager : Node
{
    private short _minDistance = 200;
    private short _maxDistance = 280;
    private short _minAddDistanceInterval = 5;
    private double _minWaitTime = 0.8;
    private double _reduceWaitTimeInterval = 0.2;
    
    private Timer _timer;

    private PackedScene _pipeScene = GD.Load<PackedScene>("res://scene/Pipe.tscn");
    
    private bool _isRuleSpawn = false;
    private short _ruleMode = 0;  // 0+   1-
    private short _ruleSpawnStep = 20;
    private short _minRandomRange = -150;
    private short _maxRandomRange = 150;
    private int _lastRandomRange;
    
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        // 定时器 事件
        _timer.Timeout += OnTimeout;
        Game.GameOverEvent += OnGameOverEvent;
        Game.LevelUpEvent += OnLevelUpEvent;
    }

    public override void _ExitTree()
    {
        Game.GameOverEvent -= OnGameOverEvent;
        Game.LevelUpEvent -= OnLevelUpEvent;
    }

    private void OnLevelUpEvent()
    {
        if (_timer.WaitTime > _minWaitTime)
        {
            _timer.WaitTime -= _reduceWaitTimeInterval;
            if (_timer.WaitTime <= 1 && !_isRuleSpawn)
            {
                _isRuleSpawn = true;
            }
        }

        if (_minDistance < _maxDistance)
        {
            _minDistance += _minAddDistanceInterval;
        }
    }

    private void OnGameOverEvent()
    {
        _timer.Stop();
    }


    private void OnTimeout()
    {
        if (!_isRuleSpawn)
        {
            _lastRandomRange = GD.RandRange(_minRandomRange, _maxRandomRange);
            GD.Print("random spawn _lastRandomRange: " + _lastRandomRange);
        }
        else
        {
            RuleRandomRange();
            GD.Print("rule spawn _lastRandomRange: " + _lastRandomRange);
        }
        
        var pipeInstance = _pipeScene.Instantiate<Pipe>();
        RandomPosition(pipeInstance);
        pipeInstance.Position = new Vector2(pipeInstance.Position.X, pipeInstance.Position.Y + _lastRandomRange);
        AddChild(pipeInstance);
    }

    private void RuleRandomRange()
    {
        if (_ruleMode == 0)
        {
            if (_lastRandomRange >= _maxRandomRange)
            {
                _ruleMode = 1;
                return;
            }
            _lastRandomRange += _ruleSpawnStep;
        }
        else
        {
            if (_lastRandomRange <= _minRandomRange)
            {
                _ruleMode = 0;
                return;
            }
            _lastRandomRange -= _ruleSpawnStep;
        }
    }


    private void RandomPosition(Pipe pipe)
    {
        var pipeUp = pipe.GetNode<StaticBody2D>("StaticBody2DPipeUp");
        pipeUp.Position = new Vector2(pipeUp.Position.X, pipeUp.Position.Y + _minDistance);
        var pipeDown = pipe.GetNode<StaticBody2D>("StaticBody2DPipeDown");
        pipeDown.Position = new Vector2(pipeDown.Position.X, pipeDown.Position.Y - _minDistance);
    }
}
