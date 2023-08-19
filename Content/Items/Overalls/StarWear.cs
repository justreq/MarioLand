using Terraria;
using MarioLand.Common.Players;

namespace MarioLand.Content.Items.Overalls;
public class StarWear : OverallsItem
{
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.StatHPMaxBonus = 20;
        modPlayer.StatSPMaxBonus = 10;
        modPlayer.StatDefBonus = 120;
    }
}