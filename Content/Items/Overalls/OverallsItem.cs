using MarioLand.Common.CustomLoadout;
using MarioLand.Common.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System;

namespace MarioLand.Content.Items.Overalls;
public abstract class OverallsItem : ModItem
{
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
        TooltipLine itemName = tooltips[tooltips.FindIndex(e => e.Name == "ItemName")];

        itemName.Text += $" [i:{nameof(MarioLand)}/{nameof(Overalls)}] {nameof(Overalls)}";
        itemName.OverrideColor = Main.DiscoColor;
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

public class Overalls : ModItem { }