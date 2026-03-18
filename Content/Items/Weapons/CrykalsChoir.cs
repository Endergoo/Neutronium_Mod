using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem
    {
        // Swing tracking
        private int swingTime = 0;
        private float swingCompletion = 0f;
        private Vector2 bladeTip;
        private bool trailSpawned = false;
        private bool swooshPlayed = false;

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
            Item.UseSound = SoundID.Item1;

            // Don't set Item.shoot — trail is spawned manually
        }

        public override void UseAnimation(Player player)
        {
            swingTime = 0;
            trailSpawned = false;
            swooshPlayed = false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            swingTime++;
            swingCompletion = swingTime / (float)Item.useAnimation;

            int dir = player.direction;

            // Calculate swing rotation (start to end angle)
            float startRot = MathHelper.ToRadians(-90) * dir;
            float endRot = MathHelper.ToRadians(90) * dir;
            player.itemRotation = MathHelper.Lerp(startRot, endRot, swingCompletion);

            // Correct arm position
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            // Blade tip position for hitbox and trail
            bladeTip = player.Center + player.itemRotation.ToRotationVector2() * 60f;

            // Spawn trail once per swing
            if (!trailSpawned && swingCompletion >= 0.25f)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), bladeTip, Vector2.Zero, ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
                trailSpawned = true;
            }

            // Play swoosh sound once
            if (!swooshPlayed && swingCompletion >= 0.1f)
            {
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                swooshPlayed = true;
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            // Use blade tip for hitbox
            float size = 40f; // adjust to taste
            hitbox = new Rectangle((int)(bladeTip.X - size / 2), (int)(bladeTip.Y - size / 2), (int)size, (int)size);
        }
    }
}