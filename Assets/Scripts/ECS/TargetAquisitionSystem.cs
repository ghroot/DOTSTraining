using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TargetAquisitionSystem : SystemBase
{
    private EntityQuery targeterQuery;
    private EntityQuery targetableQuery;
    
    protected override void OnCreate()
    {
        targeterQuery = GetEntityQuery(typeof(TargeterComponent), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>());
        targetableQuery = GetEntityQuery(typeof(TargetableComponent), ComponentType.ReadOnly<Translation>());
    }

    [BurstCompile]
    struct TargetSetterJob : IJobForEach<TargeterComponent, Translation, Rotation>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationFromEntity;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> TargetableEntities;

        public void Execute(ref TargeterComponent targeter, [ReadOnly] ref Translation pos, ref Rotation rotation)
        {
            if (!TranslationFromEntity.HasComponent(targeter.Target))
            {
                float f = (noise.snoise(pos.Value) + 1.0f) * 0.5f;
                int randomIndex = (int) math.round(f * TargetableEntities.Length);
                targeter.Target = TargetableEntities[randomIndex];
            }
        }
    }

    protected override void OnUpdate()
    {
        var job = new TargetSetterJob();
        job.TranslationFromEntity = GetComponentDataFromEntity<Translation>();
        job.TargetableEntities = targetableQuery.ToEntityArray(Allocator.TempJob);
        Dependency = job.Schedule(targeterQuery, Dependency);
    }
}