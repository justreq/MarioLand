using Terraria;
using MarioLand.Common.Players;

namespace MarioLand.Content.Items.Overalls;
public class BrawnyWear : OverallsItem
{
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.StatPowBonus = 10;
        modPlayer.StatDefBonus = 45;
    }
}