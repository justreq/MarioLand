using Terraria;
using MarioLand.Common.Players;
using System.Collections.Generic;
using Terraria.ID;

namespace MarioLand.Content.Items.Overalls;
public class AOKWear : OverallsItem
{
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.StatHPMaxBonus = 30;
        modPlayer.StatSPMaxBonus = 10;
        modPlayer.StatPowBonus = 20;
        modPlayer.StatDefBonus = 150;

        new List<int> { BuffID.Bleeding, BuffID.Poisoned, BuffID.OnFire, BuffID.Venom, BuffID.Darkness, BuffID.Blackout, BuffID.Silenced, BuffID.Cursed, BuffID.Confused, BuffID.Slow, BuffID.OgreSpit, BuffID.Weak, BuffID.BrokenArmor, BuffID.WitheredArmor, BuffID.WitheredWeapon, BuffID.Horrified, BuffID.CursedInferno, BuffID.Ichor, BuffID.Frostburn, BuffID.Chilled, BuffID.Frozen, BuffID.Webbed, BuffID.Stoned, BuffID.VortexDebuff, BuffID.Obstructed, BuffID.Electrified, BuffID.Rabies, BuffID.MoonLeech }.ForEach(e => player.buffImmune[e] = true);
    }
}