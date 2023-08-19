using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace MarioLand.Common.PMeterUI;
public class PMeterUISystem : ModSystem
{
    public UserInterface PMeterUserInterface;
    private GameTime lastUpdateUiGameTime;

    public override void Load()
    {
        LoadUI();
    }

    public void LoadUI()
    {
        if (!Main.dedServ)
        {
            PMeterUIState PMeterUIState = new();
            PMeterUserInterface = new UserInterface();
            PMeterUserInterface.SetState(PMeterUIState);
            PMeterUIState.Activate();
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        lastUpdateUiGameTime = gameTime;
        if (PMeterUIState.Visible) PMeterUserInterface.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("MarioLand: P-Meter User Interface", delegate
            {
                if (lastUpdateUiGameTime != null && PMeterUIState.Visible) PMeterUserInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);

                return true;
            }, InterfaceScaleType.UI));
        }
    }
}
