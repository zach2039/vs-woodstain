using ProtoBuf;

namespace WoodStain.ModNetwork
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SyncConfigClientPacket
    {
        public bool DummySettingBool;
    }
}