using MarioLand.Common.Globals;
using MarioLand.Content.Items.PowerUps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Patches;
internal class DrawTiles : BasePatch
{
    internal override void Patch(Mod mod)
    {
        On_Main.DoDraw_Tiles_Solid += On_Main_DoDraw_Tiles_Solid;
    }

    private void On_Main_DoDraw_Tiles_Solid(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
    {
        Main.spriteBatch.Begin();

        for (int i = 0; i < Main.item.Where(e => e.type == ModContent.ItemType<PowerUpItem>()).Count(); i++)
        {
            Item item = Main.item[i];

            if (item.ModItem == null) continue;

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(item.ModItem.Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, item.Center.ToScreenPosition(), null, item.GetGlobalItem<MarioLandGlobalItem>().lightColor, 0f, item.Size / 2, Main.GameZoomTarget, SpriteEffects.None, 0f);
        }

        Main.spriteBatch.End();

        orig(self);
    }
}
