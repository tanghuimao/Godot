using System;
using Godot;

namespace ClassicRoguelikeCourse.Managers.Fsm;

public interface IState
{
    //状态更新事件
    public event Action<IState> UpdateEvent;
    //进入
    public void Enter();
    //退出
    public void Exit();
    //视觉帧调用
    public void Update(double delta);
}