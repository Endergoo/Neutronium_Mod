using System.Collections.Generic;
using System.Linq;
using Neutronium.Content.DamageClasses;
using Neutronium.Content.Items.Weapons;
using Neutronium.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons;

public class UniverseCutterScythe : ModItem
{


    public override void SetDefaults()
    {
        Item.damage = 200;
        Item.width = 68;
        Item.height = 66;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing; // Swinging animation
        Item.noMelee = true; // The item itself doesn't deal melee damage
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 100);
        Item.rare = ItemRarityID.Lime;
        Item.UseSound = SoundID.Item71;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<UniverseCutter>(); // Spawn the UniverseCutter projectile
        Item.shootSpeed = 25f; // Speed of the projectile
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        var lineToChange = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
        if (lineToChange != null)
        {
            string[] split = lineToChange.Text.Split(' ');
            lineToChange.Text = split.First() + " Stellar " + split.Last();
        }
    }
    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage += player.GetModPlayer<GlobalPlayer>().StellarDamage;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 240);
        target.AddBuff(BuffID.Burning, 240);

    }
 
}
