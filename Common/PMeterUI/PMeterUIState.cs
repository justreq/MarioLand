using MarioLand.Common.Players;
using MarioLand.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace MarioLand.Common.PMeterUI;
public class PMeterUIState : UIState
{
    public static bool Visible => Main.LocalPlayer.GetModPlayer<MarioLandPlayer>().DoTransformationEffects && !Main.playerInventory;

    UIElement PMeterContainer { get; set; }
    List<UIImageFramed> PMeterIncrements = new();

    UIImageFramed PMeterWing { get; set; }

    public override void OnInitialize()
    {
        PMeterContainer = this.AddElement(new UIElement().With(e =>
        {
            e.Left = StyleDimension.FromPixels(465f);
            e.Top = StyleDimension.FromPixels(28f);
            e.Width = StyleDimension.FromPixels(26f * 6 + 4f * 6 + 32f);
            e.Height = StyleDimension.FromPixels(36f);
        }));

        for (int i = 0; i < 6; i++)
        {
            UIImageFramed pMeterIncrement = PMeterContainer.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Common/PMeterUI/PMeterIncrement", ReLogic.Content.AssetRequestMode.ImmediateLoad), new(0, 0, 26, 22)).With(e =>
            {
                e.VAlign = 0.5f;
                e.Left = StyleDimension.FromPixels(30f * i);
            }));

            PMeterIncrements.Add(pMeterIncrement);
        }

        PMeterWing = PMeterContainer.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Common/PMeterUI/PMeterWing", ReLogic.Content.AssetRequestMode.ImmediateLoad), new(0, 0, 32, 36)).With(e =>
        {
            e.VAlign = 0.5f;
            e.HAlign = 1f;
        }));
    }

    int pMeterWingFrame = 1;
    int pMeterWingAnimationCooldown = 0;
    public override void Update(GameTime gameTime)
    {
        MarioLandPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MarioLandPlayer>();

        PMeterIncrements.ForEach(e =>
        {
            if (modPlayer.HasPSpeed || modPlayer.IsFlyingWithPSpeed || modPlayer.PSpeedTimer < 60)
            {
                if (PMeterIncrements.IndexOf(e) < modPlayer.PSpeedTimer / 10) e.SetFrame(new(26, 0, 26, 22));
                else e.SetFrame(new(0, 0, 26, 22));
            }
            else e.SetFrame(new(26, 0, 26, 22));
        });

        if (modPlayer.HasPSpeed || modPlayer.IsFlyingWithPSpeed)
        {
            PMeterWing.SetFrame(new(32 * pMeterWingFrame, 0, 32, 36));

            pMeterWingAnimationCooldown++;

            if (pMeterWingAnimationCooldown > 10)
            {
                pMeterWingFrame += pMeterWingFrame == 1 ? 1 : -1;
                pMeterWingAnimationCooldown = 0;
            }
        }
        else PMeterWing.SetFrame(new(0, 0, 32, 36));
    }
}
