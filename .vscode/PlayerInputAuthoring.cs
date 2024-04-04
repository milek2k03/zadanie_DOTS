using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInput : IInputComponentData
    {
        public int Horizontal;
        public int Vertical;
    }

    [DisallowMultipleComponent]
    public class PlayerInputAuthoring : MonoBehaviour
    {
        private class Baking : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PlayerInput());
            }
        }
    }
}