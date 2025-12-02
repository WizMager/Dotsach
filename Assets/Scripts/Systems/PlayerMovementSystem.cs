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
            state.RequireForUpdate<PlayerInputConfigsComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<PlayerInputConfigsComponent>();
            var speed = SystemAPI.Time.DeltaTime * config.MoveSpeed;

            foreach (var (transform, input) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInputComponent>>()
                    .WithAll<PlayerTag, Simulate>())
            {
                if (input.ValueRO is { MoveHorizontal: 0, MoveVertical: 0 })
                    continue;
                
                var move = new float3(input.ValueRO.MoveHorizontal, 0, input.ValueRO.MoveVertical) * speed;
                var rotatedMove = math.rotate(transform.ValueRO.Rotation, move);
                transform.ValueRW.Position += rotatedMove;
            }
        }
    }
}