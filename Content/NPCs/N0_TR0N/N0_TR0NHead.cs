using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.NPCs.N0_TR0N
{
    public class N0_TR0NHead : ModNPC
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;

        internal static void LoadHeadIcons()
        {
            string normalIconPath = "Neutronium/NPCs/N0_TR0N/N0_TR0NHead_Head_Boss";
            string vulnerableIconPath = "Neutronium/NPCs/N0_TR0N/N0_TR0NHeadNaked_Head_Boss";

            ModContent.GetInstance<Neutronium>().AddBossHeadTexture(normalIconPath, -1);
            normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

            ModContent.GetInstance<Neutronium>().AddBossHeadTexture(vulnerableIconPath, -1);
            vulnerableIconIndex = ModContent.GetModBossHeadSlot(vulnerableIconPath);
        }

        private const float BoltAngleSpread = 280;
        private bool tail = false;

        public static readonly SoundStyle ArmorShedSound = new("Neutronium/Sounds/NPCs/N0TR0NArmorShed");
        public static readonly SoundStyle DeathSound = new("Neutronium/Sounds/NPCs/N0TR0NDeath");

        // Lightning flash variables
        public float lightning = 0f;
        private float lightningDecay = 1f;
        private float lightningSpeed = 0f;

        public static Asset<Texture2D> Phase2Texture;

        // Custom properties to replace Calamity ones
        public float DR = 0.999999f; // Damage Reduction
        public bool unbreakableDR = true;
        public bool chaseable = false;
        public bool VulnerableToElectricity = false;
        public float[] newAI = new float[4];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.85f,
                PortraitScale = 0.75f,
                CustomTexturePath = "Neutronium/ExtraTextures/Bestiary/N0TR0N_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 70;
            value.Position.Y += 55;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "Naked", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 53;
            NPC.height = 88;
            NPC.lifeMax = 100000; // Adjust balance as needed
            NPC.defense = 150;
            NPC.damage = 150;
            NPC.value = Item.buyPrice(2, 0, 0, 0);

            // Phase one settings
            NPC.knockBackResist = 0f;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = DeathSound;

            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;

            // Difficulty scaling
            if (Main.masterMode)
                NPC.scale *= 1.25f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 0.7f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.Neutronium.Bestiary.N0TR0N")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            if (NPC.life / (float)NPC.lifeMax < 0.8f)
                index = vulnerableIconIndex;
            else
                index = normalIconIndex;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(chaseable);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chaseable = reader.ReadBoolean();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode;
            bool masterMode = Main.masterMode;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Shed armor and start charging at the target
            bool phase2 = lifeRatio < 0.8f;

            // Start calling down frost waves from the sky in sheets and stop firing lightning during the charge
            bool phase3 = lifeRatio < 0.55f;

            // Lightning strike flash phase, stop charging and start summoning tornadoes
            bool phase4 = lifeRatio < 0.3f;

            // Update armored settings to naked settings
            if (phase2)
            {
                // Spawn armor gore, roar and set other crucial variables
                if (!chaseable)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        // Create simple armor break effect
                        for (int i = 0; i < 5; i++)
                        {
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 
                                GoreID.Smoke1 + Main.rand.Next(3), NPC.scale);
                        }
                    }

                    SoundEngine.PlaySound(ArmorShedSound, NPC.Center);

                    NPC.defense = 20;
                    DR = 0.2f;
                    unbreakableDR = false;
                    chaseable = true;
                    NPC.HitSound = SoundID.NPCHit13;
                    
                    if (Main.netMode != NetmodeID.Server)
                        NPC.frame = new Rectangle(0, 0, 82, 88);
                }
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 5000f)
                NPC.TargetClosest();

            if (NPC.alpha != 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int redDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TheDestroyer, 0f, 0f, 100, default, 2f);
                    Main.dust[redDust].noGravity = true;
                    Main.dust[redDust].noLight = true;
                }
            }

            NPC.alpha -= 12;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    int totalLength = masterMode ? 40 : expertMode ? 30 : 20;

                    for (int segments = 0; segments < totalLength; segments++)
                    {
                        int newSegment;
                        if (segments >= 0 && segments < totalLength - 1)
                            newSegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<N0_TR0NBody>(), NPC.whoAmI);
                        else
                            newSegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<N0_TR0NTail>(), NPC.whoAmI);

                        Main.npc[newSegment].realLife = NPC.whoAmI;
                        Main.npc[newSegment].ai[2] = NPC.whoAmI;
                        Main.npc[newSegment].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = newSegment;
                        NPC.netUpdate = true;
                        Previous = newSegment;
                    }

                    tail = true;
                }

                // Used for body and tail projectile firing timings in phase 1
                if (!phase2)
                    NPC.localAI[0] += 1f;
            }

            if (NPC.ai[0] > 0f && NPC.ai[0] < Main.npc.Length && Main.npc[(int)NPC.ai[0]].active)
            {
                if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                    NPC.life = Main.npc[(int)NPC.ai[0]].life;
            }

            if (Main.player[NPC.target].dead && NPC.life > 0)
            {
                NPC.localAI[1] = 0f;
                newAI[0] = 0f;
                newAI[2] = 0f;
                NPC.TargetClosest(false);

                NPC.velocity.Y -= 3f;
                if (NPC.position.Y < Main.topWorld + 16f)
                    NPC.velocity.Y -= 3f;

                if (NPC.position.Y < Main.topWorld + 16f)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if ((n.type == ModContent.NPCType<N0_TR0NBody>()
                            || n.type == ModContent.NPCType<N0_TR0NHead>()
                            || n.type == ModContent.NPCType<N0_TR0NTail>()))
                        {
                            n.active = false;
                        }
                    }
                }
            }

            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 10000f && NPC.life > 0)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == ModContent.NPCType<N0_TR0NBody>()
                       || n.type == ModContent.NPCType<N0_TR0NHead>()
                       || n.type == ModContent.NPCType<N0_TR0NTail>())
                    {
                        n.active = false;
                    }
                }
            }

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;

            Vector2 npcCenter = NPC.Center;
            float targetCenterX = Main.player[NPC.target].Center.X;
            float targetCenterY = Main.player[NPC.target].Center.Y;
            float velocity = (phase2 ? 12f : 10f) + (masterMode ? 3f : expertMode ? 1f : 0f);
            float acceleration = (phase2 ? 0.24f : 0.2f) + (masterMode ? 0.12f : expertMode ? 0.04f : 0f);

            // Start charging at the player when in phase 2
            if (phase2)
            {
                if (!phase4)
                {
                    newAI[0] += 1f;
                }
                else
                {
                    NPC.localAI[1] = 0f;
                    if (NPC.localAI[3] > 0f)
                        NPC.localAI[3] -= 1f;

                    newAI[0] = 0f;
                }

                newAI[2] += 1f;

                // Only use tornadoes in phase 4 and swap between using them or the frost waves
                bool useTornadoes = newAI[3] % 2f != 0f;

                // Gate value that decides when N0-TRØN will charge
                float chargePhaseGateValue = masterMode ? 280f : expertMode ? 360f : 400f;
                if (!phase3)
                    chargePhaseGateValue *= 0.5f;
                if (phase4 && expertMode)
                    chargePhaseGateValue *= 0.9f;

                // Gate value for when N0-TRØN fires projectiles
                float projectileGateValue = (int)(chargePhaseGateValue * 0.25f);

                // Call down frost waves from the sky
                if (phase3 && !useTornadoes)
                {
                    if (newAI[2] >= projectileGateValue)
                    {
                        newAI[2] = -projectileGateValue * 4f;

                        // Lightning strike
                        if (!Main.DisableIntenseVisualEffects)
                        {
                            if (Main.netMode != NetmodeID.Server)
                            {
                                if (lightningSpeed == 0f)
                                {
                                    lightningDecay = Main.rand.NextFloat() * 0.05f + 0.008f;
                                    lightningSpeed = Main.rand.NextFloat() * 0.05f + 0.05f;
                                }
                            }
                        }

                        if (phase4)
                            newAI[3] += 1f;

                        // Play a sound as a telegraph
                        SoundEngine.PlaySound(SoundID.Item120, Main.player[NPC.target].Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ProjectileID.FrostWave;
                            int waveDamage = NPC.GetProjectileDamage(type);
                            int totalWaves = masterMode ? 25 : expertMode ? 23 : 20;
                            int shotSpacing = masterMode ? 185 : expertMode ? 200 : 215;
                            float projectileSpawnX = Main.player[NPC.target].Center.X - totalWaves * shotSpacing * 0.5f;

                            int centralWave = totalWaves / 2;
                            float velocityY = 8f;
                            int wavePatternType = expertMode ? Main.rand.Next(2) + 1 : 2;
                            float delayBeforeFiring = -60f;
                            
                            for (int x = 0; x < totalWaves; x++)
                            {
                                switch (wavePatternType)
                                {
                                    case 0:
                                        if (x != 0)
                                        {
                                            if (x <= centralWave)
                                                velocityY -= 1f / 6f;
                                            else
                                                velocityY += 1f / 6f;
                                        }
                                        break;

                                    case 1:
                                        if (x != 0)
                                        {
                                            if (x % 2 == 0)
                                                velocityY += 2f;
                                            else
                                                velocityY -= 2f;
                                        }
                                        break;

                                    case 2:
                                        velocityY = 7f;
                                        break;
                                }

                                // Create telegraph and frost wave projectiles
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnX, Main.player[NPC.target].Center.Y - 1600f, 0f, velocityY * 0.5f, ProjectileID.FrostBlastFriendly, 0, 0f, Main.myPlayer, 0f, velocityY);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnX, Main.player[NPC.target].Center.Y - 1600f, 0f, velocityY * 0.1f, type, waveDamage, 0f, Main.myPlayer, delayBeforeFiring, velocityY);
                                projectileSpawnX += shotSpacing;
                            }
                        }
                    }
                }

                // Summon tornadoes
                if (useTornadoes)
                {
                    if (newAI[2] >= projectileGateValue)
                    {
                        newAI[2] = -projectileGateValue * 4f;
                        newAI[3] += 1f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int projectileType = ProjectileID.CultistBossIceMist;
                            int tornadoDamage = NPC.GetProjectileDamage(projectileType);
                            int totalTornadoes = expertMode ? 5 : 3;
                            float spawnDistance = expertMode ? 900f : 1050f;
                            
                            for (int i = 0; i < totalTornadoes; i++)
                            {
                                Vector2 spawnPosition = Main.player[NPC.target].Center + Vector2.UnitX * spawnDistance * (i - totalTornadoes / 2);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, Vector2.Zero, projectileType, tornadoDamage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                // Charge
                if (!phase4)
                {
                    if (newAI[0] >= chargePhaseGateValue)
                    {
                        NPC.localAI[3] = 60f;

                        if (NPC.localAI[1] == 0f)
                            NPC.localAI[1] = 1f;

                        if (newAI[0] >= chargePhaseGateValue + 100f)
                        {
                            NPC.TargetClosest();
                            NPC.localAI[1] = 0f;
                            newAI[0] = 0f;
                        }

                        if (NPC.localAI[1] == 2f)
                        {
                            velocity += Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.01f * (1f - (lifeRatio / 0.8f));
                            acceleration += Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.0001f * (1f - (lifeRatio / 0.8f));
                            velocity *= 2f;
                            acceleration *= 0.85f;

                            float stopChargeDistance = 800f * NPC.localAI[2];
                            if (stopChargeDistance < 0)
                            {
                                if (NPC.Center.X < Main.player[NPC.target].Center.X + stopChargeDistance)
                                {
                                    NPC.localAI[1] = 0f;
                                    newAI[0] = 0f;
                                }
                            }
                            else
                            {
                                if (NPC.Center.X > Main.player[NPC.target].Center.X + stopChargeDistance)
                                {
                                    NPC.localAI[1] = 0f;
                                    newAI[0] = 0f;
                                }
                            }
                        }

                        int dustAmt = 5;
                        for (int k = 0; k < dustAmt; k++)
                        {
                            Vector2 dustRotation = Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f;
                            dustRotation = dustRotation.RotatedBy((k - (dustAmt / 2 - 1)) * MathHelper.Pi / dustAmt) + NPC.Center;
                            Vector2 randDustMovement = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                            int bluishDust = Dust.NewDust(dustRotation + randDustMovement, 0, 0, DustID.UnusedWhiteBluePurple, randDustMovement.X, randDustMovement.Y, 100, default, 3f);
                            Main.dust[bluishDust].noGravity = true;
                            Main.dust[bluishDust].noLight = true;
                            Main.dust[bluishDust].velocity /= 4f;
                            Main.dust[bluishDust].velocity -= NPC.velocity;
                        }
                    }
                    else
                    {
                        if (NPC.localAI[3] > 0f)
                            NPC.localAI[3] -= 1f;
                    }
                }
            }

            if (Main.getGoodWorld)
            {
                velocity *= 1.4f;
                acceleration *= 1.4f;
            }

            // Movement calculations (simplified from original)
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            NPC.velocity = Vector2.Lerp(NPC.velocity, direction * velocity, acceleration);

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = velocity * 0.25f;
            float minimalDamageVelocity = velocity * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

            if (phase4)
            {
                // Adjust lightning flash variables when in phase 3
                if (Main.netMode != NetmodeID.Server)
                {
                    if (lightningSpeed > 0f)
                    {
                        lightning += lightningSpeed;
                        if (lightning >= 1f)
                        {
                            lightning = 1f;
                            lightningSpeed = 0f;
                        }
                    }
                    else if (lightning > 0f)
                        lightning -= lightningDecay;
                }

                // Thunder sound
                if (lightning == 1f)
                    SoundEngine.PlaySound(SoundID.Thunder, NPC.Center);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool expertMode = Main.expertMode;
            bool masterMode = Main.masterMode;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool phase2 = lifeRatio < 0.8f;
            bool phase3 = lifeRatio < 0.55f;

            // Gate value that decides when N0-TRØN will charge
            float chargePhaseGateValue = masterMode ? 280f : expertMode ? 360f : 400f;
            if (!phase3)
                chargePhaseGateValue *= 0.5f;

            Texture2D texture = phase2 ? Phase2Texture.Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(texture.Width / 2, texture.Height / 2);
            float chargeTelegraphTime = 120f;
            float chargeTelegraphGateValue = chargePhaseGateValue - chargeTelegraphTime;

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color drawColorAlpha = NPC.GetAlpha(drawColor);

            if (newAI[0] > chargeTelegraphGateValue)
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, MathHelper.Clamp((newAI[0] - chargeTelegraphGateValue) / chargeTelegraphTime, 0f, 1f));
            else if (NPC.localAI[3] > 0f)
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, MathHelper.Clamp(NPC.localAI[3] / 60f, 0f, 1f));

            spriteBatch.Draw(texture, drawLocation, NPC.frame, drawColorAlpha, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            bool expertMode = Main.expertMode;
            bool masterMode = Main.masterMode;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool phase3 = lifeRatio < 0.55f;

            float chargePhaseGateValue = masterMode ? 280f : expertMode ? 360f : 400f;
            if (!phase3)
                chargePhaseGateValue *= 0.5f;

            int buffDuration = newAI[0] >= chargePhaseGateValue ? 240 : 120;
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Electrified, buffDuration, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    // Create death effects
                    for (int i = 0; i < 3; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 
                            GoreID.Smoke1 + Main.rand.Next(3), NPC.scale);
                    }
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(50 * NPC.scale);
                NPC.height = (int)(50 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 20; i++)
                {
                    int electricDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 2f);
                    Main.dust[electricDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[electricDust].scale = 0.5f;
                        Main.dust[electricDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 40; j++)
                {
                    int electricDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 3f);
                    Main.dust[electricDust2].noGravity = true;
                    Main.dust[electricDust2].velocity *= 5f;
                    electricDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 2f);
                    Main.dust[electricDust2].velocity *= 2f;
                }
            }
        }

        public override bool CheckDead()
        {
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && (Main.npc[k].type == ModContent.NPCType<N0_TR0NBody>() || Main.npc[k].type == ModContent.NPCType<N0_TR0NTail>()))
                    Main.npc[k].life = 0;
            }

            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            // Set boss as defeated (you can implement your own downed system if needed)
            // YourDownedSystem.downedN0TR0N = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Basic loot - adjust as needed
            npcLoot.Add(ItemDropRule.Common(ItemID.HallowedBar, 1, 15, 25));
            npcLoot.Add(ItemDropRule.Common(ItemID.SoulofMight, 1, 20, 30));
            
            // Expert/Master mode drops
            LeadingConditionRule expertRule = new LeadingConditionRule(new Conditions.IsExpert());
            expertRule.OnSuccess(ItemDropRule.Common(ItemID.WormScarf, 4)); // Example expert drop
            
            npcLoot.Add(expertRule);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            // Implement custom damage reduction
            if (unbreakableDR)
            {
                modifiers.FinalDamage *= (1f - DR);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
            NPC.damage = (int)(NPC.damage * balance);
        }
    }
}