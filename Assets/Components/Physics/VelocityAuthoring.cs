using Unity.Entities;
using UnityEngine;

namespace Components.Physics
{
    public struct MoveSpeed : IComponentData
    {
        public float Velocity;
    }
    public class VelocityAuthoring : MonoBehaviour
    {
        public float velocity;

        private class Baker : Baker<VelocityAuthoring>
        {
            public override void Bake(VelocityAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new MoveSpeed
                {
                    Velocity = authoring.velocity
                });
            }
        }
    }
}