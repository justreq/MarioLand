using MarioLand.Common.Globals;
using MarioLand.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Patches;
internal sealed class DrawNPCHealthBar : BasePatch
{
    internal override void Patch(Mod mod)
    {
        MonoModHooks.Add(typeof(NPCLoader).GetMethod("DrawHealthBar"), DrawHealthBar);
    }

    delegate bool orig_DrawHealthBar(NPC npc, ref float scale);
    private bool DrawHealthBar(orig_DrawHealthBar orig, NPC npc, ref float scale)
    {
        if (!npc.TryGetGlobalNPC<MarioLandGlobalNPC>(out var globalNPC) || !globalNPC.isFrozen) return orig(npc, ref scale);
        return false;
    }
}