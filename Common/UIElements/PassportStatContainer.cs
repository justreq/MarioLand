using MarioLand.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace MarioLand.Common.UIElements;
internal class PassportStatContainer : UIHoverPanel
{
    string textContent;
    float hAlign;
    float topPixels;
    public PassportStatContainer(ref UIText text, string textContent, float hAlign, float topPixels) : base(textContent)
    {
        this.textContent = textContent;
        this.hAlign = hAlign;
        this.topPixels = topPixels;

        Width = StyleDimension.FromPercent(0.45f);
        Height = StyleDimension.FromPixels(44f);
        HAlign = hAlign;
        Top = StyleDimension.FromPixels(topPixels);
        BackgroundColor = Color.DarkRed;
        BorderColor = Color.DarkRed;
        SetPadding(8f);

        this.AddElement(new UIImage(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/Stat{textContent}", ReLogic.Content.AssetRequestMode.ImmediateLoad)).With(e =>
        {
            e.Top = StyleDimension.FromPixels(-2f);
        }));

        text = this.AddElement(new UIText("", 1.2f).With(e =>
        {
            e.Left = StyleDimension.FromPixels(-4f);
            e.HAlign = 1f;
            e.VAlign = 0.5f;
        }));
    }
}
