using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace MarioLand.Common.UIElements;
internal class UIHoverElement : UIElement
{
    string hoverText;
    public UIHoverElement(string hoverText) : base()
    {
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
