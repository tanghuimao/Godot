using Godot;
using System;
using ClassicRoguelikeCourse.Managers.AStarGridManager;
using ClassicRoguelikeCourse.Managers.MapManager;

/// <summary>
/// 网格地图调试
/// </summary>
public partial class AStarGridManagerDebugger : Node2D
{
    //地图管理器
    private MapManager _mapManager;
    private AStarGridManager _aStarGridManager;
    public override void _Ready()
    {
        //获取地图管理器
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _aStarGridManager = GetOwner<AStarGridManager>();
    }

    public override void _Process(double delta)
    {
        //刷新 保证调用 _Draw
        QueueRedraw();
    }

    //绘制
    public override void _Draw()
    {
        //标记不能行动的单元格
        if (_aStarGridManager?.AStarGrid2D == null) return;
        for (int x = 0; x < _mapManager.MapData.MapSize.X; x++)
        {
            for (int y = 0; y < _mapManager.MapData.MapSize.Y; y++)
            {
                //获取单元格
                var cell = new Vector2I(x, y);
                if (_aStarGridManager.AStarGrid2D.IsPointSolid(cell))
                {
                    //绘制圆形 
                    DrawCircle(cell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2, 4, Colors.Red);
                }
            }
        }
    }
}
