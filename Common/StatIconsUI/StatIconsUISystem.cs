using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace MarioLand.Common.PMeterUI;
public class StatIconsUISystem : ModSystem
{
    public UserInterface StatIconsUserInterface;
    private GameTime lastUpdateUiGameTime;

    public override void Load()
    {
        LoadUI();
    }

    public void LoadUI()
    {
        if (!Main.dedServ)
        {
            StatIconsUIState StatIconsUIState = new();
            StatIconsUserInterface = new UserInterface();
            StatIconsUserInterface.SetState(StatIconsUIState);
            StatIconsUIState.Activate();
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        lastUpdateUiGameTime = gameTime;
        if (StatIconsUIState.Visible) StatIconsUserInterface.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("MarioLand: Stat Icons User Interface", delegate
            {
                if (lastUpdateUiGameTime != null && StatIconsUIState.Visible) StatIconsUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);

                return true;
            }, InterfaceScaleType.UI));
        }
    }
}
