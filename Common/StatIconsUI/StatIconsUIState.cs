using MarioLand.Common.Players;
using MarioLand.Common.UIElements;
using MarioLand.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace MarioLand.Common.PMeterUI;
public class StatIconsUIState : UIState
{
    public static bool Visible => Main.LocalPlayer.GetModPlayer<MarioLandPlayer>().DoTransformationEffects && Main.playerInventory;

    readonly List<UIHoverImage> StatIcons = [];
    readonly List<UIText> StatIconTexts = [];

    public override void OnInitialize()
    {
        new List<string> { "Pow", "SP" }.ForEach(x =>
        {
            this.AddElement(new UIHoverImage($"{nameof(MarioLand)}/Common/StatIconsUI/Stat{x}", $"0 {x}")).With(e =>
            {
                e.AddElement(new UIText("0").With(t =>
                {
                    t.HAlign = t.VAlign = 0.5f;
                    StatIconTexts.Add(t);
                }));

                StatIcons.Add(e);
            });
        });
    }

    public override void Update(GameTime gameTime)
    {
        MarioLandPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MarioLandPlayer>();

        StatIcons.ForEach(e =>
        {
            int index = StatIcons.IndexOf(e);
            string text = (index == 0 ? modPlayer.StatPowActual : modPlayer.StatSP).ToString();
            Vector2 position = new(AccessorySlotLoader.DefenseIconPosition.X - 44f, AccessorySlotLoader.DefenseIconPosition.Y + 6f + (38f * (index + 1)));
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);
            Vector2 textPosition = position - textSize * 0.5f * Main.inventoryScale;

            e.Left = StyleDimension.FromPixels(position.X);
            e.Top = StyleDimension.FromPixels(position.Y);
            e.ImageScale = Main.inventoryScale + 0.2f;
            e.SetHoverText(e.hoverText.Replace(e.hoverText.Split(' ')[0], index == 0 ? text : $"{text}/{modPlayer.StatSPMax + modPlayer.StatSPMaxBonus}"));

            StatIconTexts[index].SetText(text);
        });
    }
}
