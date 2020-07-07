using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TargetAquisitionSystem : SystemBase
{
    private EntityQuery targeterQuery;
    private EntityQuery targetableQuery;
    
    private EndSimulationEntityCommandBufferSystem bufferSystem;
    
    protected override void OnCreate()
    {
        targeterQuery = GetEntityQuery(typeof(TargeterComponent), typeof(Translation), ComponentType.ReadOnly<TeamComponent>(), ComponentType.Exclude(typeof(TargetComponent)));
        targetableQuery = GetEntityQuery(typeof(TargetableComponent), ComponentType.ReadOnly<Translation>());

        bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct TargetSetterJob : IJobForEachWithEntity<Translation, TeamComponent>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> TargetableEntities;
        public EntityCommandBuffer.Concurrent Buffer;
        [ReadOnly] public ComponentDataFromEntity<TeamComponent> TeamsForEntity;

        public void Execute(Entity entity, int index, ref Translation translation, [ReadOnly] ref TeamComponent team)
        {
            var f = (noise.snoise(translation.Value) + 1.0f) * 0.5f;
            var randomIndex = (int) math.round(f * TargetableEntities.Length);
            var targetableEntity = TargetableEntities[randomIndex];

            var validTarget = false;
            if (TeamsForEntity.Exists(targetableEntity))
            {
                validTarget = TeamsForEntity[targetableEntity].Value != team.Value;
            }
            else
            {
                validTarget = true;
            }

            if (validTarget)
            {
                Buffer.AddComponent<TargetComponent>(index, entity);
                var target = new TargetComponent();
                target.Value = targetableEntity;
                Buffer.SetComponent(index, entity, target);
            }
        }
    }

    protected override void OnUpdate()
    {
        var job = new TargetSetterJob();
        job.TargetableEntities = targetableQuery.ToEntityArray(Allocator.TempJob);
        job.Buffer = bufferSystem.CreateCommandBuffer().ToConcurrent();
        job.TeamsForEntity = GetComponentDataFromEntity<TeamComponent>();
        Dependency = job.Schedule(targeterQuery, Dependency);
        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}