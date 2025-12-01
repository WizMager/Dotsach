using Unity.Entities;
using Unity.NetCode;
using Utils;

namespace Components
{
    public struct TeamComponent : IComponentData
    {
        [GhostField] public ETeam Value;
    }
}