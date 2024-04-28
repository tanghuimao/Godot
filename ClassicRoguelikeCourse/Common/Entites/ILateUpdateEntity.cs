namespace ClassicRoguelikeCourse.Entites;
/// <summary>
/// 延迟更新接口
/// </summary>
public interface ILateUpdateEntity
{
    //延迟更新
    public void LateUpdate();
}