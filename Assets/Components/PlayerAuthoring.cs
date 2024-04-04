using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct Player : IComponentData
    {
    
    }
    [DisallowMultipleComponent]
    public class PlayerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Player());
            }
        }
    }
}