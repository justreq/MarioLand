using MarioLand.Common.Players;
using Terraria;

namespace MarioLand.Content.Items.Transformations;
public class MariosCap : TransformationItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        MarioLand.SetupEquipTextures("Mario");
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 32;
        Item.height = 20;
    }

    public override void UpdateEquip(Player player)
    {
        player.GetModPlayer<MarioLandPlayer>().CurrentTransformation = MarioLand.Transformation.Mario;
        base.UpdateEquip(player);
    }
}
