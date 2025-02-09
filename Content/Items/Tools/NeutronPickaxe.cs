using Neutronium.Content.Dusts;
using Neutronium.Content.EmoteBubbles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Tiles;

namespace Neutronium.Content.Items.Tools
{
    public class NeutronPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 1); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.pick = 225; 
            Item.axe = 75;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true; 
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<CustomDrawDust>());
            }
        }

        public override void UseAnimation(Player player)
        {
            // Randomly causes the player to use Example Pickaxe Emote when using the item
            if (Main.myPlayer == player.whoAmI && player.ItemTimeIsZero && Main.rand.NextBool(60))
            {
                EmoteBubble.MakePlayerEmote(player, ModContent.EmoteBubbleType<NeutronPickaxeEmote>());
            }
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe CondensedNeutron = CreateRecipe();
            CondensedNeutron.AddIngredient(ModContent.ItemType<CondensedNeutronium>(), 12);
            CondensedNeutron.AddTile(TileID.AdamantiteForge);
            CondensedNeutron.Register();
        }
    }
}