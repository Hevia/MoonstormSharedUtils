﻿using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RoR2.CombatDirector;

namespace Moonstorm
{
    /// <summary>
    /// The <see cref="EliteTierDefModuleBase"/> is a <see cref="ModuleBase{T}{T}"/> that handles the <see cref="EliteTierDefBase"/> class
    /// <para><see cref="EliteTierDefModuleBase"/>'s main job is to handle the proper addition of EliteTierDefs from <see cref="EliteTierDefBase"/> inheriting classes</para>
    /// <para><see cref="EliteTierDefModuleBase"/> will automatically handle the addition of new EliteTiers by using <see cref="R2API.EliteAPI"/></para>
    /// <para>Inherit from this module if you want to load and manage EliteTiers with <see cref="EliteTierDefBase"/> systems</para>
    /// </summary>
    public abstract class EliteTierDefModuleBase : ModuleBase<EliteTierDefBase>
    {
        #region Properties and Fields
        /// <summary>
        /// A ReadOnlyDictionary that can be used for loading a specific <see cref="EliteTierDefBase"/> by giving it's tied <see cref="SerializableEliteTierDef"/>
        /// <para>If you want to modify classes inside this, subscribe to <see cref="OnDictionaryCreated"/> to ensure the dictionary is not empty</para>
        /// </summary>
        public static ReadOnlyDictionary<SerializableEliteTierDef, EliteTierDefBase> MoonstormEliteTiers { get; private set; }
        internal static Dictionary<SerializableEliteTierDef, EliteTierDefBase> eliteTierDefs = new Dictionary<SerializableEliteTierDef, EliteTierDefBase>();
        
        /// <summary>
        /// Returns all the SerializableEliteTierDefs from <see cref="MoonstormEliteTiers"/>
        /// </summary>
        public static SerializableEliteTierDef[] LoadedEliteTierDefs { get => MoonstormEliteTiers.Keys.ToArray(); }
        /// <summary>
        /// An action that gets invoked when the <see cref="MoonstormEliteTiers"/> dictionary has been populated
        /// </summary>
        public static Action<ReadOnlyDictionary<SerializableEliteTierDef, EliteTierDefBase>> OnDictionaryCreated;
        #endregion

        [SystemInitializer]
        private static void SystemInit()
        {
            AddressableAssets.AddressableAsset.OnAddressableAssetsLoaded += () =>
            {
                MSULog.Info($"Initializing EliteTierDef Module...");

                CreateAndAddTiers();

                MoonstormEliteTiers = new ReadOnlyDictionary<SerializableEliteTierDef, EliteTierDefBase>(eliteTierDefs);
                eliteTierDefs = null;

                OnDictionaryCreated?.Invoke(MoonstormEliteTiers);
            };
        }

        private static void CreateAndAddTiers()
        {
            foreach(var eliteTierDefBase in eliteTierDefs.Values)
            {
                SerializableEliteTierDef serializableEliteTierDef = eliteTierDefBase.SerializableEliteTierDef;
                serializableEliteTierDef.Create();
                serializableEliteTierDef.EliteTierDef.isAvailable = eliteTierDefBase.IsAvailable;

                R2API.EliteAPI.AddCustomEliteTier(serializableEliteTierDef.EliteTierDef);
            }
        }

        #region EliteTierDefs
        /// <summary>
        /// <inheritdoc cref="ModuleBase{T}.GetContentClasses{T}(Type)"/>
        /// <para>T in this case is <see cref="EliteTierDefBase"/></para>
        /// </summary>
        /// <returns>An IEnumerable of all your assembly's <see cref="EliteTierDefBase"/></returns>
        protected virtual IEnumerable<EliteTierDefBase> GetEliteTierDefBases()
        {
            MSULog.Debug($"Getting the EliteTierDefs found inside {GetType().Assembly}...");
            return GetContentClasses<EliteTierDefBase>();
        }

        /// <summary>
        /// Adds an EliteTierDef to the game
        /// </summary>
        /// <param name="eliteTierDef">The EliteTierDefBase being added</param>
        /// <param name="dictionary">Optional, a dictionary to add your initialized EliteTierDef and EliteTierDefBase</param>
        protected void AddEliteTierDef(EliteTierDefBase eliteTierDef, Dictionary<SerializableEliteTierDef, EliteTierDefBase> dictionary = null)
        {
            InitializeContent(eliteTierDef);
            dictionary?.Add(eliteTierDef.SerializableEliteTierDef, eliteTierDef);
            MSULog.Debug($"EliteTierDef {eliteTierDef.SerializableEliteTierDef} added to the game");
        }

        /// <summary>
        /// Adds the <see cref="EliteTierDef"/> from <paramref name="contentClass"/> to the game's CombatDirector using <see cref="R2API.EliteAPI"/>
        /// <para>Once added, it'll call <see cref="ContentBase.Initialize"/></para>
        /// </summary>
        /// <param name="contentClass">The content class being initialized</param>
        protected override void InitializeContent(EliteTierDefBase contentClass)
        {
            contentClass.Initialize();
            eliteTierDefs[contentClass.SerializableEliteTierDef] = contentClass;
        }
        #endregion
    }
}
