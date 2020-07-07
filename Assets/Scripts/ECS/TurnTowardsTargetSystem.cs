using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TurnTowardsTargetSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var translationFromEntity = GetComponentDataFromEntity<Translation>();
		
		var handle = Entities.ForEach((ref Rotation rotation, in Translation translation, in TargeterComponent targeter) =>
		{
			if (translationFromEntity.HasComponent(targeter.Target))
			{
				var targetTranslation = translationFromEntity[targeter.Target];
				float3 heading = targetTranslation.Value - translation.Value;
				rotation.Value = quaternion.LookRotation(heading, math.up());
			}
		}).WithReadOnly(translationFromEntity).Schedule(Dependency);
		Dependency = handle;
	}
}