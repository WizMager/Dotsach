using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Utils;
using Random = Unity.Mathematics.Random;

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
                float4 characterColor;
                
                switch (team.ValueRO.Value)
                {
                    case ETeam.Red:
                        characterColor = new float4(1, 0, 0, 1);
                        break;
                    case ETeam.Blue:
                        characterColor = new float4(0, 0, 1, 1);
                        break;
                    default:
                        GenerateColorFromNetworkId(ghostOwner.ValueRO.NetworkId, out characterColor);
                        break;
                }
                
                ecb.SetComponent(entity, new CharacterColorComponent
                {
                    Color = characterColor
                });
                
                ecb.RemoveComponent<NewPlayerCharacterTag>(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
        
        [BurstCompile]
        private static void GenerateColorFromNetworkId(int networkId, out float4 color)
        {
            var random = Random.CreateFromIndex((uint)networkId);
            var r = random.NextFloat(0.3f, 1f);
            var g = random.NextFloat(0.3f, 1f);
            var b = random.NextFloat(0.3f, 1f);
            
            color =  new float4(r, g, b, 1.0f);
        }
    }
}