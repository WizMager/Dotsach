using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;

namespace Components
{
    [MaterialProperty("_BaseColor")]
    public struct CharacterColorComponent : IComponentData
    {
        [GhostField] public float4 Color;
    }
}