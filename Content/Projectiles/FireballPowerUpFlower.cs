using MarioLand.Common.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    int tileCollideCount = 0;

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
        {
            Projectile.velocity.Y = Projectile.ai[0] == 0 ? -5f : -4f;
            tileCollideCount++;
        }

        return Projectile.ai[0] != 0 && tileCollideCount == 2;
    }

    public override void AI()
    {
        Projectile.velocity.Y += (Projectile.ai[0] == 0 ? 0.4f : 0.2f);
        Projectile.rotation += Math.Sign(Projectile.velocity.X) * 0.35f;

        if (Main.rand.NextBool(Projectile.ai[0] == 0 ? 3 : 2))
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, Projectile.ai[0] == 0 ? DustID.Torch : DustID.BlueTorch, Scale: 2f);
            dust.noGravity = true;
            dust.velocity *= 4f;
        }

        if (Projectile.velocity.X == 0f) Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[0] == 1) SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/IceballBreak") { Volume = 0.5f });

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
        if (Projectile.ai[0] == 0)
        {
            if (Main.rand.NextBool(10)) target.AddBuff(BuffID.OnFire, 180);
        }
        else if (target.life > 0)
        {
            SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Freeze"));
            IceBlockProjectile iceBlock = (IceBlockProjectile)Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<IceBlockProjectile>(), 0, 0f, Projectile.owner)].ModProjectile;
            target.GetGlobalNPC<MarioLandGlobalNPC>().iceBlock = iceBlock;
            iceBlock.npc = target;
            iceBlock.Projectile.timeLeft = (target.lifeMax / (target.lifeMax - hit.Damage)) * 480;
            iceBlock.Projectile.width = target.frame.Width;
            iceBlock.Projectile.height = target.frame.Height;
            iceBlock.Projectile.Bottom = target.Bottom;
        }
    }
}
