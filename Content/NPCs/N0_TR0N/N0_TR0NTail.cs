using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Neutronium.NPCs.N0_TR0N
{
    public class N0_TR0NTail : ModNPC
    {
        private int invinceTime = 180;

        public static Asset<Texture2D> Phase2Texture;
        public static Asset<Texture2D> GlowTexture;

        public override LocalizedText DisplayName => Language.GetText("Mods.Neutronium.NPCs.N0_TR0NTail.DisplayName");

        // Custom properties to replace Calamity ones
        public float DR = 0.4f;
        public bool VulnerableToElectricity = false;
        public float takenDamageMultiplier = 2f;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "Naked", AssetRequestMode.AsyncLoad);
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 80;
            NPC.lifeMax = 100000; // Adjust balance as needed
            NPC.defense = 50;
            NPC.damage = 80;
            
            // Phase one settings
            takenDamageMultiplier = 2f;
            NPC.HitSound = SoundID.NPCHit53;
            NPC.DeathSound = N0_TR0NHead.DeathSound;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            // Difficulty scaling
            if (Main.masterMode)
                NPC.scale *= 1.25f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 0.7f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(takenDamageMultiplier);
            writer.Write(DR);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            takenDamageMultiplier = reader.ReadSingle();
            DR = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            // Invincibility period after spawning
            if (invinceTime > 0)
            {
                invinceTime--;
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.damage = NPC.defDamage;
                NPC.dontTakeDamage = false;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Sync life with previous segment
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length && Main.npc[(int)NPC.ai[1]].active)
            {
                if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                    NPC.life = Main.npc[(int)NPC.ai[1]].life;
            }

            bool expertMode = Main.expertMode;
            bool masterMode = Main.masterMode;

            // Shed armor at 80% health
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.8f;

            // Update armored settings to naked settings
            if (phase2)
            {
                // Spawn armor gore and set other crucial variables
                if (takenDamageMultiplier == 2f)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        // Create armor break effects
                        for (int i = 0; i < 3; i++)
                        {
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, 
                                GoreID.Smoke1 + Main.rand.Next(3), NPC.scale);
                        }
                    }

                    NPC.defense = 40;
                    DR = 0.4f;
                    takenDamageMultiplier = 1f;
                    NPC.HitSound = SoundID.NPCHit13;
                    
                    if (Main.netMode != NetmodeID.Server)
                        NPC.frame = new Rectangle(0, 0, 42, 68);
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Fire a lightning orb every 5 seconds
                    float spawnOrbGateValue = masterMode ? 200f : 300f;
                    if (NPC.ai[3] % spawnOrbGateValue == 0f)
                    {
                        int type = ProjectileID.CultistBossLightningOrb;
                        int damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer);
                    }
                    NPC.ai[3]++;
                }
            }

            // Check if other segments are still alive
            bool shouldDespawn = !NPC.AnyNPCs(ModContent.NPCType<N0_TR0NHead>());
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            // Fade in effect
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length && Main.npc[(int)NPC.ai[1]].active)
            {
                if (Main.npc[(int)NPC.ai[1]].alpha < 128)
                {
                    if (NPC.alpha != 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int redDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TheDestroyer, 0f, 0f, 100, default, 2f);
                            Main.dust[redDust].noGravity = true;
                            Main.dust[redDust].noLight = true;
                        }
                    }

                    NPC.alpha -= 42;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                }
            }

            // Worm segment following behavior
            Vector2 segmentLocation = NPC.Center;
            float targetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float targetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            targetX = (int)(targetX / 16f) * 16;
            targetY = (int)(targetY / 16f) * 16;
            segmentLocation.X = (int)(segmentLocation.X / 16f) * 16;
            segmentLocation.Y = (int)(segmentLocation.Y / 16f) * 16;
            targetX -= segmentLocation.X;
            targetY -= segmentLocation.Y;

            float targetDistance = (float)System.Math.Sqrt(targetX * targetX + targetY * targetY);
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length && Main.npc[(int)NPC.ai[1]].active)
            {
                try
                {
                    segmentLocation = NPC.Center;
                    targetX = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - segmentLocation.X;
                    targetY = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - segmentLocation.Y;
                }
                catch
                {
                }

                NPC.rotation = (float)System.Math.Atan2(targetY, targetX) + MathHelper.PiOver2;
                targetDistance = (float)System.Math.Sqrt(targetX * targetX + targetY * targetY);
                int npcWidth = NPC.width;
                targetDistance = (targetDistance - npcWidth) / targetDistance;
                targetX *= targetDistance;
                targetY *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + targetX;
                NPC.position.Y = NPC.position.Y + targetY;

                if (targetX < 0f)
                    NPC.spriteDirection = -1;
                else if (targetX > 0f)
                    NPC.spriteDirection = 1;
            }

            // Calculate contact damage based on velocity
            float velocity = (phase2 ? 12f : 10f) + (masterMode ? 3f : expertMode ? 1f : 0f);
            float minimalContactDamageVelocity = velocity * 0.25f;
            float minimalDamageVelocity = velocity * 0.5f;
            float bodyAndTailVelocity = (NPC.position - NPC.oldPosition).Length();
            if (bodyAndTailVelocity <= minimalContactDamageVelocity)
            {
                NPC.damage = 0;
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp(0f, NPC.defDamage, velocityDamageScalar);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool expertMode = Main.expertMode;
            bool masterMode = Main.masterMode;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool phase2 = lifeRatio < 0.8f;
            bool phase3 = lifeRatio < 0.55f;

            // Charge telegraph timing
            float chargePhaseGateValue = masterMode ? 280f : expertMode ? 360f : 400f;
            if (!phase3)
                chargePhaseGateValue *= 0.5f;

            Texture2D texture = phase2 ? Phase2Texture.Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(texture.Width / 2, texture.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color drawColorAlpha = NPC.GetAlpha(drawColor);

            // Simple charge telegraph
            if (NPC.ai[3] % 60 > 45) // Flash every second for last 15 frames
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, 0.5f);

            spriteBatch.Draw(texture, drawLocation, NPC.frame, drawColorAlpha, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            // Glow effect in phase 1
            if (!phase2)
            {
                texture = GlowTexture.Value;
                Color rainbowGlow = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                Color glowColor = Color.Lerp(Color.White, rainbowGlow, 0.5f);
                spriteBatch.Draw(texture, drawLocation, NPC.frame, glowColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

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

            int buffDuration = (NPC.ai[3] % chargePhaseGateValue) >= (chargePhaseGateValue - 120) ? 60 : 30;
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Electrified, buffDuration, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
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

        public override bool CheckActive()
        {
            return false;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            // Implement custom damage reduction and taken damage multiplier
            modifiers.FinalDamage *= (1f - DR);
            modifiers.FinalDamage *= takenDamageMultiplier;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
            NPC.damage = (int)(NPC.damage * balance);
        }
    }
}