using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargeterComponent : IComponentData
{
    public Entity Target;
}