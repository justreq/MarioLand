using DragonLens;
using DragonLens.Content.Tools.Spawners;
using MarioLand.Common.Players;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace MarioLand.Patches;
internal sealed class AntiGameRaidersProtocol : BasePatch
{
    internal override void Patch(Mod mod)
    {
        // DragonLens:BuffButton
        DragonLensPatch(typeof(BuffButton), "SafeDraw", "buff");
        DragonLensPatch(typeof(BuffButton), "SafeClick", "buff", true);
        DragonLensPatch(typeof(BuffButton), "SafeRightClick", "buff", true);
        // DragonLens:DustButton
        DragonLensPatch(typeof(DustButton), "SafeDraw", "dust");
        DragonLensPatch(typeof(DustButton), "SafeClick", "dust", true);
        // DragonLens:ItemButton
        DragonLensPatch(typeof(ItemButton), "SafeDraw", "item");
        DragonLensPatch(typeof(ItemButton), "SafeClick", "item", true);
        DragonLensPatch(typeof(ItemButton), "SafeRightMouseDown", "item");
        DragonLensPatch(typeof(ItemButton), "SafeUpdate", "item");
        // DragonLens:NPCButton
        DragonLensPatch(typeof(NPCButton), "DrawSelf", "npc");
        DragonLensPatch(typeof(NPCButton), "SafeDraw", "npc");
        DragonLensPatch(typeof(NPCButton), "SafeClick", "npc", true);
        DragonLensPatch(typeof(NPCButton), "SafeRightClick", "npc", true);
        DragonLensPatch(typeof(NPCButton), "SafeUpdate", "npc");
        // DragonLens:ProjectileButton
        DragonLensPatch(typeof(ProjectileButton), "SafeDraw", "proj");
        DragonLensPatch(typeof(ProjectileButton), "SafeClick", "proj", true);
        DragonLensPatch(typeof(ProjectileButton), "SafeRightClick", "proj", true);
        // DragonLens:SoundButton
        DragonLensPatch(typeof(SoundButton), "SafeDraw", "sound");
        DragonLensPatch(typeof(SoundButton), "SafeClick", "sound", true);
        // DragonLens:TileButton
        DragonLensPatch(typeof(TileButton), "SafeDraw", "tile");
        DragonLensPatch(typeof(TileButton), "SafeClick", "tile", true);
    }

    readonly static Func<int, bool> isMarioLandBuff = type => ModContent.GetModBuff(type) != null && ModContent.GetModBuff(type).Mod.Name == nameof(MarioLand);
    readonly static Func<int, bool> isMarioLandDust = dust => ModContent.GetModDust(dust) != null && ModContent.GetModDust(dust).Mod.Name == nameof(MarioLand);
    readonly static Func<Item, bool> isMarioLandItem = item => item.ModItem != null && item.ModItem.Mod.Name == nameof(MarioLand);
    readonly static Func<NPC, bool> isMarioLandNPC = npc => npc.ModNPC != null && npc.ModNPC.Mod.Name == nameof(MarioLand);
    readonly static Func<Projectile, bool> isMarioLandProjectile = proj => proj.ModProjectile != null && proj.ModProjectile.Mod.Name == nameof(MarioLand);
    readonly static Func<SoundStyle, bool> isMarioLandSound = sound => sound.SoundPath != null && sound.SoundPath.StartsWith(nameof(MarioLand));
    readonly static Func<int, bool> isMarioLandTile = tile => ModContent.GetModTile(tile) != null && ModContent.GetModTile(tile).Mod.Name == nameof(MarioLand);

    static void DragonLensPatch(Type type, string methodName, string buttonType, bool showMessage = false)
    {
        MonoModHooks.Modify(type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), il =>
        {
            ILCursor c = new(il);

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, type.GetField(buttonType == "buff" ? "type" : buttonType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            c.EmitDelegate((Delegate)(buttonType switch
            {
                "buff" => isMarioLandBuff,
                "dust" => isMarioLandDust,
                "item" => isMarioLandItem,
                "npc" => isMarioLandNPC,
                "proj" => isMarioLandProjectile,
                "sound" => isMarioLandSound,
                "tile" => isMarioLandTile,
                _ => () => false
            }));

            var label = c.DefineLabel();
            c.Emit(OpCodes.Brfalse, label);

            if (showMessage) c.EmitDelegate(MarioLand.ShowGameRaidersProtocolMessage);

            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        });
    }
}