using System;
using System.Collections.Generic;
using Godot;

namespace ClassicRoguelikeCourse.Component.AiComponent;
/// <summary>
/// Ai组件 模块
/// </summary>
public partial class AiComponent : Node, IComponent
{
    private List<IAi> _ais = new();
    
    public void Initialize()
    {
        // 找到所有AI组件并初始化
        foreach (var child in GetChildren())
        {
            if (child is not IAi ai) continue;
            ai.Initialize();
            _ais.Add(ai);
        }
    }

    public void Update(double delta)
    {
        // 执行所有AI行为
        foreach (var ai in _ais)
        {
            // 检测是否可以执行AI行为  可以执行 中断后续检测
            if (ai.Execute()) break;
        }
    }
}