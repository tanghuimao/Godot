using System.Collections.Generic;
using Godot;

namespace Adventure.script;

public partial class GameEndScreen : Control
{
    public List<string> Texts = new List<string>()
    {
        "大魔王被打败了",
        "世界又恢复了往日的宁静",
        "按任意键键返回主菜单",
    };
    //文字索引
    public short Index = -1;


    public Label Label;
    
    public Tween Tween;
    public AudioStreamPlayer AudioStreamPlayer;
    
    public override void _Ready()
    {
        //设置文字索引
        Index = 0;
        Label = GetNode<Label>("Label");
        AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        if (AudioStreamPlayer != null)
        {
            SoundManager.PlayBGM(AudioStreamPlayer.Stream);
        }
        //初始加载显示第一行
        ShowLine(Index);
    }

    /// <summary>
    /// _Input 事件最先进入
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        //动画播放中不能进行操作
        if (Tween.IsRunning()) return;
        
        // 定义任意按键
        if (@event is InputEventKey ||
            @event is InputEventMouseButton || 
            @event is InputEventJoypadButton)
        {
            //判断按键按下并且不是回声
            if (@event.IsPressed() && !@event.IsEcho())
            {
                if (Index < Texts.Count - 1)
                {
                    Index++;
                    ShowLine(Index);
                }
                else
                {
                    GameGlobal.GetInstance().BackTitle();
                }
            }
        }
    }
    
    public void ShowLine(short index)
    {
        //创建动画
        Tween = CreateTween();
        //设置动画曲线
        Tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        if (index > 0)
        {   
            //创建动画
            Tween.TweenProperty(Label, "modulate:a", 0, 1);
        }
        else
        {
            var labelModulate = Label.Modulate;
            labelModulate.A = 0;
            Label.Modulate = labelModulate;
        }
        //设置回调函数
        Tween.TweenCallback(Callable.From(() => SetText(Texts[index])));
        //创建属性动画
        Tween.TweenProperty(Label, "modulate:a", 1, 1);
    }

    public void SetText(string text)
    {
        Label.Text = text;
    }
    
}