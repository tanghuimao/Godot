using Godot;
using System;

public partial class OutSide : Level
{
    [Export]
    public string ChangeScene;
    //门
    private Gate _gate;
    //房间
    private House _house;
    public override void _Ready()
    {
        base._Ready();
        //门
        _gate = GetNode<Gate>("Gate");
        _gate.InGateEvent += OnInGateEvent;
        //房间
        _house = GetNode<House>("Ground/House");
        if (_house != null)
        {
            _house.EnterHouseEvent += OnEnterHouseEvent;
            _house.ExitHouseEvent += OnExitHouseEvent;
        }
    }

    public override void _ExitTree()
    {
        _gate.InGateEvent -= OnInGateEvent;
        _house.EnterHouseEvent -= OnEnterHouseEvent;
        _house.ExitHouseEvent -= OnExitHouseEvent;
    }

    /// <summary>
    /// 玩家进入门
    /// </summary>
    /// <param name="obj"></param>
    private void OnInGateEvent(Node2D obj)
    {
        var tween = CreateTween();
        //播放动画 修改自定义属性
        tween.TweenProperty(Player, "_speed", 0, 0.5);

        // GetTree().ChangeSceneToFile(ChangeScene);
        //延迟调用
        // Callable.From(() => GetTree().ChangeSceneToFile(ChangeScene)).CallDeferred();
        // GetTree().ChangeSceneToPacked(ChangeScene);
        TransitionLayer.ChangeScene(ChangeScene);
    }
    
    /// <summary>
    /// 玩家进入房间
    /// </summary>
    private void OnEnterHouseEvent()
    {
        var tween = CreateTween();
        //同时多个动画
        tween.SetParallel(true);
        tween.TweenProperty(Camera, "zoom", new Vector2(1, 1), 0.5f);
        tween.SetEase(Tween.EaseType.Out);
    }
    
    /// <summary>
    /// 玩家退出房间
    /// </summary>
    private void OnExitHouseEvent()
    {
        var tween = CreateTween();
        tween.TweenProperty(Camera, "zoom", new Vector2(0.6f, 0.6f), 0.5f);
        tween.SetEase(Tween.EaseType.Out);
    }
}
