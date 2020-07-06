
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var handle = Entities.WithAll<MoveForwardComponent>().ForEach((ref Translation t, in Rotation r) =>
        {
            t.Value = t.Value + deltaTime * 10.0f * math.forward(r.Value);
        }).ScheduleParallel(Dependency);

        Dependency = handle;
    }
}
