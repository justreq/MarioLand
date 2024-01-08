using MarioLand.Common.CustomLoadout;
using MarioLand.Common.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System;
using Terraria.ID;

namespace MarioLand.Content.Items.Overalls;
public abstract class OverallsItem : ModItem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return false;
    }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.value = 0;
        Item.accessory = true;
        Item.GetGlobalItem<MarioLandGlobalItem>().ItemContext = MarioLand.ItemContext.Overalls;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips[tooltips.FindIndex(e => e.Name == "Equipable")].Text += $" Overalls";
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand)
    {
        return !(pre == -1 || pre == -3);
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded)
    {
        return modded && slot == ModContent.GetInstance<SlotOveralls>().Type;
    }
}