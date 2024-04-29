/// <summary>
/// 物理图层
/// </summary>
public enum PhysicsLayer
{
    BlockMovement = 1 << 0, //移动 1
    BlockSight = 1 << 1,    //视野 2
    PickableObject = 1 << 2,//可拾取 4
    Fog = 1 << 3,//战争迷雾 8
}
/// <summary>
/// 地图图层
/// </summary>
public enum TileMapLayer
{
    Default,
    Fog
}
/// <summary>
/// 主场景 地图类型
/// </summary>
public enum TerrainSet
{
    Default,
    Fog
}
/// <summary>
/// 地牢地形
/// </summary>
public enum DungeonTerrain
{
    Floor,
    Wall
}
/// <summary>
/// 森林地形
/// </summary>
public enum ForestTerrain
{
    Ground, //地面
    Grass, //草地
    Tree, //树
    DeadTree //死亡树
}

/// <summary>
/// 战争迷雾
/// </summary>
public enum FogTerrain
{
    Unexplored, //未探索
    Explored, //探索
    Visible //可见
}