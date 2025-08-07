using MarioLand.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Content.Buffs;
public class SuperStarBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<MarioLandPlayer>().SuperStar = true;
        player.maxRunSpeed *= 1.5f;
        player.jumpSpeedBoost += 1.5f;
    }
}
