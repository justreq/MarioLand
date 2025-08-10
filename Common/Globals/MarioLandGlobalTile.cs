using MarioLand.Content.Items.Consumables;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand.Common.Globals;
public class MarioLandGlobalTile : GlobalTile
{
    public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if ((type == TileID.Plants || type == TileID.HallowedPlants) && Main.tile[i, j].TileFrameX == 18 * 8)
        {
            noItem = true;
            int itemType;

            float rand = Main.rand.NextFloat();

            if (rand <= 0.01) itemType = ModContent.ItemType<OneUpDeluxe>();
            else if (rand <= 0.02) itemType = ModContent.ItemType<OneUpMushroom>();
            else if (rand <= 0.1) itemType = ModContent.ItemType<MaxMushroom>();
            else if (rand <= 0.15) itemType = ModContent.ItemType<UltraMushroom>();
            else if (rand <= 0.25) itemType = ModContent.ItemType<PoisonMushroom>();
            else if (rand <= 0.4) itemType = ModContent.ItemType<SuperMushroom>();
            else itemType = ItemID.Mushroom;

            Item item = new(itemType);

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, item.width, item.height, item);
        }

        base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
    }
}
