using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputConfigsComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<PlayerInputConfigsComponent>();

            foreach (var (transform, input, velocity) in 
                SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerInputComponent>, RefRW<PhysicsVelocity>>()
                    .WithAll<PlayerTag, Simulate>())
            {
                if (input.ValueRO is { MoveHorizontal: 0, MoveVertical: 0 })
                {
                    velocity.ValueRW.Linear = float3.zero;
                    continue;
                }
                
                var move = new float3(input.ValueRO.MoveHorizontal, 0, input.ValueRO.MoveVertical) * config.MoveSpeed;
                var rotatedMove = math.rotate(transform.ValueRO.Rotation, move);
                velocity.ValueRW.Linear = rotatedMove;
            }
        }
    }
}