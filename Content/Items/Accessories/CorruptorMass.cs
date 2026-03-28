using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Accessories
{
    public class CorruptorMass : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 40;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<NeutroniumPlayer>().corruptorMass = true;
        }
    }
}