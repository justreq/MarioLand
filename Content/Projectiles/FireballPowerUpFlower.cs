using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand.Content.Projectiles;
public class FireballPowerUpFlower : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 150;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        string texture = $"{string.Join("/", base.Texture.Split("/").SkipLast(1))}/{(Projectile.ai[0] == 0 ? "Fire" : "Ice")}ballPowerUpFlower";
        DrawData data = new(ModContent.Request<Texture2D>(texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, 0.75f, Math.Sign(Projectile.velocity.X) == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

        Main.EntitySpriteDraw(data);

        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f) Projectile.velocity.Y = -5f;
        return false;
    }

    public override void AI()
    {
        Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
        Projectile.rotation += Math.Sign(Projectile.velocity.X) * 0.35f;

        if (Main.rand.NextBool(3))
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, Projectile.ai[0] == 0 ? DustID.Torch : DustID.BlueTorch, Scale: 2f);
            dust.noGravity = true;
            dust.velocity *= 4f;
        }

        if (Projectile.velocity.X == 0f) Projectile.Kill();
    }

    public override void Kill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, Projectile.ai[0] == 0 ? DustID.Torch : DustID.BlueTorch, Scale: 3f);
            dust.noGravity = true;
            dust.velocity *= 4f;
            Dust dust2 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Smoke, 0f, 0f);
            dust2.noGravity = true;
            dust2.velocity *= 4f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(10)) target.AddBuff(Projectile.ai[0] == 0 ? BuffID.OnFire : BuffID.Frostburn, 180);
    }
}
