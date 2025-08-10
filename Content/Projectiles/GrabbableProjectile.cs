﻿using MarioLand.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace MarioLand.Content.Projectiles;
public abstract class GrabbableProjectile : ModProjectile
{
    public int grabOwner = -1;
    public int throwDirection = 0;

    public override void AI()
    {
        if (grabOwner == -1)
        {
            if (throwDirection != 0) ThrowUpdate();
            return;
        }

        Player player = Main.player[grabOwner];

        Projectile.Bottom = player.Top;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
    }

    public virtual void Throw()
    {
        var player = Main.player[grabOwner];

        throwDirection = Math.Sign(Main.MouseWorld.X - player.position.X);

        if (throwDirection != player.direction)
        {
            var modPlayer = player.GetModPlayer<MarioLandPlayer>();

            modPlayer.ForceDirection = throwDirection;
            modPlayer.ForceDirectionTimer = 15;
        }

        grabOwner = -1;
    }

    public virtual void ThrowUpdate() { }
}