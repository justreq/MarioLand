using MarioLand.Common.CustomLoadout;
using MarioLand.Common.Globals;
using MarioLand.Common.Players;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace MarioLand.Content.Items.Transformations;
public abstract class TransformationItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.value = 0;
        Item.accessory = true;
        Item.GetGlobalItem<MarioLandGlobalItem>().ItemContext = MarioLand.ItemContext.Transformation;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips[tooltips.FindIndex(e => e.Name == "Equipable")].Text += $" Transformation";
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand)
    {
        return !(pre == -1 || pre == -3);
    }

    public override bool CanEquipAccessory(Player player, int slot, bool modded)
    {
        return modded && slot == ModContent.GetInstance<SlotTransformation>().Type;
    }

    public override void UpdateEquip(Player player)
    {
        MarioLandPlayer modPlayer = player.GetModPlayer<MarioLandPlayer>();

        player.noFallDmg = true;
        player.lifeRegen = 0;
        player.lifeRegenTime = 0;

        player.statDefense += modPlayer.StatDef + modPlayer.StatDefBonus;
        player.statLifeMax2 = modPlayer.StatHPMax + modPlayer.StatHPMaxBonus;

        if (player.mount.Type == -1)
        {
            if (modPlayer.PreviousPowerUp != MarioLand.PowerUp.FrogSuit)
            {
                if (modPlayer.PreviousPowerUp != MarioLand.PowerUp.CapeFeather || !modPlayer.IsFlyingWithPSpeed) modPlayer.GroundPound();
                modPlayer.ConsecutiveJumps();
                if (modPlayer.SuperStar && !modPlayer.IsGrounded) modPlayer.SuperStarJumpFlip();
            }

            player.accFlipper = true;
            player.spikedBoots = player.position.X != modPlayer.LastWallJumpXPosition && !modPlayer.IsFlyingWithPSpeed && !player.wet ? 1 : 0;
        }

        modPlayer.HealingSickness();
        modPlayer.WallJump();
        modPlayer.PSpeed();

        modPlayer.JumpDamageCooldown = (int)MathHelper.Clamp(modPlayer.JumpDamageCooldown + 1, 0, 10);
        if (modPlayer.IsGrounded) modPlayer.StompCount = 0;

        if (PlayerInput.Triggers.JustPressed.Jump && modPlayer.IsGrounded && !player.wet) SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Jump") { Volume = 0.5f });
        if (PlayerInput.Triggers.JustPressed.Jump && player.wet && modPlayer.PreviousPowerUp != MarioLand.PowerUp.FrogSuit) SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Swim") { Volume = 0.5f });
    }
}