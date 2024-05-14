using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// 渐变层
/// </summary>
public partial class TransitionLayer : CanvasLayer
{
    private static SceneTree SceneTree { get; set; }
    private static AnimationPlayer _animationPlayer;
    public override void _Ready()
    {
        SceneTree = GetTree();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    
    public static async void ChangeScene(string scene)
    {
        //播放动画
        _animationPlayer.Play("fade_to_black");
        //等待1s再切换场景
        await Task.Delay(1000);
        //延迟调用
        Callable.From(() => SceneTree.ChangeSceneToFile(scene)).CallDeferred();
        //反转动画
        _animationPlayer.PlayBackwards("fade_to_black");
    }
}
