using Godot;
using ClassicRoguelikeCourse.Entites;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Managers.MapManager;
using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using Godot.Collections;
using Player = ClassicRoguelikeCourse.Entities.Characters.Player.Player;

/// <summary>
/// 拾取物父类接口
/// </summary>
public partial class PickableObject : Node2D, IEntity, ISavable
{
    // 名称
    [Export] protected string _name;

    public string Name => _name;

    // 描述
    protected string _description;
    public string Description => _description;

    // 地图管理器
    protected MapManager _mapManager;

    // 地图
    protected TileMap _tileMap;

    // 玩家
    protected Player _player;

    public virtual void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _tileMap = GetTree().CurrentScene.GetNode<TileMap>("%TileMap");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
    }

    public void Update(double delta)
    {
    }

    public Dictionary<string, Variant> GetDataForSave()
    {
        return new Dictionary<string, Variant>
        {
            {"ScenePath", SceneFilePath},
            {"Position", GlobalPosition},
        };
    }
}