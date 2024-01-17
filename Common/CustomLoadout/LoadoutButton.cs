using MarioLand.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace MarioLand.Common.CustomLoadout;
internal class ExLoadoutButtons : ILoadable
{
    internal static Asset<Texture2D> LoadoutButtonsTexture = null;

    void ILoadable.Load(Mod mod)
    {
        if (!Main.dedServ)
        {
            Main.QueueMainThreadAction(() => LoadoutButtonsTexture = mod.Assets.Request<Texture2D>("Common/CustomLoadout/LoadoutButton"));
        }
    }

    internal static void DrawLoadoutButtons(int inventoryTop, bool demonHeartSlotAvailable, bool masterModeSlotAvailable)
    {
        Player player = Main.LocalPlayer;
        LoadoutPlayer modPlayer = player.GetModPlayer<LoadoutPlayer>();

        int locationScale = 10
            - (demonHeartSlotAvailable ? 1 : 0)
            - (masterModeSlotAvailable ? 1 : 0);

        // Don't ask me about this math. It's shamfully ripped off from Vanilla.
        int x = Main.screenWidth - 58 + 14;
        int y = inventoryTop;
        int w = 4;
        int h = (int)((float)(inventoryTop - 2f) + (float)(locationScale * 56f) * Main.inventoryScale);
        Rectangle rectangle = new(x, y, w, h);

        int buttonHeight = 32;
        int buttonSpacing = 4;

        Rectangle[] buttonHitboxes = new Rectangle[MarioLand.EXTRALOADOUTS];

        for (int i = 0; i < MarioLand.EXTRALOADOUTS; i++)
        {
            buttonHitboxes[i] = new(rectangle.X + rectangle.Width, rectangle.Y + (buttonHeight + buttonSpacing) * (MarioLand.VANILLALOADOUTS + i), 32, 32);

            bool hovered = false;
            if (buttonHitboxes[i].Contains(Main.MouseScreen.ToPoint()))
            {
                hovered = true;
                player.mouseInterface = true;

                if (!Main.mouseText)
                {
                    string text = Language.GetTextValue("Mods.MarioLand.GUI.CustomLoadout");
                    Main.instance.MouseText(text);
                }

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    if (modPlayer.CurrentCustomLoadoutIndex < 0)
                    {
                        // We're on a vanilla layout currently
                        modPlayer.TrySwitchingVanillaToCustom(i);
                    }
                    else
                    {
                        // We're already on a modded layout
                        modPlayer.TrySwitchingCustomToCustom(i);
                    }
                }
            }

            int frameX = i == modPlayer.CurrentCustomLoadoutIndex ? 1 : 0;
            Rectangle frame = LoadoutButtonsTexture.Frame(3, 1, frameX, i);

            Vector2 pos = buttonHitboxes[i].Center.ToVector2();
            Vector2 origin = frame.Size() / 2f;

            // Draw main button
            Main.spriteBatch.Draw(LoadoutButtonsTexture.Value, pos, frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);

            // Draw outline if hovered
            if (hovered)
            {
                frame = LoadoutButtonsTexture.Frame(3, 1, 2, i);
                Main.spriteBatch.Draw(LoadoutButtonsTexture.Value, pos, frame, Main.OurFavoriteColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    void ILoadable.Unload()
    {
        Main.QueueMainThreadAction(() => LoadoutButtonsTexture?.Dispose());
    }
}