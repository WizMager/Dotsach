using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Utils;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessRequestGameEntrySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabsComponent>();
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<RequestCreatePlayerRpc, ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var prefabs = SystemAPI.GetSingleton<PrefabsComponent>();
            
            foreach (var (requestSource, rpcEntity) in 
                SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
                    .WithAll<RequestCreatePlayerRpc>()
                    .WithEntityAccess())
            {
                ecb.DestroyEntity(rpcEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.ValueRO.SourceConnection);

                var networkId = SystemAPI.GetComponent<NetworkId>(requestSource.ValueRO.SourceConnection);
                var playerCharacter = ecb.Instantiate(prefabs.PlayerPrefab);
                var spawnPosition = new float3(0, 1, 0);
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(playerCharacter, newTransform);
                
                ecb.SetComponent(playerCharacter, new TeamComponent
                {
                    Value = ETeam.None
                });
                
                ecb.AddComponent(playerCharacter, new GhostOwner
                {
                    NetworkId = networkId.Value
                });
                
                ecb.AppendToBuffer(requestSource.ValueRO.SourceConnection, new LinkedEntityGroup
                {
                    Value = playerCharacter
                });
                
                ecb.AddComponent<NewPlayerCharacterTag>(playerCharacter);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}