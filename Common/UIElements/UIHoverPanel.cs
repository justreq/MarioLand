using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace MarioLand.Common.UIElements;
internal class UIHoverPanel : UIPanel
{
    string hoverText;
    public UIHoverPanel(string hoverText) : base()
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
