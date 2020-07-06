using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// using Random = UnityEngine.Random;

public class TargetAquisitionSystem : SystemBase
{
    private EntityQuery targetableQuery;
    private EntityQuery targeterQuery;
    
    protected override void OnCreate()
    {
        targetableQuery = GetEntityQuery(typeof(TargetableComponent), ComponentType.ReadOnly<Translation>());
        targeterQuery = GetEntityQuery(typeof(TargeterComponent), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>());
    }

    [BurstCompile]
    struct TargetSetterJob : IJobForEach<TargeterComponent, Translation, Rotation>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> targetPositions;
        // [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> targets;
        // [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<TeamComponent> teams;

        public void Execute(ref TargeterComponent targeter, [ReadOnly] ref Translation pos, ref Rotation rotation)
        {
            if (math.distance(targeter.Value.Value, float3.zero) < 0.1f)
            {
                float f = (noise.snoise(pos.Value) + 1.0f) * 0.5f;
                int r = (int)math.round(f * targetPositions.Length);
                targeter.Value = targetPositions[r];
            }
        }
    }

    protected override void OnUpdate()
    {
        TargetSetterJob job = new TargetSetterJob();
        job.targetPositions = targetableQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        Dependency = job.Schedule(targeterQuery, Dependency);
    }
}