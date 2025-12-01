using Components;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class PlayerAuthoring : MonoBehaviour
    {
        private class PlayerAuthoringBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<PlayerTag>(entity);
                AddComponent<TeamComponent>(entity);
                AddComponent<CharacterColorComponent>(entity);
                AddComponent<URPMaterialPropertyBaseColor>(entity);
            }
        }
    }
}