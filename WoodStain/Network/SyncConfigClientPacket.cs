using ProtoBuf;

namespace WoodStain.Network
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SyncConfigClientPacket
    {
        public bool TongsUsageConsumesDurability;
    }
}