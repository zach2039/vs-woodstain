using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace WoodStain
{
    public class ItemStainBrush : Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
		{
			base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
			if (handling == EnumHandHandling.PreventDefault)
			{
				return;
			}
			if (blockSel == null)
			{
				return;
			}
			if (this.stain(byEntity, blockSel, -1))
			{
				EntityPlayer entityPlayer = byEntity as EntityPlayer;
				if (entityPlayer == null || entityPlayer.Player.WorldData.CurrentGameMode != EnumGameMode.Creative)
				{
					this.DamageItem(this.api.World, byEntity, slot, 1);
				}
			}
			handling = EnumHandHandling.PreventDefault;
		}

        private bool stain(EntityAgent byEntity, BlockSelection blockSel, int dir)
		{
			EntityPlayer entityPlayer = byEntity as EntityPlayer;
			IPlayer byPlayer = (entityPlayer != null) ? entityPlayer.Player : null;
			if (byPlayer == null)
			{
				return false;
			}
			if (!byEntity.World.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
			{
				this.api.World.BlockAccessor.MarkBlockEntityDirty(blockSel.Position.AddCopy(blockSel.Face));
				this.api.World.BlockAccessor.MarkBlockDirty(blockSel.Position.AddCopy(blockSel.Face), (IPlayer)null);
				return false;
			}

			Block block = this.api.World.BlockAccessor.GetBlock(blockSel.Position);
			string color = this.Variant["color"];
            AssetLocation newBlockCode = block.Code.WildCardReplace(new AssetLocation("*-planks-*"), new AssetLocation("*-stainedplanks-" + color + "-*"));

			if (newBlockCode.Valid)
			{
				this.api.World.PlaySoundAt(block.Sounds.Place, (double)((float)blockSel.Position.X + 0.5f), (double)((float)blockSel.Position.Y + 0.5f), (double)((float)blockSel.Position.Z + 0.5f), byPlayer, true, 32f, 1f);
				IClientWorldAccessor clientWorldAccessor = this.api.World as IClientWorldAccessor;
				if (clientWorldAccessor != null)
				{
					clientWorldAccessor.Player.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
				}
				Block newBlock = this.api.World.BlockAccessor.GetBlock(newBlockCode);
				this.api.World.BlockAccessor.ExchangeBlock(newBlock.Id, blockSel.Position);
				this.api.World.BlockAccessor.MarkBlockDirty(blockSel.Position, byPlayer);
				return true;
			}
			
			return false;
		}
    }
}