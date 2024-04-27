namespace ClassicRoguelikeCourse.Entites;
/// <summary>
/// 抽象接口
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 初始化接口
    /// </summary>
    public void Initialize();
    
    /// <summary>
    /// 逻辑处理接口
    /// </summary>
    /// <param name="delta"></param>
    public void Update(double delta);
}