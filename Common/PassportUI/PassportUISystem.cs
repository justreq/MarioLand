using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace MarioLand.Common.PassportUI;
public class PassportUISystem : ModSystem
{
    public UserInterface PassportUserInterface;
    private GameTime lastUpdateUiGameTime;

    public override void Load()
    {
        LoadUI();
    }

    public void LoadUI()
    {
        if (!Main.dedServ)
        {
            PassportUIState PassportUIState = new();
            PassportUserInterface = new UserInterface();
            PassportUserInterface.SetState(PassportUIState);
            PassportUIState.Activate();
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        lastUpdateUiGameTime = gameTime;
        if (PassportUIState.Visible) PassportUserInterface.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("MarioLand: Passport User Interface", delegate
            {
                if (lastUpdateUiGameTime != null && PassportUIState.Visible) PassportUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);

                return true;
            }, InterfaceScaleType.UI));
        }
    }
}
