using Godot;

namespace ClassicRoguelikeCourse.Resources.MapData;
/// <summary>
/// 地图数据
/// </summary>
public partial class MapData : Resource
{
    // 地图大小
    public Vector2I MapSize = new(60, 40);
    // 地图单元格大小
    public Vector2I CellSize = new(16, 16);
}