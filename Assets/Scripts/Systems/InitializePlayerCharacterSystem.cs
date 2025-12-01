using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using Utils;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct InitializePlayerCharacterSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (team, ghostOwner, entity) in 
                SystemAPI.Query<RefRO<TeamComponent>, RefRO<GhostOwner>>()
                    .WithAll<NewPlayerCharacterTag>()
                    .WithEntityAccess())
            {
                var characterColor = team.ValueRO.Value switch
                {
                    ETeam.Red => new float4(1, 0, 0, 1),
                    ETeam.Blue => new float4(0, 0, 1, 1),
                    _ => GenerateColorFromNetworkId(ghostOwner.ValueRO.NetworkId)
                };
                
                ecb.SetComponent(entity, new URPMaterialPropertyBaseColor
                {
                    Value = characterColor
                });
                
                ecb.RemoveComponent<NewPlayerCharacterTag>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
        
        [BurstCompile]
        private static float4 GenerateColorFromNetworkId(int networkId)
        {
            var random = Random.CreateFromIndex((uint)networkId);
            var r = random.NextFloat(0.3f, 1f);
            var g = random.NextFloat(0.3f, 1f);
            var b = random.NextFloat(0.3f, 1f);
            
            return new float4(r, g, b, 1.0f);
        }
    }
}