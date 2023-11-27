using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

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
						itemstack.Attributes.SetInt("durability", itemstack.Collectible.GetMaxDurability(itemstack));
					}
					world.PlaySoundAt(new AssetLocation("sounds/effect/squish2"), player, null, true, 32f, 1f);
				}
				else
				{
					world.PlaySoundAt(new AssetLocation("sounds/effect/squish2"), byEntity.SidedPos.X, byEntity.SidedPos.Y, byEntity.SidedPos.Z, null, 1f, 16f, 1f);
				}
			}
			itemslot.MarkDirty();
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
			// Try to apply stain, then try to get stain from bucket
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
			else if (this.tryGetStain(byEntity, blockSel, slot, -1))
			{
				EntityPlayer entityPlayer = byEntity as EntityPlayer;
				IPlayer player = (entityPlayer != null) ? entityPlayer.Player : null;
				
				if (player != null && this.api.Side.IsClient())
				{
					this.api.World.PlaySoundAt(new AssetLocation("sounds/effect/water-fill"), player, null, true, 32f, 1f);
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

			string state = this.Variant["state"];
			if (state == null || state.Contains("dry"))
			{
				// Dont allow use of dry brushes to stain
				return false;
			}

            AssetLocation newBlockCode;

			// Try to find valid block codes for stained variants
			if (block.Code.Path.Contains("plankstairs"))
			{
				newBlockCode = new AssetLocation("woodstain", block.Code.Path.Replace("plankstairs", "stainedplankstairs-" + color));
			}
			else if (block.Code.Path.Contains("plankslab"))
			{
				newBlockCode = new AssetLocation("woodstain", block.Code.Path.Replace("plankslab", "stainedplankslab-" + color));
			}
			else if (block.Code.Path.Contains("planks"))
			{
				newBlockCode = new AssetLocation("woodstain", block.Code.Path.Replace("planks", "stainedplanks-" + color));
			}
			else
			{
				// No valid replacement for found block
				return false;
			}
			
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

		private bool tryGetStain(EntityAgent byEntity, BlockSelection blockSel, ItemSlot itemslot, int dir)
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

			string state = this.Variant["state"];
			if (state == null)
			{
				return false;
			}

			

			// Try get liquid from target block, and perform different transformations on brush item depending on liquid
			if (block is BlockLiquidContainerBase liquidContainerBase)
			{
				ItemStack contentItemstack = liquidContainerBase.GetContent(blockSel.Position);

				if (contentItemstack != null)
				{
					WaterTightContainableProps props = liquidContainerBase.GetContentProps(blockSel.Position);
					if (contentItemstack.Collectible.Code.Path.ToString().Contains("dye-")) // container of dye
					{
						// Only allow redip on brush if no color
						if (color != "none")
						{
							return false;
						}

						string liquidcolor = contentItemstack.Collectible.Variant["color"];
						if (liquidcolor == null)
						{
							return false;
						}

						// woad is an alternative to blue dye
						if (liquidcolor == "woad")
						{
							liquidcolor = "blue";
						}

						if (liquidContainerBase.GetCurrentLitres(blockSel.Position) >= 1f)
						{
							if (liquidContainerBase.TryTakeContent(blockSel.Position, (int)(1 * props.ItemsPerLitre)) != null)
							{
								Item newItem = this.api.World.GetItem(base.CodeWithVariants(new string[]
								{
									"color",
									"state"
								}, new string[]
								{
									liquidcolor,
									"wet"
								}));
								itemslot.Itemstack = new ItemStack(newItem, 1);
								if (byPlayer != null)
								{
									if (itemslot.Itemstack != null && !itemslot.Itemstack.Attributes.HasAttribute("durability"))
									{
										itemslot.Itemstack.Attributes.SetInt("durability", itemslot.Itemstack.Collectible.GetMaxDurability(itemslot.Itemstack));
									}
									return true;
								}
							}
						}
					}
					else if (contentItemstack.Collectible.Code.Path.ToString() == "waterportion") // container of water
					{
						if (liquidContainerBase.GetCurrentLitres(blockSel.Position) >= 1f)
						{
							if (liquidContainerBase.TryTakeContent(blockSel.Position, (int)(1 * props.ItemsPerLitre)) != null)
							{
								Item newItem = this.api.World.GetItem(base.CodeWithVariants(new string[]
								{
									"color",
									"state"
								}, new string[]
								{
									"none",
									"wet"
								}));
								itemslot.Itemstack = new ItemStack(newItem, 1);
								if (byPlayer != null)
								{
									if (itemslot.Itemstack != null && !itemslot.Itemstack.Attributes.HasAttribute("durability"))
									{
										itemslot.Itemstack.Attributes.SetInt("durability", itemslot.Itemstack.Collectible.GetMaxDurability(itemslot.Itemstack));
									}
									return true;
								}
							}
						}
					}
				}
			}
			
			return false;
		}
    }
}