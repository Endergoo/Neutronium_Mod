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
        private int swingTime;           // counts ticks of the current swing
        private bool trailSpawned;       // spawn trail once per swing
        private Vector2 bladeTip;        // tip of the sword for hitbox/trail

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
            // reset swing at the start
            swingTime = 0;
            trailSpawned = false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            swingTime++;
            float completion = swingTime / (float)Item.useAnimation;

            // 1️⃣ Get current mouse position
            Vector2 mPos = Main.MouseWorld;

            // 2️⃣ Update player direction to face mouse
            int dir = -Math.Sign(player.Center.X - mPos.X);
            player.direction = dir;

            // 3️⃣ Calculate vector toward mouse
            Vector2 mouseDir = player.Center.DirectionTo(mPos);

            // 4️⃣ Smooth swing rotation relative to mouse
            float startRot = MathHelper.ToRadians(-110) * dir;
            float endRot = MathHelper.ToRadians(110) * dir;
            player.itemRotation = mouseDir.ToRotation() + MathHelper.Lerp(startRot, endRot, completion);

            // 5️⃣ Correct player arm positions
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            // 6️⃣ Sword is drawn at player center
            player.itemLocation = player.Center;

            // 7️⃣ Blade tip position for trail and hitbox
            bladeTip = player.Center + player.itemRotation.ToRotationVector2() * 60f;

            // 8️⃣ Spawn trail once mid-swing
            if (!trailSpawned && completion >= 0.25f)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), bladeTip, Vector2.Zero,
                    ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
                trailSpawned = true;

                // optional swoosh sound
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            float size = 40f;
            hitbox = new Rectangle((int)(bladeTip.X - size / 2), (int)(bladeTip.Y - size / 2),
                                    (int)size, (int)size);
        }
    }
}