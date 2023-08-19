using MarioLand.Common.Players;
using MarioLand.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace MarioLand.Content.Items.PowerUps;
[AttributeUsage(AttributeTargets.Class)]
public class PowerUpTailSwipeData : PowerUpData
{
    public readonly int flightTimeDivider;

    public PowerUpTailSwipeData(int width, int height, MarioLand.PowerUp powerUp, int flightTimeDivider) : base(width, height, powerUp)
    {
        this.flightTimeDivider = flightTimeDivider;
    }
}

public abstract class PowerUpTailSwipe : PowerUpItem
{
    private PowerUpTailSwipeData Data => ((PowerUpTailSwipeData)GetType().GetCustomAttributes(typeof(PowerUpTailSwipeData), false)[0]);

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = Data.width;
        Item.height = Data.height;
    }

    bool Swiping = false;
    int SwipeTimer = 0;
    int StartingDirection;
    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.CurrentPowerUp = Data.powerUp;

        if (modPlayer.IsGrounded && modPlayer.HasPSpeed && PlayerInput.Triggers.JustPressed.Jump) modPlayer.IsFlyingWithPSpeed = true;
        if (modPlayer.IsFlyingWithPSpeed && !modPlayer.HasPSpeed) modPlayer.IsFlyingWithPSpeed = false;

        if (modPlayer.IsFlyingWithPSpeed)
        {
            if (PlayerInput.Triggers.JustPressed.Jump)
            {
                modPlayer.PSpeedTimer = (int)MathHelper.Clamp(modPlayer.PSpeedTimer - Data.flightTimeDivider, 0, 60);

                if (!modPlayer.IsGrounded) modPlayer.Player.velocity.Y = -5f;
            }

            if (modPlayer.PSpeedTimer == 0) modPlayer.HasPSpeed = false;
        }
        else if (!modPlayer.IsGrounded && PlayerInput.Triggers.Current.Jump && player.velocity.Y > 0) player.velocity.Y = 2f;

        if (PlayerInput.Triggers.JustPressed.MouseLeft && !player.mouseInterface && Main.mouseItem.IsAir && player.HeldItem.IsAir && !Swiping && modPlayer.ConsecutiveJumpCount != 3 && !modPlayer.TanookiStatue && player.mount.Type == -1)
        {
            StartingDirection = player.direction;
            Swiping = true;

            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Bottom - new Vector2(i == 0 ? 14 : -14, 8), Vector2.Zero, ModContent.ProjectileType<TailSwipeProjectile>(), modPlayer.StatPowActual, 5f, player.whoAmI, i);
            }
        }

        if (Swiping)
        {
            SwipeTimer++;

            if (SwipeTimer == 1 || SwipeTimer == 11) modPlayer.ForceDirection = StartingDirection == 1 ? -1 : 1;
            if (SwipeTimer == 6 || SwipeTimer == 16) modPlayer.ForceDirection = StartingDirection == 1 ? 1 : -1;

            if (SwipeTimer >= 20)
            {
                Swiping = false;
                SwipeTimer = 0;
                modPlayer.ForceDirection = 0;
            }
        }
    }
}

[PowerUpTailSwipeData(30, 32, MarioLand.PowerUp.SuperLeaf, 5)]
public class SuperLeaf : PowerUpTailSwipe { }

[PowerUpTailSwipeData(28, 32, MarioLand.PowerUp.TanookiSuit, 2)]
public class TanookiSuit : PowerUpTailSwipe
{
    public override void UpdateEquip(Player player)
    {
        base.UpdateEquip(player);

        // Statue form stuff
    }
}