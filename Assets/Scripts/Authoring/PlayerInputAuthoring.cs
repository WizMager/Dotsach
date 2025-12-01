using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        private class PlayerInputAuthoringBaker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<PlayerInputComponent>(entity);
            }
        }
    }
}