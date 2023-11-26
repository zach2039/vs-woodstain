using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using WoodStain.Network;

namespace WoodStain
{
    public class WoodStainModSystem : ModSystem
    {
        private IServerNetworkChannel serverChannel;
        private ICoreAPI api;

        public override void StartPre(ICoreAPI api)
        {
            string cfgFileName = "WoodStain.json";

            try 
            {
                WoodStainConfig cfgFromDisk;
                if ((cfgFromDisk = api.LoadModConfig<WoodStainConfig>(cfgFileName)) == null)
                {
                    api.StoreModConfig(WoodStainConfig.Loaded, cfgFileName);
                }
                else
                {
                    WoodStainConfig.Loaded = cfgFromDisk;
                }
            } 
            catch 
            {
                api.StoreModConfig(WoodStainConfig.Loaded, cfgFileName);
            }

            base.StartPre(api);
        }

        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);

            api.RegisterItemClass("ItemStainBrush", typeof(ItemStainBrush));

            api.Logger.Notification("Loaded Wood Stain!");
        }

        private void OnPlayerJoin(IServerPlayer player)
        {
            // Send connecting players config settings
            this.serverChannel.SendPacket(
                new SyncConfigClientPacket {
                    TongsUsageConsumesDurability = WoodStainConfig.Loaded.TongsUsageConsumesDurability
                }, player);
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            sapi.Event.PlayerJoin += this.OnPlayerJoin; 
            
            // Create server channel for config data sync
            this.serverChannel = sapi.Network.RegisterChannel("woodstain")
                .RegisterMessageType<SyncConfigClientPacket>()
                .SetMessageHandler<SyncConfigClientPacket>((player, packet) => {});
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            // Sync config settings with clients
            capi.Network.RegisterChannel("woodstain")
                .RegisterMessageType<SyncConfigClientPacket>()
                .SetMessageHandler<SyncConfigClientPacket>(p => {
                    this.Mod.Logger.Event("Received config settings from server");
                    WoodStainConfig.Loaded.TongsUsageConsumesDurability = p.TongsUsageConsumesDurability;
                });
        }
    }
}
