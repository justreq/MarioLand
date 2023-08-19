﻿using Terraria;
using MarioLand.Common.Players;

namespace MarioLand.Content.Items.Overalls;
public class GrownUpWear : OverallsItem
{
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.StatDefBonus = 50;
    }
}