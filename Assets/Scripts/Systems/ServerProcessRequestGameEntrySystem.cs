using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

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
            
            foreach (var (createPlayerRequestRpc, requestSource, rpcEntity) in 
                SystemAPI.Query<RequestCreatePlayerRpc, ReceiveRpcCommandRequest>()
                    .WithEntityAccess())
            {
                ecb.DestroyEntity(rpcEntity);
                ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

                var player = ecb.Instantiate(prefabs.PlayerPrefab);
                var spawnPosition = new float3(0, 1, 0);
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                ecb.SetComponent(player, newTransform);
                
                ecb.AddComponent(player, new GhostOwner
                {
                    NetworkId = 
                });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}