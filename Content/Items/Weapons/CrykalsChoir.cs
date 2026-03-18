using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem
    {
        // Swing tracking
        private int swingTime;
        private bool trailSpawned;
        private Vector2 bladeTip;

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
        }

        public override void UseAnimation(Player player)
        {
            swingTime = 0;
            trailSpawned = false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            swingTime++;
            float completion = swingTime / (float)Item.useAnimation; // 0 → 1

            int dir = player.direction; // left/right

            // Start and end angles for swing (adjust as needed)
            float startRot = MathHelper.ToRadians(-90) * dir;
            float endRot = MathHelper.ToRadians(90) * dir;

            // Smooth rotation
            player.itemRotation = MathHelper.Lerp(startRot, endRot, completion);

            // Correct player arm positions
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            // Sword position
            player.itemLocation = player.Center;

            // Blade tip for trail and hitbox
            bladeTip = player.Center + player.itemRotation.ToRotationVector2() * 60f;

            // Spawn trail mid-swing once
            if (!trailSpawned && completion >= 0.25f)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), bladeTip, Vector2.Zero,
                    ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
                trailSpawned = true;
            }

            // Optional: play swoosh sound once
            if (completion >= 0.1f && !trailSpawned) // reuse trailSpawned as flag
            {
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            float size = 40f; // width/height of hitbox
            hitbox = new Rectangle((int)(bladeTip.X - size / 2), (int)(bladeTip.Y - size / 2), (int)size, (int)size);
        }
    }
}