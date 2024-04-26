using Godot;

namespace Adventure.script;
/// <summary>
/// 暂停界面  需要设置处理模式 When paused
/// </summary>
public partial class PauseScreen : Control
{

    public Button Resume;
    public Button Quit;
    
    public override void _Ready()
    {
        //设置不显示界面
        Hide();
        //设置音效
        SoundManager.SetUiAudio(this);
        Resume = GetNode<Button>("VBox/Actions/HBox/Resume");
        Quit = GetNode<Button>("VBox/Actions/HBox/Quit");
        //连接信号
        Resume.Pressed += OnResume;
        Quit.Pressed += OnQuit;
        
        //场景可用状态变更时信号连接  
        VisibilityChanged += () => GetTree().Paused = Visible;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //玩家按下esc
        if (@event.IsActionPressed("pause"))
        {
            OnResume();
            //标记已处理
            GetWindow().SetInputAsHandled();
        }
    }

    /// <summary>
    /// 显示暂停界面
    /// </summary>
    public void ShowPause()
    {
        Show();
    }
    /// <summary>
    /// 继续游戏
    /// </summary>
    private void OnResume()
    {
        //隐藏当前场景
        Hide();
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnQuit()
    {
        //返回主界面
        GameGlobal.Instance.BackTitle();
    }


}