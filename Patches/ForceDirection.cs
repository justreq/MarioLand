using MarioLand.Common.Players;
using Terraria.ModLoader;
using Terraria;

namespace MarioLand.Patches;
internal sealed class ForceDirection : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_Player.HorizontalMovement += On_Player_HorizontalMovement;
        On_Player.ChangeDir += On_Player_ChangeDir;
    }

    private void On_Player_HorizontalMovement(On_Player.orig_HorizontalMovement orig, Player self)
    {
        orig(self);

        MarioLandPlayer modPlayer = self.GetModPlayer<MarioLandPlayer>();

        if (modPlayer.ForceDirection != 0) self.direction = modPlayer.ForceDirection;
    }

    private void On_Player_ChangeDir(On_Player.orig_ChangeDir orig, Player self, int dir)
    {
        orig(self, dir);

        MarioLandPlayer modPlayer = self.GetModPlayer<MarioLandPlayer>();

        if (modPlayer.ForceDirection != 0) self.direction = modPlayer.ForceDirection;
    }
}