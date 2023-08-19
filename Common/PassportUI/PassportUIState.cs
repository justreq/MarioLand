using MarioLand.Common.Players;
using MarioLand.Common.UIElements;
using MarioLand.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace MarioLand.Common.PassportUI;
public class PassportUIState : UIState
{
    public static bool Visible => Main.LocalPlayer.GetModPlayer<MarioLandPlayer>().DoTransformationEffects;
    public bool ShowPassport = false;

    public static Player UICharacter;

    Vector2 PassportUIToggleButtonOffset = Vector2.Zero;
    PassportUIToggleButton PassportUIToggleButton { get; set; }

    DraggableUIPanel ContentContainer { get; set; }

    UIText PlayerNameText { get; set; }
    UIText LevelText { get; set; }

    UIImage RankSlots { get; set; }
    UIImageFramed RankIcons { get; set; }

    UIElement StatsContainer { get; set; }
    UIText StatHPText;
    UIText StatSPText;
    UIText StatPowText;
    UIText StatDefText;

    UIElements.UIProgressBar LevelProgressBar { get; set; }
    UIElements.UIProgressBar RankProgressBar { get; set; }

    public override void OnInitialize()
    {
        PassportUIToggleButton = this.AddElement(new PassportUIToggleButton().With(e =>
        {
            e.Left = StyleDimension.FromPixelsAndPercent(PassportUIToggleButtonOffset.X, 0.01f);
            e.Top = StyleDimension.FromPixelsAndPercent(PassportUIToggleButtonOffset.Y, 0.935f);
            e.OnLeftClick += TogglePassportUI;
        }));

        ContentContainer = new DraggableUIPanel().With(e =>
       {
           e.Width = StyleDimension.FromPixels(512f);
           e.Height = StyleDimension.FromPixels(334f);
           e.HAlign = 0.5f;
           e.VAlign = 0.5f;
           e.BackgroundColor = new Color(224, 24, 16);
           e.BorderColor = Color.Black;
       });

        PlayerNameText = ContentContainer.AddElement(new UIText("", 1.4f).With(e =>
        {
            e.Top = StyleDimension.FromPixels(10f);
        }));

        LevelText = ContentContainer.AddElement(new UIText("", 1.2f).With(e =>
        {
            e.Top = StyleDimension.FromPixels(20f + PlayerNameText.GetDimensions().Height);
        }));

        RankSlots = ContentContainer.AddElement(new UIImage(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/RankSlots", ReLogic.Content.AssetRequestMode.ImmediateLoad)).With(e =>
       {
           e.HAlign = 0.5f;
           e.Top = StyleDimension.FromPixels(50f);
       }));

        RankIcons = RankSlots.AddElement(new UIImageFramed(ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/UI/RankIcons", ReLogic.Content.AssetRequestMode.ImmediateLoad), new(0, 0, 48, 56)).With(e =>
       {
           e.Left = StyleDimension.FromPixels(2f);
       }));

        StatsContainer = ContentContainer.AddElement(new UIElement().With(e =>
       {
           e.Width = StyleDimension.FromPercent(0.85f);
           e.Height = StyleDimension.FromPixels(64f + 36f);
           e.Top = StyleDimension.FromPixels(120f + RankSlots.GetDimensions().Height);
           e.HAlign = 0.5f;
       }));

        StatsContainer.AddElement(new PassportStatContainer(ref StatHPText, "HP", 0f, 0f));
        StatsContainer.AddElement(new PassportStatContainer(ref StatSPText, "SP", 1f, 0f));
        StatsContainer.AddElement(new PassportStatContainer(ref StatPowText, "Pow", 0f, 54f));
        StatsContainer.AddElement(new PassportStatContainer(ref StatDefText, "Def", 1f, 54f));
    }

    private void TogglePassportUI(UIMouseEvent evt, UIElement listeningElement)
    {
        bool previousState = ShowPassport;

        ShowPassport = !previousState;
        PassportUIToggleButton.SetState(!previousState);

        if (ShowPassport)
        {
            PassportUIToggleButtonOffset.X -= 6;
            PassportUIToggleButtonOffset.Y -= 18;
            PassportUIToggleButton.Recalculate();

            this.AddElement(ContentContainer);
        }
        else
        {
            PassportUIToggleButtonOffset = Vector2.Zero;

            ContentContainer.Remove();
        }
    }

    public void SetRankHoverText()
    {
        MarioLandPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MarioLandPlayer>();
        RankIcons.RemoveAllChildren();
        for (int i = 0; i < (int)modPlayer.CurrentRank + (modPlayer.CurrentLevel < 40 ? 1 : 0); i++)
        {
            UIHoverElement rankHoverText = RankIcons.AddElement(new UIHoverElement(Enum.GetNames(typeof(MarioLand.Rank))[i] + " Rank").With(e =>
            {
                e.Width = StyleDimension.FromPixels(48f);
                e.Height = StyleDimension.FromPixels(44f);
                e.Left = StyleDimension.FromPixels(48f * i);
                e.VAlign = i == 0 || i == 4 ? 1f : i == 1 || i == 3 ? 0.3f : 0f;
            }));
        }
    }

    bool Loaded = false;
    public override void Update(GameTime gameTime)
    {
        MarioLandPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MarioLandPlayer>();

        PassportUIToggleButton.Left = StyleDimension.FromPixelsAndPercent(PassportUIToggleButtonOffset.X, 0.01f);
        PassportUIToggleButton.Top = StyleDimension.FromPixelsAndPercent(PassportUIToggleButtonOffset.Y, 0.935f);

        if (!Loaded)
        {
            int originalMount = Main.LocalPlayer.mount.Type;

            UICharacter = Main.LocalPlayer.clientClone();

            string equipName = $"{modPlayer.CurrentTransformation}None";
            UICharacter.head = EquipLoader.GetEquipSlot(MarioLand.Instance, equipName, EquipType.Head);
            UICharacter.body = EquipLoader.GetEquipSlot(MarioLand.Instance, equipName, EquipType.Body);
            UICharacter.legs = EquipLoader.GetEquipSlot(MarioLand.Instance, equipName, EquipType.Legs);

            ContentContainer.AddElement(new UICharacter(UICharacter, true).With(e =>
            {
                e.Top = StyleDimension.FromPixels(10f);
                e.HAlign = 1f;
            }));

            RankIcons.SetFrame(new(0, 0, 48 * ((int)modPlayer.CurrentRank + 1), 56));
            SetRankHoverText();

            LevelProgressBar = ContentContainer.AddElement(new UIElements.UIProgressBar(modPlayer.CurrentXP, modPlayer.XPNeededForNextLevel, "Next Level").With(e =>
             {
                 e.Width = StyleDimension.FromPercent(0.8f);
                 e.HAlign = 0.5f;
                 e.Top = StyleDimension.FromPixels(StatsContainer.Top.Pixels + StatsContainer.Height.Pixels + 20f);
             }));

            RankProgressBar = ContentContainer.AddElement(new UIElements.UIProgressBar(modPlayer.CurrentLevel, modPlayer.RankLevels.FindLast(e => e <= modPlayer.CurrentLevel), "Next Rank").With(e =>
             {
                 e.Width = StyleDimension.FromPercent(0.8f);
                 e.HAlign = 0.5f;
                 e.Top = StyleDimension.FromPixels(StatsContainer.Top.Pixels + StatsContainer.Height.Pixels + 56f);
             }));

            Loaded = true;
        }

        PlayerNameText.SetText(Main.LocalPlayer.name);
        LevelText.SetText($"Level {modPlayer.CurrentLevel}");

        RankIcons.SetFrame(new(0, 0, 48 * ((int)modPlayer.CurrentRank + (modPlayer.CurrentLevel < 40 ? 1 : 0)), 56));

        StatHPText.SetText($"{Main.LocalPlayer.statLife} / {Main.LocalPlayer.statLifeMax2}");
        StatSPText.SetText($"{modPlayer.StatSP} / {modPlayer.StatSPMax + modPlayer.StatSPMaxBonus}");
        StatPowText.SetText(modPlayer.StatPowActual.ToString());
        StatDefText.SetText(Main.LocalPlayer.statDefense.Positive.ToString());

        LevelProgressBar.UpdateProgress(modPlayer.CurrentXP, modPlayer.XPNeededForNextLevel);

        if (modPlayer.CurrentLevel <= 40)
        {
            if (RankIcons.Children.Count() != (int)modPlayer.CurrentRank + 1) SetRankHoverText();

            if (modPlayer.PreviousLevel != modPlayer.CurrentLevel)
            {
                RankProgressBar.UpdateProgress(modPlayer.CurrentLevel, modPlayer.RankLevels[(int)modPlayer.CurrentRank + (modPlayer.CurrentLevel < 40 ? 1 : 0)]);
            }
        }
        else RankProgressBar.SetHoverText($"Current Level: {modPlayer.CurrentLevel}");
    }
}
