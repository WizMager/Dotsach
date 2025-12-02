using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Utils;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerLookSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerTransform, input, config) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInputComponent>, RefRO<PlayerInputConfigsComponent>>()
                    .WithAll<PlayerTag, Simulate>())
            {
                var cameraTransform = CameraSingleton.Instance.transform;
                var currentEuler = cameraTransform.rotation.eulerAngles;
                
                var xAngle = currentEuler.x > 180f ? currentEuler.x - 360f : currentEuler.x;
                var yAngle = currentEuler.y > 180f ? currentEuler.y - 360f : currentEuler.y;
                
                if (input.ValueRO.LookHorizontal != 0)
                {
                    var rotationDeltaDegrees = input.ValueRO.LookHorizontal * config.ValueRO.MouseSensitivity;
                    yAngle += rotationDeltaDegrees;
                }
                
                var yAngleRadians = math.radians(yAngle);
                
                if (input.ValueRO.LookVertical != 0)
                {
                    var verticalDelta = -input.ValueRO.LookVertical * config.ValueRO.MouseSensitivity;
                    xAngle += verticalDelta;
                    xAngle = math.clamp(xAngle, config.ValueRO.MinVerticalAngle, config.ValueRO.MaxVerticalAngle);
                }
                
                playerTransform.ValueRW.Rotation = quaternion.Euler(0, yAngleRadians, 0);
                cameraTransform.rotation = Quaternion.Euler(xAngle, yAngle, 0);
                cameraTransform.position = playerTransform.ValueRO.Position + new float3(0, config.ValueRO.YCameraOffset, 0);
            }
        }
    }
}