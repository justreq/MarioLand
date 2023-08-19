using MarioLand.Common.CustomLoadout;
using MarioLand.Common.Globals;
using MarioLand.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace MarioLand.Content.Items.PowerUps;
[AttributeUsage(AttributeTargets.Class)]
public class PowerUpData : Attribute
{
    public readonly int width;
    public readonly int height;
    public readonly MarioLand.PowerUp powerUp;

    public PowerUpData(int width, int height, MarioLand.PowerUp powerUp)
    {
        this.width = width;
        this.height = height;
        this.powerUp = powerUp;
    }
}

public abstract class PowerUpItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.value = 0;
        Item.accessory = true;
        Item.GetGlobalItem<MarioLandGlobalItem>().ItemContext = MarioLand.ItemContext.PowerUp;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine itemName = tooltips[tooltips.FindIndex(e => e.Name == "ItemName")];

        itemName.Text += $" [i:{nameof(MarioLand)}/{nameof(PowerUp)}] Power-Up";
        itemName.OverrideColor = Main.DiscoColor;
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand)
    {
        return !(pre == -1 || pre == -3);
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded)
    {
        return modded && slot == ModContent.GetInstance<SlotPowerUp>().Type;
    }

    bool animateSpawnFromBlock = false;
    Vector2 spawnPosition = Vector2.Zero;
    int offset = 0;
    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        float normalGravity = gravity;
        if (animateSpawnFromBlock && Item.noGrabDelay > 0)
        {
            gravity = 0f;
            Item.Center = spawnPosition + new Vector2(8, -4 - offset * 0.6f);

            offset++;
        }
        else
        {
            gravity = normalGravity;
            animateSpawnFromBlock = false;
        }
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_TileInteraction interaction)
        {
            Point tileCoords = interaction.TileCoords;
            Tile tile = Main.tile[tileCoords.X, tileCoords.Y];

            if (tile.TileType == ModContent.TileType<QuestionBlock>() || (tile.TileType == ModContent.TileType<BrickBlock>() && tile.TileFrameX / 36 == 1))
            {
                animateSpawnFromBlock = true;
                spawnPosition = tileCoords.ToWorldCoordinates();
            }
        }
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Item.GetGlobalItem<MarioLandGlobalItem>().lightColor = lightColor;
        return false;
    }
}

public class PowerUp : ModItem
{
    public override string Texture => $"{nameof(MarioLand)}/Assets/Textures/VanillaMushroom";
}