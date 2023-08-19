using MarioLand.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;

namespace MarioLand.Content.Items.PowerUps;
public class FrogSuit : PowerUpItem
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
        Vector2 inputVector = Vector2.Zero;

        modPlayer.CurrentPowerUp = MarioLand.PowerUp.FrogSuit;

        if (player.wet)
        {
            player.fullRotationOrigin = player.Hitbox.Size() / 2;
            player.gravity = 0f;
            player.accDivingHelm = true;
            player.controlJump = false;

            if (PlayerInput.Triggers.Current.Left) inputVector.X = -1f;
            else if (PlayerInput.Triggers.Current.Right) inputVector.X = 1f;
            else inputVector.X = 0f;

            if (PlayerInput.Triggers.Current.Up) inputVector.Y = -1f;
            else if (PlayerInput.Triggers.Current.Down) inputVector.Y = 1f;
            else inputVector.Y = 0f;

            if (inputVector != Vector2.Zero && player.velocity != Vector2.Zero) player.fullRotation = Utils.AngleLerp(player.fullRotation, player.velocity.ToRotation() + MathHelper.PiOver2, 0.1f);
            player.velocity = inputVector * 5f;
        }
        else
        {
            player.fullRotation = modPlayer.ConsecutiveJumpCount != 3 ? 0f : player.fullRotation;

            if (modPlayer.IsGrounded)
            {
                player.runSlowdown += 2f;
                player.maxRunSpeed -= 0.25f;
            }

            player.jumpSpeedBoost += 1f;
        }
    }
}
