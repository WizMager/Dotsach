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
                var cameraTransform = CameraSingleton.Instance.transform;
                cameraTransform.position = playerTransform.ValueRO.Position + new float3(0, 1, 0);
                
                if (input.ValueRO.LookHorizontal != 0)
                {
                    playerTransform.ValueRW.Rotation = quaternion.RotateY(input.ValueRO.LookHorizontal);
                    cameraTransform.Rotate(Vector3.up, input.ValueRO.LookVertical);
                }

                if (input.ValueRO.LookVertical != 0)
                {
                    cameraTransform.Rotate(Vector3.right, input.ValueRO.LookVertical);
                }
            }
        }
    }
}