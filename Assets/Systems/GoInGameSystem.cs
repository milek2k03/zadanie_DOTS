using System.Runtime.InteropServices;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |  WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInGameClientSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpawner>();
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NetworkId>()
                .WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>()
                         .WithEntityAccess().WithNone<NetworkStreamInGame>())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(entity);
                var req = commandBuffer.CreateEntity();
                commandBuffer.AddComponent<GoInGame>(req);
                commandBuffer.AddComponent(req, new SendRpcCommandRequest { TargetConnection = entity });
            }
            commandBuffer.Playback(state.EntityManager);
        }
    }
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [StructLayout(LayoutKind.Auto)]
    public partial struct GoInGameServerSystem : ISystem
    {
        private ComponentLookup<NetworkId> _networkIdFromEntity;
    
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpawner>();
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GoInGame>()
                .WithAll<ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
            _networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefab = SystemAPI.GetSingleton<PlayerSpawner>().Player;
            state.EntityManager.GetName(prefab, out var prefabName);
            var worldName = state.WorldUnmanaged.Name;
        
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            _networkIdFromEntity.Update(ref state);
        
            foreach (var (reqSrc, reqEntity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
                         .WithAll<GoInGame>().WithEntityAccess())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.ValueRO.SourceConnection);
                var networkId = _networkIdFromEntity[reqSrc.ValueRO.SourceConnection];
                var player = commandBuffer.Instantiate(prefab);
                commandBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value});
                UnityEngine.Debug.Log($"'{worldName}' setting connection '{networkId.Value}' to in game, spawning a Ghost '{prefabName}' for them!");
                commandBuffer.AppendToBuffer(reqSrc.ValueRO.SourceConnection, new LinkedEntityGroup{Value = player});
                commandBuffer.DestroyEntity(reqEntity);
            }
            commandBuffer.Playback(state.EntityManager);
        }
    }
}