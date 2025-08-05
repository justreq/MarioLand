using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System.Linq;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using MarioLand.Common.Players;
using System.Collections.Generic;

namespace MarioLand.Content.Items.Consumables;
public abstract class EdibleMushroom : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [
                new(234, 51, 34),
                new(255, 188, 140),
                new(5, 5, 5)
            ];
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 30;
        Item.UseSound = SoundID.Item2;
        Item.useStyle = ItemUseStyleID.EatFood;
        Item.useTurn = true;
        Item.useTime = 17;
        Item.useAnimation = 17;
        Item.consumable = true;
    }
}

public class SuperMushroom : EdibleMushroom
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 30;
    }
}

public class UltraMushroom : EdibleMushroom
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 60;
    }
}

public class MaxMushroom : EdibleMushroom
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.healLife = 120;
    }
}

public class PoisonMushroom : EdibleMushroom
{
    public override string Texture => string.Join("/", base.Texture.Split("/").Take(base.Texture.Split("/").Length - 1).ToList()) + "/PoisonMushroom";

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

        ItemID.Sets.FoodParticleColors[Item.type] = [
                new(147, 63, 164),
                new(251, 195, 230),
                new(5, 5, 5)
            ];
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.buffType = BuffID.Poisoned;
        Item.buffTime = 300;
    }

    public override void OnConsumeItem(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();
        if (modPlayer.DoTransformationEffects) modPlayer.JustConsumedBadItem = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.RemoveAt(tooltips.FindIndex(e => e.Name == "BuffTime"));
    }
}