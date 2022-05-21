﻿/*using RoR2;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Moonstorm.Components
{
    [RequireComponent(typeof(CharacterBody))]
    public class MoonstormItemManager : MonoBehaviour
    {
        private CharacterBody body;
        private MoonstormEliteBehavior eliteBehavior;
        IStatItemBehavior[] statItemBehaviors = Array.Empty<IStatItemBehavior>();
        IBodyStatArgModifier[] bodyStatArgModifiers = Array.Empty<IBodyStatArgModifier>();

        public ManagerExtension[] managerExtensions = Array.Empty<ManagerExtension>();

        void Awake()
        {
            body = gameObject.GetComponent<CharacterBody>();
            eliteBehavior = body.gameObject.AddComponent<MoonstormEliteBehavior>();
            eliteBehavior.body = body;
            body.onInventoryChanged += CheckForItems;
        }

        public void CheckForItems()
        {
            //It seems counter-intuitive to add an item behavior for something even if it has none of them, but the game actually destroys the behavior if there isn't one which is what we want and it doesn't add a component if it doesn't have any of the item
            foreach (var item in PickupModuleBase.MoonstormItems)
            {
                item.Value.AddBehavior(ref body, body.inventory.GetItemCount(item.Key.itemIndex));
            }
            if (IsMSElite())
            {
                if (eliteBehavior.model)
                {
                    eliteBehavior.model.UpdateOverlays(); //<-- not updating this will cause model.myEliteIndex to not be accurate.
                    body.RecalculateStats(); //<-- not updating recalcstats will cause isElite to be false IF it wasnt an elite before.
                    foreach (var eliteDef in EliteModuleBase.MoonstormElites)
                    {
                        if (body.isElite && eliteBehavior.model.myEliteIndex == eliteDef.eliteIndex)
                        {
                            eliteBehavior.SetNewElite(eliteDef);
                        }
                    }
                }
            }
            else
            {
                eliteBehavior.SetNewElite(null);
            }
            foreach (var equipment in PickupModuleBase.MoonstormEquipments)
            {
                //This change broke me but it fixes a fucking error that points to MSUtil being in the Moonstorm.Utilities namespace despite being on Moonstorm namespace, WTF?
                if (body.inventory?.GetEquipmentIndex() == equipment.Key.equipmentIndex)
                {
                    equipment.Value.AddBehavior(ref body, 1);
                }
            }

            foreach (var extension in managerExtensions)
                extension.CheckForItems();

            StartCoroutine(GetInterfaces());
        }

        public void CheckForBuffs()
        {
            foreach (var buffRef in BuffModuleBase.MoonstormBuffs)
            {
                buffRef.Value.AddBehavior(ref body, body.GetBuffCount(buffRef.Key));
            }

            foreach (var extension in managerExtensions)
                extension.CheckForBuffs();

            StartCoroutine(GetInterfaces());
        }

        //Neb: This really, REALLY shouldnt be called using an invoke, but i cant
        //figure out another way to make sure this method doesnt grab the interfaces from destroyed item behaviors.
        private IEnumerator GetInterfaces()
        {
            yield return new WaitForEndOfFrame();
            statItemBehaviors = GetComponents<IStatItemBehavior>();
            bodyStatArgModifiers = GetComponents<IBodyStatArgModifier>();
            body.healthComponent.onIncomingDamageReceivers = GetComponents<RoR2.IOnIncomingDamageServerReceiver>();
            body.healthComponent.onTakeDamageReceivers = GetComponents<IOnTakeDamageServerReceiver>();

            foreach (var extension in managerExtensions)
                extension.GetInterfaces();
        }

        public bool IsMSElite()
        {
            foreach (var eliteEqp in PickupModuleBase.MoonstormEliteEquipments)
            {
                if (body.inventory?.GetEquipmentIndex() == eliteEqp.Key.equipmentIndex)
                {
                    return true;
                }
            }
            return false;
        }

        public void RunStatRecalculationsStart()
        {
            foreach (var statBehavior in statItemBehaviors)
                statBehavior.RecalculateStatsStart();
        }
        public void RunStatRecalculationsEnd()
        {
            foreach (var statBehavior in statItemBehaviors)
                statBehavior.RecalculateStatsEnd();
        }

        public void RunStatHookEventModifiers(R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            foreach (var statModifier in bodyStatArgModifiers)
            {
                statModifier.ModifyStatArguments(args);
            }
        }

        public T AddManagerExtension<T>() where T : ManagerExtension
        {
            T extension = gameObject.GetComponent<T>();
            if (!extension)
            {
                extension = gameObject.AddComponent<T>();

                extension.body = body;
                extension.manager = this;

                if (!managerExtensions.Contains(extension))
                    HG.ArrayUtils.ArrayAppend(ref managerExtensions, extension);

                extension.SetIndex(Array.IndexOf(managerExtensions, extension));

                return extension;
            }
            return extension;
        }
    }
}*/