using Godot;

namespace Adventure.script;
/// <summary>
/// 游戏结束界面
/// </summary>
public partial class GameOverScreen : Control
{

    public AnimationPlayer AnimationPlayer;
    
    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        
        
        
        //设置隐藏
        Hide();
        //设置隐藏不处理输入事件
        SetProcessInput(false);
    }


    /// <summary>
    /// _Input 事件最先进入
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        // 获取窗口 标记为处理
        GetWindow().SetInputAsHandled();
        //动画播放中不能进行操作
        if (AnimationPlayer.IsPlaying()) return;
        
        // 定义任意按键
        if (@event is InputEventKey ||
            @event is InputEventMouseButton || 
            @event is InputEventJoypadButton)
        {
            //判断按键按下并且不是回声
            if (@event.IsPressed() && !@event.IsEcho())
            {
                if (GameGlobal.HasSave())
                {
                    GameGlobal.GetInstance().LoadGame();
                }
                else
                {
                    GameGlobal.GetInstance().BackTitle();
                }
            }
        }
    }

    /// <summary>
    /// 显示死亡界面
    /// </summary>
    public void ShowGameOver()
    {
        //显示
        Show();
        //设置处理输入事件
        SetProcessInput(true);
        AnimationPlayer.Play("enter");
        
    }
}