using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using MarioLand.Content.Buffs;

namespace MarioLand.Content.Items.Consumables;
public class SuperStar : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;

        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.IsFood[Type] = true;
        ItemID.Sets.FoodParticleColors[Item.type] = [
                new(255, 254, 140),
                new(253, 208, 26),
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
        Item.buffType = ModContent.BuffType<SuperStarBuff>();
        Item.buffTime = 1500;
    }
}