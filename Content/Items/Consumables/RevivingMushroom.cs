using Terraria.ModLoader;
using Terraria;

namespace MarioLand.Content.Items.Consumables;
public abstract class RevivingMushroom : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 30;
    }
}

public class OneUpMushroom : RevivingMushroom { }

public class OneUpDeluxe : RevivingMushroom { }