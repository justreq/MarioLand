using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace MarioLand.Content.Projectiles;
public class IceBlockProjectile : ModProjectile
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

        Vector2 sourcePosition = Projectile.TopLeft - Main.screenPosition + new Vector2(10f, 0f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, Projectile.width - 20, Projectile.height), new Rectangle(10, 10, 2, 2), color));

        sourcePosition = Projectile.TopLeft - Main.screenPosition + new Vector2(0f, 10f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, 10, Projectile.height - 20), new Rectangle(10, 10, 2, 2), color));

        sourcePosition = Projectile.TopRight - Main.screenPosition - new Vector2(10f, -10f);
        Main.EntitySpriteDraw(new DrawData(texture, new Rectangle((int)sourcePosition.X, (int)sourcePosition.Y, 10, Projectile.height - 20), new Rectangle(10, 10, 2, 2), color));

        Main.EntitySpriteDraw(texture, Projectile.TopLeft - Main.screenPosition + Projectile.Size * 0.5f, new Rectangle(0, 0, 10, 10), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.TopRight - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(10f, 0f), new Rectangle(12, 0, 10, 10), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.BottomLeft - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(0f, 10f), new Rectangle(0, 12, 10, 10), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.BottomRight - Main.screenPosition + Projectile.Size * 0.5f - new Vector2(10f), new Rectangle(12, 12, 10, 10), color, Projectile.rotation, Projectile.Size * 0.5f, Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void AI()
    {
        if (npc == null) return;

        npc.noGravity = true;
        npc.velocity = Vector2.Zero;
        npc.Center = Projectile.Center;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        npc.immortal = false;
    }
}
