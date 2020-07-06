using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct TargeterComponent : IComponentData
{
    // public Entity Value;
    public Translation Value;
}