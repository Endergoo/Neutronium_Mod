using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Neutronium.Content.Items.Armor;
using Neutronium.Content.Tiles;


namespace Neutronium.Content.Items.Placeables
{
    internal class CondensedNeutronium : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 59;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = Item.buyPrice(silver: 1, copper: 75);

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Pink;

            Item.createTile = ModContent.TileType<Tiles.CondensedNeutroniumBar>();
            Item.placeStyle = 0;
        }
        public override void AddRecipes()
        {
            Recipe CondensedNeutronium = CreateRecipe();
            CondensedNeutronium.AddIngredient<NeutroniumOre>(4);
            CondensedNeutronium.AddTile(TileID.AdamantiteForge);
            CondensedNeutronium.Register();
        }
    }
}


