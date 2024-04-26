using Godot;

namespace Adventure.script;
/// <summary>
/// 音频总线枚举
/// </summary>
public enum SoundBus
{
    Master,
    Sfx,
    Bgm
}

/// <summary>
/// 音频管理器
/// </summary>
public partial class SoundManager : Node
{
    //音效
    public static Node SFX;
    //bgm
    public static AudioStreamPlayer BGMPlayer;
    public override void _Ready()
    {
        SFX = GetNode<Node>("SFX");
        BGMPlayer = GetNode<AudioStreamPlayer>("BGMPlayer");
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public static void PlaySFX(string name)
    {
        var audioStreamPlayer = SFX.GetNode<AudioStreamPlayer>(name);
        if (audioStreamPlayer == null) return;
        audioStreamPlayer.Play();
    }

    public static void PlayBGM(AudioStream bgm)
    {
        // 如果正在播放相同bgm则返回
        if (BGMPlayer.Stream == bgm && BGMPlayer.Playing) return;
        // 设置bgm
        BGMPlayer.Stream = bgm;
        // 播放bgm
        BGMPlayer.Play();
    }
    
    /// <summary>
    /// 设置节点音效
    /// </summary>
    /// <param name="node"></param>
    public static void SetUiAudio(Node node)
    {
        if (node == null) return;
        //按钮
        var button = node as Button;
        if (button != null)
        {
            button.Pressed += () => PlaySFX("UIPress");
            button.FocusEntered += () => PlaySFX("UIFocus");
            button.MouseEntered += () => button.GrabFocus();
        }
        //滑动器
        var slider = node as Slider;
        if (slider != null)
        {
            slider.ValueChanged += (V) => PlaySFX("UIPress");
            slider.FocusEntered += () => PlaySFX("UIFocus");
            slider.MouseEntered += () => slider.GrabFocus();
        }
        
        foreach (var child in node.GetChildren())
        {
            SetUiAudio(child);
        }
    }
    /// <summary>
    /// 获取音频总线分贝
    /// </summary>
    /// <param name="bus"></param>
    /// <returns></returns>
    public static float GetVolume(SoundBus bus)
    {
        //获取当前音频总线分贝
        var busVolumeDb = AudioServer.GetBusVolumeDb((int)bus);
        //转换为线性
        return Mathf.DbToLinear(busVolumeDb);
    }
    
    
    /// <summary>
    /// 设置音频总线分贝
    /// </summary>
    /// <param name="bus"></param>
    /// <returns></returns>
    public static void SetVolume(SoundBus bus, float v)
    {
        //转换为分贝
        var linearToDb = Mathf.LinearToDb(v);
        //设置音频总线分贝
        AudioServer.SetBusVolumeDb((int)bus, linearToDb);
    }
}