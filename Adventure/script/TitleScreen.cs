using Godot;

namespace Adventure.script;
/// <summary>
/// 标题界面
/// </summary>
public partial class TitleScreen : Control
{
    //开始游戏按钮
    public Button NewGameButton;
    public Button LoadGameButton;
    public Button ExitGameButton;
    // public Label TitleLabel;
    public AudioStreamPlayer AudioStreamPlayer;
    public override void _Ready()
    {
        // TitleLabel = GetNode<Label>("TitleLabel");
        // //设置label宽度
        // var titleLabelSize = TitleLabel.Size;
        // titleLabelSize.X = GetParentAreaSize().X;
        // TitleLabel.Size = titleLabelSize;
        NewGameButton = GetNode<Button>("VBoxContainer/NewGame");
        LoadGameButton = GetNode<Button>("VBoxContainer/LoadGame");
        ExitGameButton = GetNode<Button>("VBoxContainer/ExitGame");
        AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        if (AudioStreamPlayer != null)
        {
            SoundManager.PlayBGM(AudioStreamPlayer.Stream);
        }

        //获取焦点
        NewGameButton.GrabFocus();
        //鼠标进入信号
        // NewGameButton.MouseEntered += () => { NewGameButton.GrabFocus(); };
        // LoadGameButton.MouseEntered += () => { LoadGameButton.GrabFocus(); };
        // ExitGameButton.MouseEntered += () => { ExitGameButton.GrabFocus(); };
        //点击信号
        NewGameButton.Pressed += OnNewGameButtonPressed;
        LoadGameButton.Pressed += OnLoadGameButtonPressed;
        ExitGameButton.Pressed += OnExitGameButtonPressed;
        
        //没有存档设置禁用
        if (!GameGlobal.HasSave())
        {
            LoadGameButton.Disabled = true;
        }
        //设置节点音效
        SoundManager.SetUiAudio(this);
    }
    
    /// <summary>
    /// 新游戏
    /// </summary>
    private void OnNewGameButtonPressed()
    {
        //加载存档
        GameGlobal.Instance.NewGame();
    }

    /// <summary>
    /// 加载存档
    /// </summary>
    private void OnLoadGameButtonPressed()
    {
        //加载存档
        GameGlobal.Instance.LoadGame();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnExitGameButtonPressed()
    {
        GameGlobal.Instance.ExitGame();
    }
    /// <summary>
    /// 设置
    /// </summary>
    private void OnSettingButtonPressed()
    {
        GameGlobal.Instance.OpenSetting();
    }
}