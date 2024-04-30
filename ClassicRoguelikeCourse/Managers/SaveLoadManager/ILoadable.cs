namespace ClassicRoguelikeCourse.Managers.SaveLoadManager;
/// <summary>
/// 加载接口  需要加载的对象都要实现该接口
/// </summary>
public interface ILoadable
{
    /// <summary>
    /// 通过存档数据进行初始化  成功返回ture 失败返回false  进行默认初始化
    /// </summary>
    /// <returns></returns>
    public bool InitializeByLoadData();
}