using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TurnTowardsTargetSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var translationFromEntity = GetComponentDataFromEntity<Translation>();
		
		var handle = Entities.ForEach((ref Rotation rotation, in Translation translation, in TargetComponent target) =>
		{
			var targetTranslation = translationFromEntity[target.Value];
			var heading = targetTranslation.Value - translation.Value;
			rotation.Value = quaternion.LookRotation(heading, math.up());
		}).WithReadOnly(translationFromEntity).Schedule(Dependency);
		Dependency = handle;
	}
}