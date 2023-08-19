using Terraria;
using MarioLand.Common.Players;

namespace MarioLand.Content.Items.Overalls;
public class MasterWear : OverallsItem
{
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.StatSPMaxBonus = 20;
        modPlayer.StatPowBonus = (int)(modPlayer.StatPow * 1.25);
        modPlayer.StatDefBonus = 140;
    }
}