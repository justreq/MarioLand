using MarioLand.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand.Content.Items.PowerUps;

public class CapeFeather : PowerUpItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 32;
        Item.height = 32;
    }

    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        if (player.mount.Type != MountID.None) return;

        modPlayer.CurrentPowerUp = MarioLand.PowerUp.CapeFeather;

        if (modPlayer.IsGrounded && modPlayer.PSpeedTimer == 60 && PlayerInput.Triggers.JustPressed.Jump) modPlayer.IsFlyingWithPSpeed = true;
        if (modPlayer.IsFlyingWithPSpeed && (PlayerInput.Triggers.JustReleased.Jump))
        {
            modPlayer.PSpeedTimer = 0;
            modPlayer.IsFlyingWithPSpeed = false;
        }

        if (!modPlayer.IsFlyingWithPSpeed)
        {
            if (PlayerInput.Triggers.Current.Jump && !modPlayer.IsGrounded && player.velocity.Y > 0f) player.velocity.Y = 2f;
        }

        if (modPlayer.IsGrounded) modPlayer.ForceDirection = 0;
    }
}
