using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures; 
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Accessories
{
    public class CorruptorMass : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 33;
            Item.height = 60;
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