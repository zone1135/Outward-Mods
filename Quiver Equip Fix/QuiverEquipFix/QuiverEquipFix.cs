﻿using BepInEx;
using HarmonyLib;
using System;

namespace QuiverEquipFix
{
    [BepInPlugin(ID, NAME, VERSION)]
    public class QuiverEquipFix : BaseUnityPlugin
    {
        private const string ID = "com.Zalamaur.QuiverEquipFix";
        private const string NAME = "Quiver Equip Fix";
        private const string VERSION = "1.2";
        public static Harmony harmony;
        public static QuiverEquipFix self;

        public void Awake()
        {
            try
            {
                self = this;
                Logger.LogDebug("Creating Harmony instance...");
                harmony = new Harmony("com.Zalamaur.QuiverEquipFix");

                Logger.LogDebug("Harmony patching initiating...");
                harmony.PatchAll();
                Logger.LogDebug("Harmony patching complete.");
            }
            catch (Exception e)
            {
                Logger.LogFatal(e.Message);
                throw e;
            }
        }

        [HarmonyPatch(typeof(CharacterInventory), "TakeItem", new Type[] { typeof(Item), typeof(bool) })]
        public class CharacterInventory_TakeItem
        {
            [HarmonyPostfix]
            public static void Postfix(Item takenItem, CharacterEquipment ___m_characterEquipment)
            {
                try
                {
                    if (!(takenItem is null) && !(takenItem is Equipment))
                    {
                        QuiverEquipFix.self.Logger.LogDebug(takenItem);
                    }
                    if (takenItem && takenItem is Equipment)
                    {
                        QuiverEquipFix.self.Logger.LogDebug(takenItem);
                        QuiverEquipFix.self.Logger.LogDebug("Running 'CharacterInventory_TakeItem' Postfix");
                        Equipment equipment = (Equipment)takenItem;
                        if ((equipment.EquipSlot == EquipmentSlot.EquipmentSlotIDs.Quiver)
                            && ___m_characterEquipment.GetEquippedAmmunition()
                            && (___m_characterEquipment.GetEquippedAmmunition().ItemID
                            != takenItem.ItemID))
                        {
                            ___m_characterEquipment.EquipItem(equipment, false);
                        }
                    }else if (takenItem is null)
                    {
                        QuiverEquipFix.self.Logger.LogDebug("takenItem is null");
                    }
                }
                catch (Exception e)
                {
                    QuiverEquipFix.self.Logger.LogFatal(e.Message);
                    throw e;
                }
            }
        }

        [HarmonyPatch(typeof(ItemDisplay), "TryEquipUnequip")]
        public class ItemDisplay_TryEquipUnequip
        {
            [HarmonyPostfix]
            public static void Postfix(ItemDisplay __instance, bool __result)
            {
                try
                {
                    QuiverEquipFix.self.Logger.LogDebug("Running 'ItemDisplay_TryEquipUnequip' Postfix");
                    if (__result && !(__instance.RefItem.IsEquipped))
                    {
                        if (!__instance.RefItem.IsEquipped)
                        {
                            if (!(__instance.RefItem is Bag) && (__instance.RefItem is Ammunition))
                            {
                                Item equippedAmmunition = __instance.LocalCharacter.Inventory.GetEquippedAmmunition();
                                if (!(equippedAmmunition is null) && equippedAmmunition.ItemID != __instance.RefItem.ItemID)
                                {
                                    __instance.LocalCharacter.Inventory.EquipItem((Equipment)__instance.RefItem, true);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    QuiverEquipFix.self.Logger.LogFatal(e.Message);
                    throw e;
                }
            }
        }
    }
}