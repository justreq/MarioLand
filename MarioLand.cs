using MarioLand.Common.CustomLoadout;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand;
public class MarioLand : Mod
{
    public enum ItemContext
    {
        None,
        Transformation,
        PowerUp,
        Accessory,
        Overalls,
        Gloves,
        Socks,
        Boots
    }

    public enum Transformation
    {
        None,
        Mario,
        Luigi
    }

    public enum PowerUp
    {
        None,
        FireFlower,
        IceFlower,
        SuperLeaf,
        TanookiSuit,
        CapeFeather,
        FrogSuit,
        HammerSuit
    }

    public enum Rank
    {
        Mushroom,
        Shell,
        Flower,
        Shine,
        Star,
        Rainbow
    }

    public static MarioLand Instance => ModContent.GetInstance<MarioLand>();

    public const int VANILLALOADOUTS = 3;
    public const int EXTRALOADOUTS = 1;

    public static readonly Color[,] CustomLoadoutColors = new Color[EXTRALOADOUTS, 3] {
        { new(186, 12, 47), new(155, 17, 30), new(143, 7, 15) }
    };

    public static List<string> TransformationEquipTextures = [];

    public static List<int> IllegalHealingItems = [ItemID.LesserHealingPotion, ItemID.HealingPotion, ItemID.GreaterHealingPotion, ItemID.SuperHealingPotion, ItemID.BottledHoney, ItemID.StrangeBrew, ItemID.LesserRestorationPotion, ItemID.RestorationPotion];

    public static List<PowerUp> TailPowerUps = [PowerUp.SuperLeaf, PowerUp.TanookiSuit];

    private Asset<Texture2D> oldMushroomTexture;

    public override void Load()
    {
        oldMushroomTexture = TextureAssets.Item[ItemID.Mushroom];
        TextureAssets.Item[ItemID.Mushroom] = ModContent.Request<Texture2D>($"{nameof(MarioLand)}/Assets/Textures/VanillaMushroom");

        GetFileNames().Where(e => e.StartsWith("Assets/Textures/Transformations")).ToList().ForEach(file =>
        {
            var root = "Assets/Textures/Transformations/";
            var path = file[root.Length..].Split("/");
            var name = path[1].Split(".")[0];
            var type = name.Split("_")[1];

            EquipLoader.AddEquipTexture(this, $"{nameof(MarioLand)}/{file}".Split(".")[0], Enum.Parse<EquipType>(type), name: name.Split("_")[0]);
            TransformationEquipTextures.Add(name.Split("_")[0]);
        });

        AddContent<SlotTransformation>();
        AddContent<SlotPowerUp>();
        AddContent<SlotOveralls>();
        AddContent<SlotGloves>();
        AddContent<SlotSocks>();
        AddContent<SlotBoots>();
        AddContent<SlotAccessory1>();
        AddContent<SlotAccessory2>();
        AddContent<SlotAccessory3>();
    }

    public override void Unload()
    {
        TextureAssets.Item[ItemID.Mushroom] = oldMushroomTexture;
        oldMushroomTexture = null;
    }

    public static void SetupEquipTextures(string baseName)
    {
        TransformationEquipTextures.Where(e => e.StartsWith(baseName)).ToList().ForEach(e =>
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Instance, e, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Instance, e, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Instance, e, EquipType.Legs);

            if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            if (equipSlotBody != -1) ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            if (equipSlotBody != -1) ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        });
    }
}