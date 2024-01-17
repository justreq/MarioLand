using MarioLand.Common.Globals;
using MarioLand.Common.Players;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MarioLand.Common.CustomLoadout;
[AttributeUsage(AttributeTargets.Class)]
public class Context : Attribute
{
    public readonly MarioLand.ItemContext context;

    public Context(MarioLand.ItemContext context)
    {
        this.context = context;
    }
}

[Autoload(false)]
public abstract class MarioLandSlot : ModAccessorySlot
{
    private MarioLand.ItemContext Context => ((Context)GetType().GetCustomAttributes(typeof(Context), false)[0]).context;

    public override bool DrawVanitySlot => false;
    public override string FunctionalTexture => $"{nameof(MarioLand)}/Common/CustomLoadout/{Name}";

    public override void OnMouseHover(AccessorySlotType context)
    {
        Main.hoverItemName = context == AccessorySlotType.DyeSlot ? "Dye" : Name.Replace("Slot", "");
    }

    public override bool DrawDyeSlot => Name.Contains("Transformation");

    public override bool IsEnabled()
    {
        return !Main.gameMenu && Player.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout;
    }

    public override bool IsVisibleWhenNotEnabled()
    {
        return false;
    }

    public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
    {
        if (!Player.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout) return false;

        return checkItem.GetGlobalItem<MarioLandGlobalItem>().ItemContext == Context;
    }

    public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
    {
        if (!Player.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout) return false;

        return item.GetGlobalItem<MarioLandGlobalItem>().ItemContext == Context;
    }
}

public abstract class MarioLandSlotAccessory : MarioLandSlot
{
    public override string FunctionalTexture => $"{nameof(MarioLand)}/Common/CustomLoadout/SlotAccessory";

    public override void OnMouseHover(AccessorySlotType context)
    {
        Main.hoverItemName = "Accessory";
    }
}

[Context(MarioLand.ItemContext.Transformation)] public class SlotTransformation : MarioLandSlot { }

[Context(MarioLand.ItemContext.PowerUp)] public class SlotPowerUp : MarioLandSlot { }

[Context(MarioLand.ItemContext.Overalls)]
public class SlotOveralls : MarioLandSlot
{
    public override bool IsEnabled()
    {
        return false;
    }
}

[Context(MarioLand.ItemContext.Gloves)]
public class SlotGloves : MarioLandSlot
{
    public override bool IsEnabled()
    {
        return false;
    }
}

[Context(MarioLand.ItemContext.Socks)]
public class SlotSocks : MarioLandSlot
{
    public override bool IsEnabled()
    {
        return false;
    }
}

[Context(MarioLand.ItemContext.Boots)]
public class SlotBoots : MarioLandSlot
{
    public override bool IsEnabled()
    {
        return false;
    }
}

[Context(MarioLand.ItemContext.Accessory)]
public class SlotAccessory1 : MarioLandSlotAccessory
{
    public override bool IsEnabled()
    {
        return false;
    }
}

[Context(MarioLand.ItemContext.Accessory)]
public class SlotAccessory2 : MarioLandSlotAccessory
{
    public override bool IsEnabled()
    {
        return base.IsEnabled() && Player.GetModPlayer<MarioLandPlayer>().CurrentRank >= MarioLand.Rank.Shell;
    }
}

[Context(MarioLand.ItemContext.Accessory)]
public class SlotAccessory3 : MarioLandSlotAccessory
{
    public override bool IsEnabled()
    {
        return base.IsEnabled() && Player.GetModPlayer<MarioLandPlayer>().CurrentRank >= MarioLand.Rank.Flower;
    }
}