using Godot;
using System;
/// <summary>
/// 音频管理
/// </summary>
public partial class AudioManager : Node
{
    //音效
    public static Node SFX;
    public override void _Ready()
    {
        SFX = GetNode<Node>("SFX");
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="sfx"></param>
    public static void PlaySFX(SFX sfx)
    {
        var audioStreamPlayer = SFX.GetNode<AudioStreamPlayer>(sfx.ToString());
        if (audioStreamPlayer == null) return;
        audioStreamPlayer.Play();
    }
}

public enum SFX
{
    sfx_die,
    sfx_hit,
    sfx_point,
    sfx_swooshing,
    sfx_wing,
}
