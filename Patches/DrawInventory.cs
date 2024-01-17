using MarioLand.Common.Players;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Patches;
internal sealed class DrawInventory : BasePatch
{
    internal override void Patch(Mod mod)
    {
        IL_Main.DrawInventory += IL_Main_DrawInventory;
    }

    private void IL_Main_DrawInventory(ILContext il)
    {
        ILCursor c = new(il);

        c.TryGotoNext(i => i.MatchCall<Main>("DrawLoadoutButtons"));
        c.TryGotoNext(i => i.MatchBr(out _));

        c.EmitDelegate(() =>
        {
            return Main.LocalPlayer.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout;
        });

        ILLabel label = c.DefineLabel();
        c.Emit(OpCodes.Brtrue, label);

        c.TryGotoNext(MoveType.After, i => i.MatchLdloc(88), i => i.MatchLdcI4(3), i => i.MatchBlt(out _));

        c.MarkLabel(label);

        c.TryGotoNext(i => i.MatchBr(out _));

        c.EmitDelegate(() =>
        {
            return Main.LocalPlayer.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout;
        });

        ILLabel label2 = c.DefineLabel();
        c.Emit(OpCodes.Brtrue, label2);

        c.TryGotoNext(MoveType.After, i => i.MatchLdloc(98), i => i.MatchLdcI4(13), i => i.MatchBlt(out _));

        c.MarkLabel(label2);

        c.TryGotoNext(i => i.MatchBr(out _));

        c.EmitDelegate(() =>
        {
            return Main.LocalPlayer.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout;
        });

        ILLabel label3 = c.DefineLabel();
        c.Emit(OpCodes.Brtrue, label3);

        c.TryGotoNext(MoveType.After, i => i.MatchLdloc(103), i => i.MatchLdcI4(3), i => i.MatchBlt(out _));

        c.MarkLabel(label3);

        c.TryGotoNext(i => i.MatchLdloc(87));

        c.EmitDelegate(() =>
        {
            return Main.LocalPlayer.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout;
        });

        ILLabel label4 = c.DefineLabel();
        c.Emit(OpCodes.Brtrue, label4);

        c.TryGotoNext(MoveType.Before, i => i.MatchCall<AccessorySlotLoader>("get_DefenseIconPosition"));

        c.MarkLabel(label4);

        c.TryGotoNext(MoveType.After, i => i.MatchConvI4());

        c.Emit(OpCodes.Ldc_I4, 93);
        c.Emit(OpCodes.Add);
    }
}
