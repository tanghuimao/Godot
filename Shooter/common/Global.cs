using Godot;
using System;

/// <summary>
/// 全局变量
/// </summary>
public partial class Global : Node
{
    private static AudioStreamPlayer2D _hitMusic;
    public override void _Ready()
    {
        _hitMusic = new AudioStreamPlayer2D();
        _hitMusic.Stream = (AudioStream)GD.Load("res://audio/solid_impact.ogg");
        AddChild(_hitMusic);
    }

    /// <summary>
    /// 改变状态事件
    /// </summary>
    public static event Action ChangeStateEvent;
    //子弹数量
    private static int _laserAmount = 20;
    public static int LaserAmount
    {
        get => _laserAmount;
        set
        {
            _laserAmount = value;
            ChangeStateEvent?.Invoke();
        }
    }
    //榴弹数量
    private static int _grenadeAmount = 5;
    public static int GrenadeAmount
    {
        get => _grenadeAmount;
        set
        {
            _grenadeAmount = value;
            ChangeStateEvent?.Invoke();
        }
    }
    //生命值
    private static int _health = 100;
    public static int Health
    {
        get => _health;
        set
        {
            if (value >= 100)
            {
                value = 100;
            }

            if (value <= 0)
            {
                value = 0;
            }

            if (value < _health)
            {
                _hitMusic.Play();
            }
            _health = value;
            ChangeStateEvent?.Invoke();
        }
    }
    //玩家位置
    public static Vector2 PlayerPosition;

}