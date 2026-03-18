using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Players;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem
    {
        private int swingCount = 0;
        private Vector2 bladeTip;
        private bool trailSpawned;
        private bool playSound;
        private float completion;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UseAnimation(Player player)
        {
            swingCount++;
            bladeTip = player.Center;
            trailSpawned = false;
            playSound = true;
            completion = 0f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            var nPlayer = player.GetModPlayer<NeutroniumPlayer>();
            nPlayer.UpdateSwordSwing(player, Item.useAnimation);
            bladeTip = nPlayer.bladeTip;

            completion = nPlayer.swingCompletion;

            int dir = player.direction;

            // Spawn trail mid-swing
            if (!trailSpawned && completion >= 0.25f)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), bladeTip, Vector2.Zero,
                    ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
                trailSpawned = true;
            }

            // Spawn spark particles along blade mid-swing
            if (completion >= 0.35f && completion <= 0.8f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(bladeTip + Main.rand.NextVector2Circular(2f, 2f), DustID.Electric, 
                        Vector2.Zero, 150, Color.Cyan, 1.2f);
                    d.noGravity = true;
                }
            }

            // Play swoosh sound once per swing
            if (playSound)
            {
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                playSound = false;
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            float size = 40f;
            hitbox = new Rectangle((int)(bladeTip.X - size / 2), (int)(bladeTip.Y - size / 2),
                                    (int)size, (int)size);
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            Vector2 shootDir = player.Center.DirectionTo(Main.MouseWorld);
            float _ = float.NaN;
            bool hitCheck = Collision.CheckAABBvLineCollision(
                target.Hitbox.TopLeft(), target.Hitbox.Size(),
                player.Center - shootDir * 30f,
                player.Center + shootDir * 100f,
                Item.width * 3f, ref _);

            return (completion >= 0.35f && completion <= 0.8f && hitCheck) ? null : false;
        }

        // Glowmask overlay
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Neutronium/Content/Items/Weapons/CrykalsChoir_Glow").Value;
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, glow);
        }
    }
}