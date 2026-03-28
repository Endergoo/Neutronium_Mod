using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures; 
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Accessories
{
    public class CorruptorChunk : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 44;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDamage += 0.05f
            player.lifeRegen -= 2;
            player.lifeRegenTime -= 30;
            
            player.GetModPlayer<NeutroniumPlayer>().corruptorChunk = true;
        }
    }
}