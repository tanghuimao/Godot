using Godot;

namespace Adventure.script;

/// <summary>
/// 场景脚本
/// </summary>
public partial class World : Node2D
{
    //引入节点
    private TileMap _tileMap;
    private Camera2D _camera2D;
    private Player _player;
    private AudioStreamPlayer _audioStreamPlayer;
    /// <summary>
    /// 初始化执行
    /// </summary>
    public override void _Ready()
    {
        //获取节点
        _tileMap = GetNode<TileMap>("TileMap");
        _player = GetNode<Player>("Player");
        _camera2D = _player.Camera2D;
        
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        if (_audioStreamPlayer != null)
        {
            SoundManager.PlayBGM(_audioStreamPlayer.Stream);
        }
        
        //获取TileMap所站矩形大小 Grow -1 缩小一格
        var used = _tileMap.GetUsedRect().Grow(-1);
        //获取单个TileSet大小
        var tileSize = _tileMap.TileSet.TileSize;
        
        //设置相机显现位置
        _camera2D.LimitTop = used.Position.Y * tileSize.Y;
        _camera2D.LimitRight = used.End.X * tileSize.X;
        _camera2D.LimitBottom = used.End.Y * tileSize.Y;
        _camera2D.LimitLeft = used.Position.X * tileSize.X;
        
        //重置
        _camera2D.ResetSmoothing();
        //强制刷新
        _camera2D.ForceUpdateScroll();
    }
    /// <summary>
    /// 设置玩家位置
    /// </summary>
    /// <param name="position">位置</param>
    /// <param name="direction">方向</param>
    public void SetPlayerPosition(Vector2 position, Vector2 direction)
    {
        //设置玩家位置
        _player.GlobalPosition = position;
        //设置玩家方向
        _player.Graphics.Scale = direction;
        //重置相机位置
        _camera2D.ResetSmoothing();
        //强制刷新
        _camera2D.ForceUpdateScroll();
    }

}