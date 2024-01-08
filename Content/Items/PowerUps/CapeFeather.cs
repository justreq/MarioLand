using MarioLand.Common.Players;
using Microsoft.Xna.Framework;
using System;
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

    int initialAscentTimer = 0;
    int timeSinceBobbedUp = 0;
    Vector2 previousVelocity = Vector2.Zero;

    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        if (player.mount.Type != MountID.None) return;

        modPlayer.CurrentPowerUp = MarioLand.PowerUp.CapeFeather;

        if (modPlayer.IsGrounded && modPlayer.PSpeedTimer == 60 && PlayerInput.Triggers.JustPressed.Jump) modPlayer.IsFlyingWithPSpeed = true;
        if (modPlayer.IsFlyingWithPSpeed && PlayerInput.Triggers.JustReleased.Jump)
        {
            modPlayer.PSpeedTimer = 0;
            modPlayer.IsFlyingWithPSpeed = false;
        }

        if (modPlayer.IsFlyingWithPSpeed)
        {
            if (initialAscentTimer < 120)
            {
                initialAscentTimer++;
                player.velocity.Y = -10f;
            }
            else
            {
                modPlayer.ForceDirection = player.direction;

                if (modPlayer.ForceDirection == -1 ? PlayerInput.Triggers.Current.Left : PlayerInput.Triggers.Current.Right)
                {
                    timeSinceBobbedUp = 0;
                    player.velocity.X = MathHelper.Lerp(player.velocity.X, 10f * modPlayer.ForceDirection, 0.1f);
                    player.velocity.Y = MathHelper.Lerp(player.velocity.Y, 10f, 0.1f);
                    previousVelocity = player.velocity;
                }

                if (modPlayer.ForceDirection == -1 ? PlayerInput.Triggers.JustPressed.Right : PlayerInput.Triggers.JustPressed.Left) player.velocity.Y = -previousVelocity.Y * 1.5f;

                if (modPlayer.ForceDirection == -1 ? PlayerInput.Triggers.Current.Right : PlayerInput.Triggers.Current.Left)
                {
                    timeSinceBobbedUp++;

                    if (timeSinceBobbedUp > 60) player.velocity.X = MathHelper.Lerp(player.velocity.X, 0.1f * modPlayer.ForceDirection, 0.01f);
                    else player.velocity.X = previousVelocity.X;
                    player.velocity.Y = MathHelper.Lerp(player.velocity.Y, 10f, 0.001f);
                }
            }
        }
        else
        {
            if (PlayerInput.Triggers.Current.Jump && !modPlayer.IsGrounded && player.velocity.Y > 0f) player.velocity.Y = 2f;
        }

        if (modPlayer.IsGrounded)
        {
            modPlayer.ForceDirection = 0;
            initialAscentTimer = 0;
        }
    }
}
