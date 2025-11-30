using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ClientRequestGameEntrySystem : ISystem
    {
        private EntityQuery _networkIdQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
            _networkIdQuery = state.GetEntityQuery(builder);
            state.RequireForUpdate(_networkIdQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var networkIds = _networkIdQuery.ToEntityArray(Allocator.Temp);

            foreach (var networkId in networkIds)
            {
                ecb.AddComponent<NetworkStreamInGame>(networkId);
                var rpcEntity = ecb.CreateEntity();
                ecb.AddComponent<RequestCreatePlayerRpc>(rpcEntity);
                ecb.AddComponent(rpcEntity, new SendRpcCommandRequest
                {
                    TargetConnection = networkId
                });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}