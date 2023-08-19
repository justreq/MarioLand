using MarioLand.Common.Globals;
using MarioLand.Content.Items.Consumables;
using MarioLand.Content.Items.PowerUps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand.Patches;
internal class ApplyPotionDelay : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_Player.ApplyPotionDelay += On_Player_ApplyPotionDelay;
    }

    private void On_Player_ApplyPotionDelay(On_Player.orig_ApplyPotionDelay orig, Player self, Item sItem)
    {
        if (!(new List<int>() { ItemID.Mushroom, ModContent.ItemType<SuperMushroom>(), ModContent.ItemType<UltraMushroom>(), ModContent.ItemType<MaxMushroom>(), ItemID.Mushroom }.Contains(sItem.type)))
            orig(self, sItem);
    }
}
