using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace MarioLand.Common.UIElements;
internal class UIHoverImageButton : UIImageButton
{
    string texture;
    string hoverText;
    public UIHoverImageButton(string texture, string hoverText) : base(ModContent.Request<Texture2D>(texture, ReLogic.Content.AssetRequestMode.ImmediateLoad))
    {
        this.texture = texture;
        this.hoverText = hoverText;
    }

    public void SetHoverText(string hoverText)
    {
        this.hoverText = hoverText;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (IsMouseHovering) Main.hoverItemName = hoverText;
    }
}
