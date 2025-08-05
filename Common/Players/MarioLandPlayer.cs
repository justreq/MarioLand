using MarioLand.Common.CustomLoadout;
using MarioLand.Content.Items.Consumables;
using MarioLand.Content.Items.PowerUps;
using MarioLand.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace MarioLand.Common.Players;
public class MarioLandPlayer : ModPlayer
{
    public int ForceDirection = 0;

    public int CurrentXP = 0;

    public int PreviousLevel;
    public int CurrentLevel = 1;

    public MarioLand.Rank PreviousRank;
    public MarioLand.Rank CurrentRank = MarioLand.Rank.Mushroom;

    public int XPNeededForNextLevel = 24;
    public List<int> RankLevels = [1, 6, 12, 18, 25, 40];

    public MarioLand.Transformation PreviousTransformation;
    public MarioLand.Transformation CurrentTransformation = MarioLand.Transformation.None;

    public MarioLand.PowerUp PreviousPowerUp;
    public MarioLand.PowerUp CurrentPowerUp = MarioLand.PowerUp.None;

    public bool DoTransformationEffects => CurrentTransformation != MarioLand.Transformation.None && Player.GetModPlayer<LoadoutPlayer>().UsingCustomLoadout;

    public int StatHPMax = 20;
    public int StatSPMax = 15;
    public int StatPow = 16;
    public int StatDef = 7;

    public int StatSP = 15;

    public int StatHPMaxBonus = 0;
    public int StatSPMaxBonus = 0;
    public int StatPowBonus = 0;
    public int StatDefBonus = 0;

    public int StatPowActual => StatPow + StatPowBonus;

    public List<int> StatIncreaseValues = [];

    public bool PlayLevelUpAnimation = false;
    public int LevelUpAnimationTimer = 0;

    // Cue agressive inlining
    public bool IsGrounded => new List<Vector2> { Player.BottomLeft, Player.Bottom, Player.BottomRight, Player.BottomLeft + new Vector2(0, 1), Player.Bottom + new Vector2(0, 1), Player.BottomRight + new Vector2(0, 1), Player.BottomLeft + new Vector2(0, 17), Player.Bottom + new Vector2(0, 17), Player.BottomRight + new Vector2(0, 17) }.Any(e => Framing.GetTileSafely(e.ToTileCoordinates()).HasTile && (Main.tileSolid[Framing.GetTileSafely(e.ToTileCoordinates()).TileType] || Main.tileSolidTop[Framing.GetTileSafely(e.ToTileCoordinates()).TileType])) && Player.velocity.Y >= 0;

    public bool GroundPoundRequested = false;
    public int GroundPoundStallTimer = 0;

    public int JumpDamageCooldown = 0;
    public int StompCount = 0;
    public int ConsecutiveJumpCount = 0;
    public int ConsecutiveJumpCooldown = 0;
    public int TripleJumpFlipTimer = 0;

    public float LastWallJumpXPosition = 0f;

    public int PSpeedTimer = 0;
    public bool HasPSpeed = false;
    public bool IsFlyingWithPSpeed = false;

    public bool JustConsumedBadItem = false;
    public int DisgustTimer = 0;

    public bool JustSummonedProjectile = false;
    public int ProjectileSummonCooldown = 0;

    public bool TanookiStatue = false;

    public void GrantXP(int amount)
    {
        CurrentXP += amount;

        if (CurrentXP >= XPNeededForNextLevel) LevelUp();
    }

    public void LevelUp()
    {
        StatIncreaseValues = [Main.rand.Next(1, 6), Main.rand.Next(1, 6), Main.rand.Next(1, 6), Main.rand.Next(1, 6)];
        PlayLevelUpAnimation = true;

        PreviousLevel = CurrentLevel;
        CurrentLevel++;
        CurrentXP -= XPNeededForNextLevel;
        XPNeededForNextLevel = (int)Math.Floor(XPNeededForNextLevel * 1.2);

        if (RankLevels.Contains(CurrentLevel))
        {
            PreviousRank = CurrentRank;
            CurrentRank = (MarioLand.Rank)RankLevels.FindLastIndex(e => e <= CurrentLevel);

            CombatText.NewText(Player.Hitbox, Main.DiscoColor, "Rank Up!", true);
        }
        else CombatText.NewText(Player.Hitbox, Main.DiscoColor, "Level Up!", true);

        StatHPMax += StatIncreaseValues[0];
        StatSPMax += StatIncreaseValues[1];
        StatPow += StatIncreaseValues[2];
        StatDef += StatIncreaseValues[3];
    }

    public void HealingSickness()
    {
        if (!JustConsumedBadItem) return;

        DisgustTimer++;

        if (DisgustTimer >= 60)
        {
            SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Disgusted{CurrentTransformation}") { Volume = 0.5f });
            DisgustTimer = 0;
            JustConsumedBadItem = false;
        }
    }

    public void GroundPound()
    {
        if (!IsGrounded && PlayerInput.Triggers.JustPressed.Down) GroundPoundRequested = true;
        if ((GroundPoundRequested && PlayerInput.Triggers.JustReleased.Down) || IsGrounded)
        {
            if (IsGrounded && GroundPoundRequested)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustDirect(Player.Center - new Vector2(Player.width / 2, 0f), Player.width, Player.height, DustID.Smoke);
                    dust.noGravity = true;
                    dust.position.Y = Player.Bottom.Y;
                    dust.velocity = new(i % 2 == 0 ? -1f : 1f, 0f);
                }

                SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/GroundPound") { Volume = 0.5f });
            }

            GroundPoundRequested = false;
            GroundPoundStallTimer = 0;
        }

        if (GroundPoundRequested)
        {
            GroundPoundStallTimer++;

            if (PreviousPowerUp == MarioLand.PowerUp.TanookiSuit) TanookiStatue = true;
            else
            {
                Player.fullRotationOrigin = Player.Hitbox.Size() / 2;
                Player.fullRotation = Player.fullRotation % MathHelper.TwoPi == 0 && Player.fullRotation != 0 ? Player.fullRotation : ((float)GroundPoundStallTimer / 15) * MathHelper.TwoPi * Player.direction;
            }

            if (GroundPoundStallTimer < 15)
            {
                Player.velocity = Vector2.Zero;
            }
            else
            {
                Player.velocity.Y = 15f;
            }
        }
        else
        {
            Player.fullRotation = 0f;
        }
    }

    public void ConsecutiveJumps()
    {
        if (Player.sliding || GroundPoundRequested || HasPSpeed) ConsecutiveJumpCount = 0;

        if (PlayerInput.Triggers.JustPressed.Jump && IsGrounded && !Player.wet && Math.Abs(Player.velocity.X) > 0f)
        {
            ConsecutiveJumpCount++;
            ConsecutiveJumpCooldown = 0;
        }

        if (ConsecutiveJumpCount > 0 && IsGrounded) ConsecutiveJumpCooldown++;
        if (ConsecutiveJumpCooldown >= 15) ConsecutiveJumpCount = 0;
        if (ConsecutiveJumpCount > 3) ConsecutiveJumpCount = 1;

        if (ConsecutiveJumpCount > 0)
        {
            Player.jumpSpeedBoost += ConsecutiveJumpCount * 0.5f;

            if (ConsecutiveJumpCount % 3 == 0)
            {
                TripleJumpFlipTimer = (int)MathHelper.Clamp(TripleJumpFlipTimer + 1, 0, 45);

                Player.fullRotationOrigin = Player.Hitbox.Size() / 2;
                Player.fullRotation = Player.fullRotation % MathHelper.TwoPi == 0 && Player.fullRotation != 0 ? Player.fullRotation : ((float)TripleJumpFlipTimer / 45) * MathHelper.TwoPi * Player.direction;
            }
            else TripleJumpFlipTimer = 0;
        }
    }

    public void WallJump()
    {
        if (IsGrounded || Player.wet) LastWallJumpXPosition = 0;

        if (Player.sliding && ((Player.slideDir == -1 && (PlayerInput.Triggers.JustReleased.Left || PlayerInput.Triggers.JustPressed.Right)) || (Player.slideDir == 1 && (PlayerInput.Triggers.JustReleased.Right || PlayerInput.Triggers.JustPressed.Left)) || PlayerInput.Triggers.JustPressed.Jump))
        {
            LastWallJumpXPosition = Player.position.X;
        }
    }

    public void PSpeed()
    {
        if (HasPSpeed) Player.jumpSpeedBoost += 1f;

        if (IsFlyingWithPSpeed) return;

        if (IsGrounded)
        {
            PSpeedTimer = (int)MathHelper.Clamp(PSpeedTimer + (Player.maxRunSpeed - Math.Abs(Player.velocity.X) < 1 && Player.mount.Type == -1 ? 1 : -1), 0, 60);

            if (PSpeedTimer == 0) HasPSpeed = false;
            else if (PSpeedTimer == 60) HasPSpeed = true;
        }
        else
        {
            PSpeedTimer = 0;
            HasPSpeed = false;
        }
    }

    public void HitItemBlock()
    {
        if (Collision.SolidCollision(Player.Top - new Vector2(0, 1), 1, 1))
        {
            Point16 collisionPos = (Player.Top - new Vector2(0, 1)).ToTileCoordinates16();
            collisionPos -= new Point16(Main.tile[collisionPos.X, collisionPos.Y].TileFrameX == 0 ? 0 : 1, 0);
            Tile collisionTile = Main.tile[collisionPos.X, collisionPos.Y];

            if (collisionTile.TileType == ModContent.TileType<QuestionBlock>())
            {
                OnHitItemBlock(collisionPos, 1, !QuestionBlock.IsFull(collisionPos.X, collisionPos.Y));
            }
            else if (collisionTile.TileType == ModContent.TileType<BrickBlock>())
            {
                if (BrickBlock.IsFull(collisionPos.X, collisionPos.Y)) OnHitItemBlock(collisionPos, 1, false);
                else
                {
                    Player.velocity.Y = 0;
                    WorldGen.KillTile(collisionPos.X, collisionPos.Y);
                }
            }
        }
    }

    public void OnHitItemBlock(Point16 position, int direction, bool forceCoinSpawn)
    {
        Player.velocity.Y = 0;

        int powerup = GetPowerupToSpawnFromItemBlock(forceCoinSpawn);

        int item = Item.NewItem(Player.GetSource_TileInteraction(position.X, position.Y), position.ToWorldCoordinates(), new Item(powerup, powerup == ItemID.GoldCoin ? Main.rand.Next(1, 11) : 1));
        Main.item[item].noGrabDelay = 60;

        WorldGen.KillTile(position.X, position.Y);
        WorldGen.PlaceTile(position.X, position.Y, (ushort)ModContent.TileType<EmptyBlock>(), true, true);
    }

    public int GetPowerupToSpawnFromItemBlock(bool forceCoinSpawn = false, bool retry = false)
    {
        if (!retry)
        {
            if (Main.rand.NextBool(5) || forceCoinSpawn) return ItemID.SilverCoin;
        }

        int powerup = 0;

        if (Player.ZoneBeach)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                case 1: powerup = ModContent.ItemType<FireFlower>(); break;
                case 2: powerup = ModContent.ItemType<IceFlower>(); break;
                case 3: powerup = ModContent.ItemType<FrogSuit>(); break;
            }
        }
        else if (Player.ZoneCorrupt)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                case 2: powerup = ModContent.ItemType<FireFlower>(); break;
            }
        }
        else if (Player.ZoneCrimson)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                case 1: powerup = ModContent.ItemType<FireFlower>(); break;
                case 2: powerup = ModContent.ItemType<IceFlower>(); break;
            }
        }
        else if (Player.ZoneDesert)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                case 1: powerup = ModContent.ItemType<FireFlower>(); break;
                case 2: powerup = ModContent.ItemType<IceFlower>(); break;
            }
        }
        else if (Player.ZoneForest)
        {
            switch (Main.rand.Next(3))
            {
                case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                case 2: powerup = ModContent.ItemType<SuperLeaf>(); break;
            }
        }
        else if (Player.ZoneGlowshroom)
        {
            switch (Main.rand.Next(3))
            {
                case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                case 2: powerup = ModContent.ItemType<TanookiSuit>(); break;
            }
        }
        else if (Player.ZoneHallow)
        {
            switch (Main.rand.Next(3))
            {
                case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                case 2: powerup = ModContent.ItemType<CapeFeather>(); break;
            }
        }
        else if (Player.ZoneJungle)
        {
            if (Player.ZoneRockLayerHeight)
            {
                switch (Main.rand.Next(3))
                {
                    case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                    case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                    case 2: powerup = ModContent.ItemType<SuperLeaf>(); break;
                }
            }
            else
            {
                switch (Main.rand.Next(5))
                {
                    case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                    case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                    case 2: powerup = ModContent.ItemType<SuperLeaf>(); break;
                    case 3: powerup = ModContent.ItemType<TanookiSuit>(); break;
                    case 4: powerup = ModContent.ItemType<FrogSuit>(); break;
                }
            }
        }
        else if (Player.ZoneSnow)
        {
            if (Player.ZoneRockLayerHeight)
            {
                switch (Main.rand.Next(5))
                {
                    case 0:
                    case 1: powerup = ModContent.ItemType<FireFlower>(); break;
                    case 2:
                    case 3: powerup = ModContent.ItemType<IceFlower>(); break;
                    case 4: powerup = ModContent.ItemType<FrogSuit>(); break;
                }
            }
            else
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                    case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                    case 2: powerup = ModContent.ItemType<FireFlower>(); break;
                }
            }
        }
        else if (Player.ZoneUndergroundDesert)
        {
            switch (Main.rand.Next(2))
            {
                case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
            }
        }
        else if (Player.ZoneUnderworldHeight)
        {
            switch (Main.rand.Next(2))
            {
                case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
            }
        }
        else if (Player.ZoneSkyHeight)
        {
            switch (Main.rand.Next(3))
            {
                case 0: powerup = ModContent.ItemType<FireFlower>(); break;
                case 1: powerup = ModContent.ItemType<IceFlower>(); break;
                case 2: powerup = ModContent.ItemType<CapeFeather>(); break;
            }
        }

        if (Player.HasItemInAnyInventory(powerup)) powerup = GetPowerupToSpawnFromItemBlock(retry: true);

        return powerup;
    }

    public override void ResetEffects()
    {
        PreviousTransformation = CurrentTransformation;
        CurrentTransformation = MarioLand.Transformation.None;
        PreviousPowerUp = CurrentPowerUp;
        CurrentPowerUp = MarioLand.PowerUp.None;

        StatHPMaxBonus = 0;
        StatSPMaxBonus = 0;
        StatPowBonus = 0;
        StatDefBonus = 0;
    }

    public override void FrameEffects()
    {
        if (!DoTransformationEffects) return;

        string equipName = $"{CurrentTransformation}{CurrentPowerUp}";
        Player.head = EquipLoader.GetEquipSlot(Mod, equipName, EquipType.Head);
        Player.body = EquipLoader.GetEquipSlot(Mod, equipName, EquipType.Body);
        Player.legs = EquipLoader.GetEquipSlot(Mod, equipName, EquipType.Legs);

        if (MarioLand.TailPowerUps.Contains(CurrentPowerUp) && IsFlyingWithPSpeed)
        {
            Player.head = EquipLoader.GetEquipSlot(Mod, equipName + "Flying", EquipType.Head);
            Player.body = EquipLoader.GetEquipSlot(Mod, equipName + "Flying", EquipType.Body);
            Player.legs = EquipLoader.GetEquipSlot(Mod, equipName + "Flying", EquipType.Legs);
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (!DoTransformationEffects) return;

        if (TanookiStatue)
        {
            drawInfo.cHead = drawInfo.cBody = drawInfo.cLegs = ContentSamples.ItemsByType[ItemID.ReflectiveMetalDye].dye;
        }
        else drawInfo.cHead = drawInfo.cBody = drawInfo.cLegs = ModContent.GetInstance<SlotTransformation>().DyeItem.dye;
    }

    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
    {
        // Jump and ground pound stomp damage / bounce effects
        if (!DoTransformationEffects || Player != Main.LocalPlayer || damageSource.SourceNPCIndex == -1) return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);

        if (GroundPoundRequested)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i] == Main.npc[damageSource.SourceNPCIndex] && Player.fullRotation % MathHelper.TwoPi == 0)
                {
                    if (!Main.npc[i].boss)
                    {
                        GrantXP((int)Math.Ceiling((double)(Main.npc[i].damage + Main.npc[i].defense + Main.npc[i].lifeMax) / 20));
                        Main.npc[i].StrikeInstantKill();
                    }
                    else if (TanookiStatue)
                    {
                        var target = Main.npc[i];

                        Player.ApplyDamageToNPC(target, StatPowActual, 5f, Math.Sign(target.position.X - Player.position.X));
                        if (target.life <= 0) GrantXP((int)Math.Ceiling((double)(target.damage + target.defense + target.lifeMax) / 20));
                    }
                    else
                    {
                        Player.velocity.Y = Player.controlJump ? -10f : -7.5f;
                        GroundPoundRequested = false;
                    }
                }
            }

            return true;
        }
        else if (JumpDamageCooldown == 20 || Player.velocity.Y > 2f)
        {
            var target = Main.npc[damageSource.SourceNPCIndex];

            StompCount++;

            Player.ApplyDamageToNPC(target, StatPowActual, 0, 0);

            Player.velocity.Y = Player.controlJump ? -10f : -7.5f;
            JumpDamageCooldown = 0;

            if (StompCount > 7) Player.Heal(5);
            SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/{(StompCount <= 7 ? $"Stomp{StompCount}" : "Heal")}") { Volume = 0.5f });

            if (target.life <= 0) GrantXP((int)Math.Ceiling((double)(target.damage + target.defense + target.lifeMax) / 20));

            return true;
        }

        if (TanookiStatue && !GroundPoundRequested)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var target = Main.npc[i];

                if (Main.npc[i] == target && !Main.npc[i].boss)
                {
                    Player.ApplyDamageToNPC(target, StatPowActual, 5f, Math.Sign(target.position.X - Player.position.X));
                    if (target.life <= 0) GrantXP((int)Math.Ceiling((double)(target.damage + target.defense + target.lifeMax) / 20));
                }
            }

            return true;
        }

        return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
    }

    public override void PostUpdate()
    {
        if (PreviousTransformation != CurrentTransformation)
        {
            if (CurrentTransformation != MarioLand.Transformation.None)
            {
                SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/PowerUp") { Volume = 0.5f });
                SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Equip{CurrentTransformation}") { Volume = 0.5f });
            }
            else
            {
                SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/PowerDown") { Volume = 0.5f });
                SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Unequip{PreviousTransformation}") { Volume = 0.5f });
            }

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.LoadoutChange, new ParticleOrchestraSettings
            {
                PositionInWorld = Player.Center,
                UniqueInfoPiece = 0
            }, Player.whoAmI);
        }

        if (PreviousPowerUp != CurrentPowerUp)
        {
            if (CurrentPowerUp != MarioLand.PowerUp.None) SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/PowerUp{(MarioLand.TailPowerUps.Contains(CurrentPowerUp) ? "Tail" : "")}") { Volume = 0.5f });
            else SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/PowerDown") { Volume = 0.5f });

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.LoadoutChange, new ParticleOrchestraSettings
            {
                PositionInWorld = Player.Center,
                UniqueInfoPiece = 0
            }, Player.whoAmI);
        }

        if (PlayLevelUpAnimation)
        {
            LevelUpAnimationTimer++;

            if (LevelUpAnimationTimer == 30) CombatText.NewText(Player.Hitbox, Color.LightPink, $"HP +{StatIncreaseValues[0]}", true, true);
            else if (LevelUpAnimationTimer == 60) CombatText.NewText(Player.Hitbox, Color.Green, $"SP +{StatIncreaseValues[1]}", true, true);
            else if (LevelUpAnimationTimer == 90) CombatText.NewText(Player.Hitbox, Color.Red, $"Pow +{StatIncreaseValues[2]}", true, true);
            else if (LevelUpAnimationTimer == 120) CombatText.NewText(Player.Hitbox, Color.LightGray, $"Def +{StatIncreaseValues[3]}", true, true);
            else if (LevelUpAnimationTimer > 120)
            {
                LevelUpAnimationTimer = 0;
                PlayLevelUpAnimation = false;
            }
        }

        if (IsGrounded)
        {
            Player.fullRotation = 0f;
            IsFlyingWithPSpeed = false;
        }

        if (JustSummonedProjectile)
        {
            ProjectileSummonCooldown++;
            Player.bodyFrame.Y = 56 * (ProjectileSummonCooldown / 3);

            if (ProjectileSummonCooldown > 9)
            {
                ProjectileSummonCooldown = 0;
                JustSummonedProjectile = false;
                ForceDirection = 0;
            }
        }

        if (TanookiStatue && !PlayerInput.Triggers.Current.Down) TanookiStatue = false;

        HitItemBlock();
    }

    public override void SetControls()
    {
        if (TanookiStatue)
        {
            PlayerInput.Triggers.Current.Left = PlayerInput.Triggers.Current.Right = false;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

        if (target.life > 0 || !DoTransformationEffects) return;

        GrantXP((int)Math.Ceiling((double)(target.damage + target.defense + target.lifeMax) / 20));
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (DoTransformationEffects) modifiers.DisableSound();
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (DoTransformationEffects) SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/{CurrentTransformation}Hurt{Main.rand.Next(1, 5)}") { Volume = 0.5f });
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
    {
        bool hasOneUpMushroom = Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<OneUpMushroom>());

        if (!DoTransformationEffects || (!hasOneUpMushroom && !Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<OneUpDeluxe>())))
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);

        int item = -1;
        bool inVoidBag = false;
        int inventorySlot = 0;

        for (int i = 0; i < Player.inventory.Length; i++)
        {
            if (Player.inventory[i].type == ModContent.ItemType<OneUpMushroom>() || Player.inventory[i].type == ModContent.ItemType<OneUpDeluxe>())
            {
                item = Player.inventory[i].type;
                inventorySlot = i;
                break;
            }
        }

        if (item == -1)
        {
            inVoidBag = true;

            for (int i = 0; i < Player.bank4.item.Length; i++)
            {
                if (Player.bank4.item[i].type == ModContent.ItemType<OneUpMushroom>() || Player.bank4.item[i].type == ModContent.ItemType<OneUpDeluxe>())
                {
                    item = Player.bank4.item[i].type;
                    inventorySlot = i;
                    break;
                }
            }
        }

        int healAmount = Player.statLifeMax2 / (item == ModContent.ItemType<OneUpMushroom>() ? 2 : 1);
        Player.HealEffect(healAmount);
        Player.statLife = healAmount;
        SoundEngine.PlaySound(new($"{nameof(MarioLand)}/Assets/Sounds/Heal") { Volume = 0.5f });

        if (inVoidBag) Player.bank4.item[inventorySlot].stack--;
        else Player.inventory[inventorySlot].stack--;

        return false;
    }
}
