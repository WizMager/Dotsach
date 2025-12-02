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
            //TODO: to settings
            const float moveSpeed = 10;
            
            var speed = SystemAPI.Time.DeltaTime * moveSpeed;

            foreach (var (transform, input) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInputComponent>>()
                    .WithAll<PlayerTag, Simulate>())
            {
                if (input.ValueRO.MoveHorizontal == 0 && input.ValueRO.MoveVertical == 0)
                    continue;
                
                var move = new float3(input.ValueRO.MoveHorizontal, 0, input.ValueRO.MoveVertical) * speed;
                var rotatedMove = math.rotate(transform.ValueRO.Rotation, move);
                transform.ValueRW.Position += rotatedMove;
            }
        }
    }
}