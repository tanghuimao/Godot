using Godot;

namespace Adventure.script;
/// <summary>
/// 音量控件
/// </summary>
public partial class VolumeHSlider : HSlider
{
    //导出音量控件类型
    [Export] public SoundBus Bus { get; set; } = SoundBus.Master;
    
    public override void _Ready()
    {
        //设置音量值
        Value = SoundManager.GetVolume(Bus);
        
        //音量改变
        ValueChanged += v =>
        {
            //设置音量
            SoundManager.SetVolume(Bus, (float)v);
            //保存配置
            GameGlobal.SaveConfig();
        };
    }
}