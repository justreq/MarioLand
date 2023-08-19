using Terraria;
using MarioLand.Common.Players;

namespace MarioLand.Content.Items.Overalls;
public class LeisureWear : OverallsItem
{
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.StatDefBonus = 30;
    }
}