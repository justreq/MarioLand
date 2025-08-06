using CheatSheet.Menus;
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
        DragonLensPatch(typeof(SoundButton), "SafeDraw", "Sound");
        DragonLensPatch(typeof(SoundButton), "SafeClick", "Sound", true);
        // DragonLens:TileButton
        DragonLensPatch(typeof(TileButton), "SafeDraw", "tile");
        DragonLensPatch(typeof(TileButton), "SafeClick", "tile", true);

        // CheatSheet:Slot
        MonoModHooks.Modify(typeof(Slot).GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), CheatSheetSlotInit);
        MonoModHooks.Modify(typeof(Slot).GetMethod("Slot2_onLeftClick", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), CheatSheetSlotSlot2_onLeftClick);
        MonoModHooks.Modify(typeof(Slot).GetMethod("Draw"), CheatSheetSlotDraw);

        // CheatSheet:NPCSlot
        MonoModHooks.Modify(typeof(NPCSlot).GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), CheatSheetNPCSlotInit);
        MonoModHooks.Modify(typeof(NPCSlot).GetMethod("Slot2_onLeftClick", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), CheatSheetNPCSlotSlot2_onLeftClick);
        MonoModHooks.Modify(typeof(NPCSlot).GetMethod("Draw"), CheatSheetNPCSlotDraw);
    }

    static bool IsMarioLandBuff(int buff) => ModContent.GetModBuff(buff) != null && ModContent.GetModBuff(buff).Mod.Name == nameof(MarioLand);
    static bool IsMarioLandDust(Dust dust) => ModContent.GetModDust(dust.type) != null && ModContent.GetModDust(dust.type).Mod.Name == nameof(MarioLand);
    static bool IsMarioLandItem(Item item) => item.ModItem != null && item.ModItem.Mod.Name == nameof(MarioLand);
    static bool IsMarioLandNPC(NPC npc) => npc.ModNPC != null && npc.ModNPC.Mod.Name == nameof(MarioLand);
    static bool IsMarioLandProjectile(Projectile proj) => proj.ModProjectile != null && proj.ModProjectile.Mod.Name == nameof(MarioLand);
    static bool IsMarioLandSound(SoundStyle sound) => sound.SoundPath != null && sound.SoundPath.StartsWith(nameof(MarioLand));
    static bool IsMarioLandTile(int tile) => ModContent.GetModTile(tile) != null && ModContent.GetModTile(tile).Mod.Name == nameof(MarioLand);

    static void DragonLensPatch(Type type, string methodName, string buttonType, bool showMessage = false)
    {
        MonoModHooks.Modify(type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), il =>
        {
            ILCursor c = new(il);

            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, type.GetField(buttonType == "buff" ? "type" : buttonType == "tile" ? "tileType" : buttonType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            c.EmitDelegate((Delegate)(buttonType switch
            {
                "buff" => IsMarioLandBuff,
                "dust" => IsMarioLandDust,
                "item" => IsMarioLandItem,
                "npc" => IsMarioLandNPC,
                "proj" => IsMarioLandProjectile,
                "Sound" => IsMarioLandSound,
                "tile" => IsMarioLandTile,
                _ => () => false
            }));

            var label = c.DefineLabel();
            c.Emit(OpCodes.Brfalse, label);

            if (showMessage) c.EmitDelegate(MarioLand.ShowGameRaidersProtocolMessage);

            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        });
    }

    static void CheatSheetSlotInit(ILContext il)
    {
        ILCursor c = new(il);

        c.TryGotoNext(i => i.MatchLdnull());
        c.Index += 7;

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(Slot).GetField("item", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        c.EmitDelegate(IsMarioLandItem);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brfalse, label);
        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }

    static void CheatSheetSlotSlot2_onLeftClick(ILContext il)
    {
        ILCursor c = new(il);

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(Slot).GetField("item", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        c.EmitDelegate(IsMarioLandItem);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brfalse, label);

        c.EmitDelegate(MarioLand.ShowGameRaidersProtocolMessage);

        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }

    static void CheatSheetSlotDraw(ILContext il)
    {
        ILCursor c = new(il);

        c.TryGotoNext(MoveType.After, i => i.MatchLdcI4(0), i => i.MatchLdcR4(out _));
        c.Index++;

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(Slot).GetField("item", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        c.EmitDelegate(IsMarioLandItem);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brfalse, label);
        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }

    static void CheatSheetNPCSlotInit(ILContext il)
    {
        ILCursor c = new(il);

        c.TryGotoNext(i => i.MatchRet());
        c.Index -= 10;

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(NPCSlot).GetField("npc", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        c.EmitDelegate(IsMarioLandNPC);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brfalse, label);

        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }

    static void CheatSheetNPCSlotSlot2_onLeftClick(ILContext il)
    {
        ILCursor c = new(il);

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(NPCSlot).GetField("npc", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        c.EmitDelegate(IsMarioLandNPC);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brfalse, label);

        c.EmitDelegate(MarioLand.ShowGameRaidersProtocolMessage);

        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }

    static void CheatSheetNPCSlotDraw(ILContext il)
    {
        ILCursor c = new(il);

        c.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchLdcR4(out _));
        c.Index += 3;

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(OpCodes.Ldfld, typeof(NPCSlot).GetField("npc", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

        c.EmitDelegate(IsMarioLandNPC);

        var label = c.DefineLabel();
        c.Emit(OpCodes.Brfalse, label);

        c.Emit(OpCodes.Ret);
        c.MarkLabel(label);
    }
}