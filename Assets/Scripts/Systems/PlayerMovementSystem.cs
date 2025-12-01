using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var speed = SystemAPI.Time.DeltaTime * 10;

            foreach (var (transform, input) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInputComponent>>()
                    .WithAll<PlayerTag, Simulate>())
            {
                if (input.ValueRO.MoveHorizontal == 0 && input.ValueRO.MoveVertical == 0)
                    continue;
                
                var move = new float3(input.ValueRO.MoveHorizontal, 0, input.ValueRO.MoveVertical) * speed;
                transform.ValueRW.Position += move;
            }
        }
    }
}