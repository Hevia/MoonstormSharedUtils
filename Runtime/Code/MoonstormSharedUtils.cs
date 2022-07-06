﻿using BepInEx;
using Moonstorm.Utilities;
using R2API;
using R2API.ScriptableObjects;
using R2API.Utils;
using RoR2.ContentManagement;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Moonstorm
{
    /// <summary>
    /// The main class of MSU
    /// </summary>
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("iHarbHD.DebugToolkit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [R2APISubmoduleDependency(new string[]
        {
          nameof(ArtifactCodeAPI),
          nameof(DamageAPI),
          nameof(RecalculateStatsAPI),
          nameof(DirectorAPI),
          nameof(CommandHelper),
          nameof(EliteAPI)
        })]
    public class MoonstormSharedUtils : BaseUnityPlugin
    {
        public const string GUID = "com.TeamMoonstorm.MoonstormSharedUtils";
        public const string MODNAME = "Moonstorm Shared Utils";
        public const string VERSION = "1.0.0";

        /// <summary>
        /// Instance of MSU
        /// </summary>
        public static MoonstormSharedUtils Instance { get; private set; }
        /// <summary>
        /// MSU's PluginInfo
        /// </summary>
        public static PluginInfo PluginInfo { get; private set; }
        /// <summary>
        /// The main AssetBundle of MSU
        /// </summary>
        public static AssetBundle MSUAssetBundle { get; private set; }
        private static R2APISerializableContentPack MSUSerializableContentPack { get; set; }
        private static string AssemblyDir { get => Path.Combine(Path.GetDirectoryName(PluginInfo.Location), "assetbundles"); }

        private void Awake()
        {
            Instance = this;
            PluginInfo = Info;
            new MSULog(Logger);
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
            new MSUConfig().Init();
            if (MSUConfig.enableDebugFeatures.Value)
            {
                gameObject.AddComponent<MSUDebug>();
            }
            MSUAssetBundle = AssetBundle.LoadFromFile(Path.Combine(AssemblyDir, "msuassets"));
            R2API.ContentManagement.R2APIContentManager.AddPreExistingSerializableContentPack(MSUAssetBundle.LoadAsset<R2APISerializableContentPack>("MSUSCP"));
            //Events.Init();
        }
    }
}
