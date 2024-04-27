namespace ClassicRoguelikeCourse.Component.AiComponent;
/// <summary>
/// AI 抽象接口
/// </summary>
public interface IAi
{
    /// <summary>
    /// 初始化接口
    /// </summary>
    public void Initialize();
    
    /// <summary>
    /// 逻辑处理接口
    /// 检测是否可以执行  可以返回true 并中断后续AI行为检测
    /// </summary>
    public bool Execute();
}