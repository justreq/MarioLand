using MarioLand.Common.Players;
using Terraria;

namespace MarioLand.Content.Items.Transformations;
public class LuigisCap : TransformationItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        // MarioLand.SetupEquipTextures("Luigi");
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 32;
        Item.height = 20;
    }

    public override void UpdateEquip(Player player)
    {
        // player.GetModPlayer<MarioLandPlayer>().CurrentTransformation = MarioLand.Transformation.Luigi;
        base.UpdateEquip(player);
    }
}
