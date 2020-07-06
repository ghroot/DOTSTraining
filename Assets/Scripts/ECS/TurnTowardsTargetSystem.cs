using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TurnTowardsTargetSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var handle = Entities.ForEach((ref Rotation rotation, in Translation translation, in TargeterComponent targeter) =>
		{
			float3 heading = targeter.Value.Value - translation.Value;
			rotation.Value = quaternion.LookRotation(heading, math.up());
		}).ScheduleParallel(Dependency);
		Dependency = handle;
	}
}