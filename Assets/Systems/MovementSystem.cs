using System.Runtime.InteropServices;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct MovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<PlayerInput>()
                .WithAll<Simulate>();
            state.RequireForUpdate((state.GetEntityQuery(builder)));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveJob = new MoveJob
            {
                Speed = 4 * SystemAPI.Time.DeltaTime,
                JumpForce = 4 * SystemAPI.Time.DeltaTime
            };
            state.Dependency = moveJob.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    public partial struct MoveJob : IJobEntity
    {
        public float Speed;
        public float JumpForce;
        private void Execute(PlayerInput input, ref LocalTransform transform)
        {
            var moveInput = new float2(input.Horizontal);
            var jumpInput = new float2(input.Vertical);


            moveInput = math.normalizesafe(moveInput) * Speed;
            jumpInput = math.normalizesafe(jumpInput) * JumpForce;

            transform.Position += new float3(moveInput.x, jumpInput.y, 0);
        }
    }
}