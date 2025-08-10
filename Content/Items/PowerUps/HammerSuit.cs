using MarioLand.Common.Players;
using MarioLand.Content.Projectiles;
using System;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace MarioLand.Content.Items.PowerUps;
public class HammerSuit : PowerUpItem
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

        modPlayer.CurrentPowerUp = MarioLand.PowerUp.HammerSuit;

        if (PlayerInput.Triggers.JustPressed.MouseRight && !player.mouseInterface && !modPlayer.JustSummonedProjectile && modPlayer.GrabProjectile == null)
        {
            modPlayer.ForceDirection = Math.Sign(Main.MouseWorld.X - player.position.X);

            Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, new Vector2((Main.MouseWorld.X - player.Center.X) / 50, -8f), ModContent.ProjectileType<HammerSuitHammer>(), modPlayer.StatPowActual, 0f, player.whoAmI);

            modPlayer.JustSummonedProjectile = true;
        }
    }
}