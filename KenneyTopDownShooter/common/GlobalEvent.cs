using System;

namespace KenneyTopDownShooter.common;

public class GlobalEvent
{
    // 子弹发射事件
    public static event Action<BulletSpawn> BulletFiredEvent;
    public static void OnBulletFiredEvent(BulletSpawn args)
    {
        BulletFiredEvent?.Invoke(args);
    }
}