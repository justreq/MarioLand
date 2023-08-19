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

    int glideTimer = 0;
    float oldXVelocity = 0f;
    int pullingForwardTimer = 0;
    int pullingBackTimer = 0;
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

        if (modPlayer.IsFlyingWithPSpeed)
        {
            if (PlayerInput.Triggers.Current.Jump && glideTimer < 110)
            {
                player.velocity.X += player.direction * 0.25f;
                player.velocity.Y = -5f;
                glideTimer++;
            }

            if (glideTimer == 110)
            {
                modPlayer.ForceDirection = player.direction;
                player.gravity = 0.1f;

                if (player.direction == 1)
                {
                    if (PlayerInput.Triggers.Current.Right)
                    {
                        pullingForwardTimer++;
                        player.velocity.X += pullingForwardTimer * 0.001f;
                    }

                    if (PlayerInput.Triggers.JustPressed.Left)
                    {
                        player.velocity.Y -= pullingForwardTimer / 10;
                        pullingForwardTimer = 0;
                    }

                    if (PlayerInput.Triggers.Current.Left) pullingBackTimer++;
                    else pullingBackTimer = 0;

                    if (player.velocity.X < oldXVelocity) player.velocity.X = oldXVelocity;

                    if (pullingBackTimer > 30 && player.velocity.X > 0f) player.velocity.X -= 0.1f;
                }
                else
                {
                    if (PlayerInput.Triggers.Current.Left)
                    {
                        pullingForwardTimer++;
                        player.velocity.X -= pullingForwardTimer * 0.001f;
                    }

                    if (PlayerInput.Triggers.JustPressed.Right)
                    {
                        player.velocity.Y -= pullingForwardTimer / 10;
                        pullingForwardTimer = 0;
                    }

                    if (PlayerInput.Triggers.Current.Right) pullingBackTimer++;
                    else pullingBackTimer = 0;

                    if (player.velocity.X > oldXVelocity) player.velocity.X = oldXVelocity;

                    if (pullingBackTimer > 30 && player.velocity.X < 0f) player.velocity.X += 0.1f;
                }

                oldXVelocity = player.velocity.X;
            }
            else
            {
                modPlayer.ForceDirection = 0;
                player.gravity = 1f;
            }
        }
        else
        {
            glideTimer = 0;

            if (PlayerInput.Triggers.Current.Jump && !modPlayer.IsGrounded && player.velocity.Y > 0f) player.velocity.Y = 2f;
        }

        if (modPlayer.IsGrounded) modPlayer.ForceDirection = 0;
    }
}
