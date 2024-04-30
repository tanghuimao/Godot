using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Managers.PickableObjectSpawner;
/// <summary>
/// 生成地面可拾取物品 （未拾取）
/// </summary>
public partial class PickableObjectSpawner : Node, IManager, ILoadable
{
    private SaveLoadManager.SaveLoadManager _saveLoadManager;
    private Node _pickableObjectContainer;
    public void Initialize()
    {
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager.SaveLoadManager>("%SaveLoadManager");
        _pickableObjectContainer = GetTree().CurrentScene.GetNode<Node>("%PickableObjectContainer");
        InitializeByLoadData();
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
            //获取地图中可拾取物品
            var pickableObjects = map["PickableObjects"].AsGodotArray<Dictionary<string, Variant>>();
            for(int j = 0; j < pickableObjects.Count; j++)
            {
                var pickableObject = pickableObjects[j];
                //创建实例
                var pickableObjectInstance = GD.Load<PackedScene>(pickableObject["ScenePath"].AsString()).Instantiate<PickableObject>();
                //添加到场景中
                _pickableObjectContainer.AddChild(pickableObjectInstance);
                //位置
                pickableObjectInstance.GlobalPosition = pickableObject["Position"].AsVector2();
                //初始化
                pickableObjectInstance.Initialize();
            }

            return true;
        }

        return false;
    }
}