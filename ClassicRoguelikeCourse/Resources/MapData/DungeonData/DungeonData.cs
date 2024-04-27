using Godot;

namespace ClassicRoguelikeCourse.Resources.MapData.DungeonData;
/// <summary>
/// 地牢地图数据
/// </summary>
public partial class DungeonData : MapData
{
    //房间最大大小
    public Vector2I MaxRoomSize = new(15, 15);
    //房间最小大小
    public Vector2I MinRoomSize = new(5, 5);
}