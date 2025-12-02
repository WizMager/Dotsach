using Components;
using Unity.Burst;
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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerTransform, input) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInputComponent>>()
                    .WithAll<PlayerTag, Simulate>())
            {
                //TODO: to settings
                const float mouseSensitivity = 5f;
                const float minVerticalAngle = -80f;
                const float maxVerticalAngle = 80f;
                const float yCameraOffset = 1f;
                
                var cameraTransform = CameraSingleton.Instance.transform;
                var currentEuler = cameraTransform.rotation.eulerAngles;
                
                // Нормализуем углы Эйлера в диапазон -180..180
                var xAngle = currentEuler.x > 180f ? currentEuler.x - 360f : currentEuler.x;
                var yAngle = currentEuler.y > 180f ? currentEuler.y - 360f : currentEuler.y;
                
                if (input.ValueRO.LookHorizontal != 0)
                {
                    var rotationDeltaDegrees = input.ValueRO.LookHorizontal * mouseSensitivity;
                    yAngle += rotationDeltaDegrees;
                }
                
                if (input.ValueRO.LookVertical != 0)
                {
                    var verticalDelta = -input.ValueRO.LookVertical * mouseSensitivity;
                    xAngle += verticalDelta;
                    xAngle = math.clamp(xAngle, minVerticalAngle, maxVerticalAngle);
                }
                
                // Синхронизируем поворот персонажа с Y-углом камеры
                var yAngleRadians = math.radians(yAngle);
                playerTransform.ValueRW.Rotation = quaternion.Euler(0, yAngleRadians, 0);
                
                cameraTransform.rotation = Quaternion.Euler(xAngle, yAngle, 0);
                
                cameraTransform.position = playerTransform.ValueRO.Position + new float3(0, yCameraOffset, 0);
            }
        }
    }
}