


using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Managers.SaveLoadManager;
/// <summary>
/// 保存接口  需要保存的对象都要实现该接口
/// </summary>
public interface ISavable
{
    /// <summary>
    /// 保存数据  key value:Godot.Variant
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, Variant> GetDataForSave();
}