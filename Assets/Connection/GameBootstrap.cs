using Unity.NetCode;

namespace Connection
{
    [UnityEngine.Scripting.Preserve]
    public class GameBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            AutoConnectPort = 7777;
            return base.Initialize(defaultWorldName);
        }
    }
}
