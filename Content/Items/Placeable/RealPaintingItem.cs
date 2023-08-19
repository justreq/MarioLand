using MarioLand.Content.Tiles;
using Terraria.ModLoader;

namespace MarioLand.Content.Items.Placeable;
public abstract class RealPaintingItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
    }
}

public class RealMarioPaintingItem : RealPaintingItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<RealMarioPainting>());
        base.SetDefaults();
    }
}

public class RealLuigiPaintingItem : RealPaintingItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<RealLuigiPainting>());
        base.SetDefaults();
    }
}