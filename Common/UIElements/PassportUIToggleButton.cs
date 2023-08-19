using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Common.UIElements;
internal class PassportUIToggleButton : UIHoverImageButton
{
    public PassportUIToggleButton() : base($"{nameof(MarioLand)}/Assets/Textures/UI/PassportUIToggleClosed", "Show Passport")
    {
        SetVisibility(1f, 1f);
        SetHoverImage(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/PassportUIToggleClosedHover", ReLogic.Content.AssetRequestMode.ImmediateLoad));
    }

    public void SetState(bool opened)
    {
        SetImage(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/PassportUIToggle{(opened ? "Opened" : "Closed")}", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        SetHoverImage(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/PassportUIToggle{(opened ? "Opened" : "Closed")}Hover", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        SetHoverText($"{(opened ? "Hide" : "Show")} Passport");
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (IsMouseHovering) Main.LocalPlayer.mouseInterface = true;
    }
}
