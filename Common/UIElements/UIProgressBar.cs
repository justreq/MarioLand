using MarioLand.Utilities.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace MarioLand.Common.UIElements;
internal class UIProgressBar : UIHoverElement
{
    float current;
    float max;
    string title;

    UIElement ProgressContainer { get; set; }

    public UIProgressBar(float current, float max, string title) : base(title)
    {
        this.current = current;
        this.max = max;
        this.title = title;

        Height = StyleDimension.FromPixels(12f);

        this.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/UIProgressBarFrame", ReLogic.Content.AssetRequestMode.ImmediateLoad), new(0, 0, 4, 16)));
        this.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/UIProgressBarFrame", ReLogic.Content.AssetRequestMode.ImmediateLoad), new(6, 0, 4, 16)).With(e =>
        {
            e.HAlign = 1f;
        }));

        ProgressContainer = this.AddElement(new UIElement().With(e =>
        {
            e.Width = StyleDimension.FromPixelsAndPercent(-4f, 1f);
            e.Height = StyleDimension.Fill;
            e.HAlign = 0.5f;
            e.Top = StyleDimension.FromPixels(2f);
        }));

        SetHoverText($"{title}: {current} / {max}");
    }

    public void UpdateProgress(float progress, float newMax)
    {
        ProgressContainer.RemoveAllChildren();

        for (int i = 0; i < Math.Floor(ProgressContainer.GetDimensions().Width / 2) * (progress / newMax); i++)
        {
            ProgressContainer.AddElement(new UIImage(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/UIProgressBarFill", ReLogic.Content.AssetRequestMode.ImmediateLoad)).With(e =>
            {
                e.Left = StyleDimension.FromPixels(2f * i);
            }));
        }

        current = progress;
        max = newMax;
        SetHoverText($"{title}: {current} / {max}");
        ProgressContainer.Recalculate();
    }

    public void ChangeHoverTextType(string title, bool includeProgress)
    {
        SetHoverText(includeProgress ? $"{title}: {current} / {max}" : title);
    }

    bool Loaded = false;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (!Loaded)
        {
            for (int i = 0; i < Math.Floor((GetDimensions().Width - 8) / 2); i++)
            {
                this.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/UIProgressBarFrame", ReLogic.Content.AssetRequestMode.ImmediateLoad), new(4, 0, 2, 16)).With(e =>
                {
                    e.Left = StyleDimension.FromPixels(4f + 2f * i);
                }));
            }

            UpdateProgress(current, max);
            Loaded = true;
        }
    }
}
