using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct PlayerSpawner : IComponentData
    {
        public Entity Player;
    }

    [DisallowMultipleComponent]
    public class PlayerSpawnerAuthoring : MonoBehaviour
    {
        public GameObject player;
        private class Baker : Baker<PlayerSpawnerAuthoring>
        {
            public override void Bake(PlayerSpawnerAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PlayerSpawner
                {
                    Player = GetEntity(authoring.player, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}