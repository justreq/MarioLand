using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Content.Projectiles;
public class TailSwipeProjectile : ModProjectile
{
    public override string Texture => $"{nameof(MarioLand)}/Assets/Textures/EmptyPixel";

    public override void SetDefaults()
    {
        Projectile.width = 28;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 20;
    }

    public override void AI()
    {
        Projectile.Center = Main.player[Projectile.owner].Bottom - new Vector2(-28 * Projectile.ai[0] + 14, 8);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.HitDirectionOverride = 2 * (int)Projectile.ai[0] - 1;
        base.ModifyHitNPC(target, ref modifiers);
    }
}
