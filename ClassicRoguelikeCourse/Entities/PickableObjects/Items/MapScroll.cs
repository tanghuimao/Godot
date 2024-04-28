using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;

/// <summary>
/// 地图卷
/// </summary>
public partial class MapScroll : Item, IConsumableItem
{
    public override void Initialize()
    {
        base.Initialize();
        _description = "使用后消除本地图战争迷雾";
    }

    public void Consume()
    {
        //未解锁的战争迷雾单元格
        var unexploredFogCell = new Array<Vector2I>();
        //遍历地图
        for (int x = 0; x < _mapManager.MapData.MapSize.X; x++)
        {
            for (int y = 0; y < _mapManager.MapData.MapSize.Y; y++)
            {
                //单元格
                var cell = new Vector2I(x, y);
                //获取当前单元格图块数据
                var tileData = _tileMap.GetCellTileData((int)TileMapLayer.Fog, cell);
                //如果是战争迷雾且未探索
                if (tileData.TerrainSet == (int)TerrainSet.Fog && tileData.Terrain == (int)FogTerrain.Unexplored)
                {
                    unexploredFogCell.Add(cell);
                }
            }
        }

        //设置战争迷雾为探索
        _tileMap.SetCellsTerrainConnect(
            (int)TileMapLayer.Fog,
            unexploredFogCell,
            (int)TerrainSet.Fog,
            (int)FogTerrain.Explored
        );

        //移除使用物品
        (_player.CharacterData as PlayerData).Inventory.Remove(this);
    }
}