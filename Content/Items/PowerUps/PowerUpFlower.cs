using MarioLand.Common.Players;
using MarioLand.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace MarioLand.Content.Items.PowerUps;
[AttributeUsage(AttributeTargets.Class)]
public class PowerUpFlowerData : PowerUpData
{
    public readonly string useSoundName;
    public readonly int projectileAI0;

    public PowerUpFlowerData(int width, int height, MarioLand.PowerUp powerUp, string useSoundName, int projectileAI0) : base(width, height, powerUp)
    {
        this.useSoundName = useSoundName;
        this.projectileAI0 = projectileAI0;
    }
}

public abstract class PowerUpFlower : PowerUpItem
{
    private PowerUpFlowerData Data => ((PowerUpFlowerData)GetType().GetCustomAttributes(typeof(PowerUpFlowerData), false)[0]);

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = Data.width;
        Item.height = Data.height;
    }

    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        modPlayer.CurrentPowerUp = Data.powerUp;

        if (PlayerInput.Triggers.JustPressed.MouseLeft && !player.mouseInterface && Main.mouseItem.IsAir && player.HeldItem.IsAir && !modPlayer.JustSummonedProjectile)
        {
            int direction = Math.Sign(Main.MouseWorld.X - player.position.X);

            modPlayer.ForceDirection = direction;

            SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/{Data.useSoundName}") { Volume = 0.5f });
            Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, new Vector2(5 * direction, 0f), ModContent.ProjectileType<FireballPowerUpFlower>(), modPlayer.StatPowActual, 0f, player.whoAmI, Data.projectileAI0);

            modPlayer.JustSummonedProjectile = true;
        }
    }
}

[PowerUpFlowerData(32, 32, MarioLand.PowerUp.FireFlower, "Fireball", 0)]
public class FireFlower : PowerUpFlower { }

[PowerUpFlowerData(32, 32, MarioLand.PowerUp.IceFlower, "Fireball", 1)]
public class IceFlower : PowerUpFlower { }