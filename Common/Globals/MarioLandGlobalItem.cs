using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using MarioLand.Common.Players;
using Terraria.ID;
using Terraria.DataStructures;

namespace MarioLand.Common.Globals;
public class MarioLandGlobalItem : GlobalItem
{
    public Color lightColor;
    public MarioLand.ItemContext ItemContext;

    public MarioLandGlobalItem()
    {
        lightColor = Color.Transparent;
        ItemContext = MarioLand.ItemContext.None;
    }

    public override bool InstancePerEntity => true;

    public override GlobalItem Clone(Item item, Item itemClone)
    {
        MarioLandGlobalItem MarioLandGlobalItem = (MarioLandGlobalItem)base.Clone(item, itemClone);
        MarioLandGlobalItem.lightColor = lightColor;
        MarioLandGlobalItem.ItemContext = ItemContext;
        return MarioLandGlobalItem;
    }

    public override void SetDefaults(Item entity)
    {
        base.SetDefaults(entity);

        if (entity.type == ItemID.Mushroom)
        {
            entity.maxStack = 30;
        }
    }

    public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
    {
        if (player.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout && item.GetGlobalItem<MarioLandGlobalItem>().ItemContext == MarioLand.ItemContext.None) return false;

        return base.CanEquipAccessory(item, player, slot, modded);
    }

    public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
    {
        if (player.GetModPlayer<MarioLandPlayer>().DoTransformationEffects && MarioLand.IllegalHealingItem.Contains(item.type)) healValue = 0;

        base.GetHealLife(item, player, quickHeal, ref healValue);
    }

    public override void OnConsumeItem(Item item, Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        if (modPlayer.DoTransformationEffects && MarioLand.IllegalHealingItem.Contains(item.type)) modPlayer.JustConsumedBadItem = true;
        base.OnConsumeItem(item, player);
    }
}