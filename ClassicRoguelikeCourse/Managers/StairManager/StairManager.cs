using System;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using Godot;
using Godot.Collections;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

namespace ClassicRoguelikeCourse.Managers.StairManager;

/// <summary>
/// 楼梯管理器
/// </summary>
public partial class StairManager : Node, IManager, ILoadable
{
    //上层场景路径
    [Export] private string _previousScenePath;

    //下层场景路径
    [Export] private string _nextScenePath;

    //地图管理器
    private MapManager.MapManager _mapManager;

    //输入处理
    private InputHandler _inputHandler;

    //玩家
    private Player _player;

    //地图瓦片地图
    private TileMap _tileMap;

    //上下楼梯位置
    private Vector2 _upStairPosition;
    private Vector2 _downStairPosition;

    private SaveLoadManager.SaveLoadManager _saveLoadManager;

    public void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager.MapManager>("%MapManager");
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        _tileMap = GetTree().CurrentScene.GetNode<TileMap>("%TileMap");
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager.SaveLoadManager>("%SaveLoadManager");
        
        if (!InitializeByLoadData())
        {
            GenerateUpStair();
            GenerateDownStair();
        }
    }

    public void Update(double delta)
    {
    }

    /// <summary>
    /// 存档加载数据
    /// </summary>
    /// <returns></returns>
    public bool InitializeByLoadData()
    {
        if (_saveLoadManager.LoadedData == null ||
            _saveLoadManager.LoadedData.Count == 0 ||
            !_saveLoadManager.LoadedData.ContainsKey("Maps")) return false;

        var maps = _saveLoadManager.LoadedData["Maps"].AsGodotArray<Dictionary<string, Variant>>();
        for (int i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            //名称 和 当前保存场景相同
            if (map["SceneName"].AsString() != GetTree().CurrentScene.Name) continue;
            //获取上下楼梯单元格
            var upStairCell = map["UpStairCell"].AsVector2I();
            var downStairCell = map["DownStairCell"].AsVector2I();
            //获取上下楼梯位置
            _upStairPosition = upStairCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
            _downStairPosition = downStairCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;

            //设置楼梯
            if (upStairCell != Vector2I.Zero)
            {
                //设置上层楼梯
                _tileMap.SetCellsTerrainConnect(
                    (int)TileMapLayer.Default,
                    new Array<Vector2I> { upStairCell },
                    (int)TerrainSet.Stair,
                    (int)StairTerrain.UpStair,
                    false
                );
            }

            //设置楼梯
            if (downStairCell != Vector2I.Zero)
            {
                //设置上层楼梯
                _tileMap.SetCellsTerrainConnect(
                    (int)TileMapLayer.Default,
                    new Array<Vector2I> { downStairCell },
                    (int)TerrainSet.Stair,
                    (int)StairTerrain.DownStair,
                    false
                );
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 尝试返回上层场景
    /// </summary>
    public void TryGoToPreviousScene()
    {
        //如果上层场景路径为空，则不生成
        if (string.IsNullOrEmpty(_previousScenePath)) return;

        if (Mathf.IsEqualApprox(_player.GlobalPosition.X, _upStairPosition.X) &&
            Mathf.IsEqualApprox(_player.GlobalPosition.Y, _upStairPosition.Y))
        {
            //存档
            _saveLoadManager.Save();
            //切换场景
            GetTree().ChangeSceneToFile(_previousScenePath);
        }
    }

    /// <summary>
    /// 尝试进入下层场景
    /// </summary>
    public void TryGoToNextScene()
    {
        //如果下场景路径为空，则不生成
        if (string.IsNullOrEmpty(_nextScenePath)) return;

        if (Mathf.IsEqualApprox(_player.GlobalPosition.X, _downStairPosition.X) &&
            Mathf.IsEqualApprox(_player.GlobalPosition.Y, _downStairPosition.Y))
        {
            //存档
            _saveLoadManager.Save();
            //切换场景
            GetTree().ChangeSceneToFile(_nextScenePath);
        }
    }

    /// <summary>
    /// 生成上行楼梯
    /// </summary>
    private void GenerateUpStair()
    {
        //如果上层场景路径为空，则不生成
        if (string.IsNullOrEmpty(_previousScenePath)) return;
        //设置上层楼梯位置
        _upStairPosition = _player.GlobalPosition;
        //上行楼梯所在单元格位置
        var upStairCell = (Vector2I)(_upStairPosition - _mapManager.MapData.CellSize / 2) /
                          _mapManager.MapData.CellSize;
        //设置上层楼梯
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Default,
            new Array<Vector2I> { upStairCell },
            (int)TerrainSet.Stair,
            (int)StairTerrain.UpStair,
            false
        );
    }

    /// <summary>
    /// 生成下行楼梯
    /// </summary>
    private void GenerateDownStair()
    {
        //如果下层场景路径为空，则不生成
        if (string.IsNullOrEmpty(_nextScenePath)) return;

        //2D世界物理空间状态
        var space = _tileMap.GetWorld2D().DirectSpaceState;
        while (true)
        {
            //随机生成
            var randomX = GD.RandRange(1, _mapManager.MapData.MapSize.X - 2);
            var randomY = GD.RandRange(1, _mapManager.MapData.MapSize.Y - 2);
            //单元格
            var randomCell = new Vector2I(randomX, randomY);

            //下行楼梯所位置
            var randomCellPosition = randomCell * _mapManager.MapData.CellSize + _mapManager.MapData.CellSize / 2;
            //检测单元格内是否存在其他碰撞体
            var parameter = new PhysicsPointQueryParameters2D
            {
                Position = randomCellPosition,
                CollisionMask = (uint)PhysicsLayer.BlockMovement,
                Exclude = new Array<Rid> { _player.GetNode<Area2D>("Area2D").GetRid() },
            };
            //检测结果
            var results = space.IntersectPoint(parameter);
            if (results.Count > 0) continue;

            _downStairPosition = randomCellPosition;

            //设置下行楼梯
            _tileMap.SetCellsTerrainConnect(
                (int)TileMapLayer.Default,
                new Array<Vector2I> { randomCell },
                (int)TerrainSet.Stair,
                (int)StairTerrain.DownStair,
                false
            );
            return;
        }
    }
}