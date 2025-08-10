using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Common.Globals;
public class MarioLandGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public bool isFrozen = false;

    public override bool PreAI(NPC npc)
    {
        if (isFrozen) return false;
        return base.PreAI(npc);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (isFrozen) return false;
        return base.CanBeHitByProjectile(npc, projectile);
    }

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        if (isFrozen) return false;
        return base.CanBeHitByItem(npc, player, item);
    }

    public override bool CanBeHitByNPC(NPC npc, NPC attacker)
    {
        if (isFrozen) return false;
        return base.CanBeHitByNPC(npc, attacker);
    }

    public override bool CanHitNPC(NPC npc, NPC target)
    {
        if (isFrozen) return false;
        return base.CanHitNPC(npc, target);
    }

    public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
    {
        if (isFrozen) return false;
        return base.CanHitPlayer(npc, target, ref cooldownSlot);
    }

    public override void FindFrame(NPC npc, int frameHeight)
    {
        if (isFrozen)
        {
            npc.frame.X = npc.frame.Y = 0;
            return;
        }

        base.FindFrame(npc, frameHeight);
    }
}
