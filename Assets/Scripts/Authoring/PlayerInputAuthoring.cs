using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        public float MouseSensitivity;
        public float MinVerticalAngle;
        public float MaxVerticalAngle;
        public float YCameraOffset;
        
        private class PlayerInputAuthoringBaker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<PlayerInputComponent>(entity);
                AddComponent(entity, new PlayerInputConfigsComponent
                {
                    MoveSpeed = authoring.MoveSpeed,
                    MouseSensitivity = authoring.MouseSensitivity,
                    MinVerticalAngle = authoring.MinVerticalAngle,
                    MaxVerticalAngle = authoring.MaxVerticalAngle,
                    YCameraOffset = authoring.YCameraOffset
                });
            }
        }
    }
}