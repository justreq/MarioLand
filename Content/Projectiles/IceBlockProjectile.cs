using MarioLand.Common.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand.Content.Projectiles;
public class IceBlockProjectile : GrabbableProjectile
{
    public NPC npc;

    public override void SetDefaults()
    {
        Projectile.penetrate = -1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if (npc == null) return;
        npc.Center = Projectile.Center;

        if (Projectile.timeLeft % 100 == 0)
        {
            SoundEngine.PlaySound(SoundID.Item50);
            SpawnGore(false, 0.5f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        if (npc != null)
        {
            npc.GetGlobalNPC<MarioLandGlobalNPC>().iceBlock = null;
            npc = null;
        }

        SoundEngine.PlaySound(SoundID.Item27);
        SpawnGore(Projectile.velocity != Vector2.Zero, velocityMultiplier: 3);

        base.OnKill(timeLeft);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return oldVelocity.X != 0 && Projectile.velocity.X == 0;
    }

    public override void Throw()
    {
        base.Throw();
        Projectile.velocity.X = 10 * throwDirection;
        Projectile.friendly = true;
        Projectile.damage = 10;
        Projectile.knockBack = 5f;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (target == npc) return false;

        return base.CanHitNPC(target);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (target.TryGetGlobalNPC<MarioLandGlobalNPC>(out var globalNPC) && globalNPC.IsFrozen)
        {
            globalNPC.iceBlock.OnKill(0);
        }

        base.OnHitNPC(target, hit, damageDone);
    }

    public override void ThrowUpdate()
    {
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 7.5f * throwDirection, 0.15f);
        Projectile.velocity.Y += 0.8f;
    }

    public void SpawnGore(bool matchVelocity = true, float scale = 1f, float velocityMultiplier = 1)
    {
        for (int i = 0; i < 4; i++)
        {
            int gore = Mod.Find<ModGore>($"{Name}Gore_{i + 1}").Type;
            gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, matchVelocity ? Projectile.velocity : MathHelper.ToRadians(0 - i * 60).ToRotationVector2() * velocityMultiplier, gore, scale);
            Main.gore[gore].timeLeft = 0;
        }
    }
}
