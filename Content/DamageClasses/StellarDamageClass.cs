using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.DamageClasses
{
    public class StellarDamageClass : DamageClass
    {
        // This is an example damage class designed to demonstrate all the current functionality of the feature and explain how to create one of your own, should you need one.
        // For information about how to apply stat bonuses to specific damage classes, please instead refer to ExampleMod/Content/Items/Accessories/ExampleStatBonusAccessory.
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
       
            if (damageClass == DamageClass.Generic)
                return StatInheritanceData.Full;

            return new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
            );
           
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Melee)
                return true;
            if (damageClass == DamageClass.Magic)
                return true;

            return false;
        }

        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<StellarDamageClass>() += 4;
            player.GetArmorPenetration<StellarDamageClass>() += 10;
        }

        public override bool UseStandardCritCalcs => true;

        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            // This method lets you prevent certain common statistical tooltip lines from appearing on items associated with this DamageClass.
            // The four line names you can use are "Damage", "CritChance", "Speed", and "Knockback". All four cases default to true, and thus will be shown. For example...
            if (lineName == "Speed")
                return false;

            return true;
            // PLEASE BE AWARE that this hook will NOT be here forever; only until an upcoming revamp to tooltips as a whole comes around.
            // Once this happens, a better, more versatile explanation of how to pull this off will be showcased, and this hook will be removed.
        }
    }
}