using Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct PlayerInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpawner>();
            state.RequireForUpdate<PlayerInput>();
            state.RequireForUpdate<NetworkId>();
        }
    
        public void OnUpdate(ref SystemState state)
        {
            var left = Input.GetKey("left");
            var right = Input.GetKey("right");
            var down = Input.GetKey("down");
            var up = Input.GetKey("up");
        
            foreach (var input in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
            {
                input.ValueRW = default;
                if (left)
                    input.ValueRW.Horizontal -= 1;
                if (right)
                    input.ValueRW.Horizontal += 1;
                if (down)
                    input.ValueRW.Vertical -= 1;
                if (up)
                    input.ValueRW.Vertical += 1;
            }
        }
    }
}