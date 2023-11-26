using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace WoodStain.Items
{
    public class ItemStainBrush : Item
    {
        public override void DamageItem(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, int amount = 1)
        {
            // Change damage item to return dry plain brush on max damage
			ItemStack itemstack = itemslot.Itemstack;
			int leftDurability = itemstack.Collectible.GetRemainingDurability(itemstack);
			leftDurability -= amount;
			itemstack.Attributes.SetInt("durability", leftDurability);
			if (leftDurability <= 0)
			{
				Item newItem = this.api.World.GetItem(base.CodeWithVariants(new string[]
				{
					"color",
					"state"
				}, new string[]
				{
					"none",
					"dry"
				}));
				itemslot.Itemstack = new ItemStack(newItem, 1);
				EntityPlayer entityPlayer = byEntity as EntityPlayer;
				IPlayer player = (entityPlayer != null) ? entityPlayer.Player : null;
				if (player != null)
				{
					if (itemslot.Itemstack != null && !itemslot.Itemstack.Attributes.HasAttribute("durability"))
					{
						itemstack = itemslot.Itemstack;
						itemstack.Attributes.SetInt("durability", 0);
					}
					world.PlaySoundAt(new AssetLocation("sounds/effect/squish2"), player, null, true, 32f, 1f);
				}
				else
				{
					world.PlaySoundAt(new AssetLocation("sounds/effect/squish2"), byEntity.SidedPos.X, byEntity.SidedPos.Y, byEntity.SidedPos.Z, null, 1f, 16f, 1f);
				}
			}
        }

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
				IPlayer player = (entityPlayer != null) ? entityPlayer.Player : null;
				if (entityPlayer == null || entityPlayer.Player.WorldData.CurrentGameMode != EnumGameMode.Creative)
				{
					this.DamageItem(this.api.World, byEntity, slot, 1);
				}
				
				if (player != null && this.api.Side.IsClient())
				{
					this.api.World.PlaySoundAt(new AssetLocation("sounds/effect/squish1"), player, null, true, 32f, 1f);
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
			if (block == null)
			{
				return false;
			}

			string color = this.Variant["color"];
			if (color == null)
			{
				return false;
			}

            AssetLocation newBlockCode = new AssetLocation("woodstain", block.Code.Path.Replace("planks", "stainedplanks-" + color));
			if (newBlockCode != null)
			{
				Block newBlock = this.api.World.GetBlock(newBlockCode);
				if (newBlock != null)
				{
					this.api.World.PlaySoundAt(block.Sounds.Place, (double)((float)blockSel.Position.X + 0.5f), (double)((float)blockSel.Position.Y + 0.5f), (double)((float)blockSel.Position.Z + 0.5f), byPlayer, true, 32f, 1f);
					IClientWorldAccessor clientWorldAccessor = this.api.World as IClientWorldAccessor;
					if (clientWorldAccessor != null)
					{
						clientWorldAccessor.Player.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
					}

					this.api.World.BlockAccessor.ExchangeBlock(newBlock.Id, blockSel.Position);
					this.api.World.BlockAccessor.MarkBlockDirty(blockSel.Position, byPlayer);
					return true;
				}
			}
			
			return false;
		}
    }
}