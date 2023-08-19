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

            if (Main.rand.NextBool(100)) itemType = ModContent.ItemType<OneUpDeluxe>();
            else if (Main.rand.NextBool(50)) itemType = ModContent.ItemType<OneUpMushroom>();
            else if (Main.rand.NextBool(20)) itemType = ModContent.ItemType<MaxMushroom>();
            else if (Main.rand.Next(20) < 3) itemType = ModContent.ItemType<UltraMushroom>();
            else if (Main.rand.NextBool(20)) itemType = ModContent.ItemType<PoisonMushroom>();
            else if (Main.rand.Next(10) < 3) itemType = ModContent.ItemType<SuperMushroom>();
            else itemType = ItemID.Mushroom;

            Item item = new(itemType);

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, item.width, item.height, item);
        }

        base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
    }
}
