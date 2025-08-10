using MarioLand.Common.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

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
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Color color = Color.White * 0.75f;

        Main.EntitySpriteDraw(texture, Projectile.TopLeft - Main.screenPosition + Projectile.Size * 0.5f, new Rectangle(0, 0, 4, 4), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.TopRight - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(4f, 0f), new Rectangle(18, 0, 4, 4), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.BottomLeft - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(0f, 4f), new Rectangle(0, 18, 4, 4), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.BottomRight - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(4f), new Rectangle(18, 18, 4, 4), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Vector2 sourcePosition = Projectile.TopLeft - Main.screenPosition + new Vector2(0f, 4f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, 4, Projectile.height - 8), new Rectangle(0, 4, 4, 14), color));

        sourcePosition = Projectile.TopLeft - Main.screenPosition + new Vector2(4f, 0f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, Projectile.width - 8, Projectile.height - 4), new Rectangle(4, 0, 14, 18), color));

        sourcePosition = Projectile.TopRight - Main.screenPosition - new Vector2(4f, -4f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, 4, Projectile.height - 8), new Rectangle(18, 4, 4, 14), color));

        sourcePosition = Projectile.BottomLeft - Main.screenPosition + new Vector2(4f, -4f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, Projectile.width - 8, 4), new Rectangle(4, 18, 14, 4), color));

        Main.EntitySpriteDraw(texture, Projectile.TopLeft - Main.screenPosition + Projectile.Size * 0.5f, new Rectangle(22, 0, 14, 12), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.BottomRight - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(8f), new Rectangle(36, 14, 8, 8), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)(Projectile.TopLeft.X - Main.screenPosition.X) + 4, (int)(Projectile.TopLeft.Y - Main.screenPosition.Y) + 4, Projectile.width - 8, (int)(Projectile.height * 0.67f)), new Rectangle(44, 0, 22, 22), color * 0.35f));

        return false;
    }

    public override void AI()
    {
        base.AI();
        if (npc == null) return;

        npc.Center = Projectile.Center;
    }

    public override void OnKill(int timeLeft)
    {
        if (npc != null)
        {
            npc.GetGlobalNPC<MarioLandGlobalNPC>().isFrozen = false;
            npc = null;
        }

        base.OnKill(timeLeft);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (oldVelocity.X != 0 && Projectile.velocity.X == 0)
        {
            SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/IceBlockBreak") { Volume = 0.75f });
            return true;
        }
        return false;
    }

    public override void Throw()
    {
        base.Throw();
        Projectile.velocity.X = 10 * throwDirection;
    }

    public override void ThrowUpdate()
    {
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 7.5f * throwDirection, 0.15f);
        Projectile.velocity.Y += 0.8f;
    }
}
