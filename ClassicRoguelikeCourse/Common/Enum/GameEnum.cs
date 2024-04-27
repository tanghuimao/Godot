/// <summary>
/// 物理图层
/// </summary>
public enum PhysicsLayer
{
    BlockMovement = 1 << 0,
    BlockSight = 1 << 1,
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
public enum DungeonTerrainSet
{
    Floor,
    Wall
}
/// <summary>
/// 森林地形
/// </summary>
public enum ForestTerrainSet
{
}