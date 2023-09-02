﻿using System.Diagnostics;
using System.Reflection;
using AdvancedTerrainGrass;
using Construction;
using Il2CppInterop.Runtime.Injection;
using RedLoader;
using RedLoader.Utils;
using Sons.Gui;
using Sons.Save;
using SonsSdk;
using SonsSdk.Attributes;
using TheForest;
using TheForest.Utils;
using UnityEngine;
using Color = System.Drawing.Color;

namespace SonsGameManager;

using static SUI.SUI;

public class Core : SonsMod
{
    internal static Core Instance;

    internal static HarmonyLib.Harmony HarmonyInst => Instance.HarmonyInstance;

    public Core()
    {
        Instance = this;
    }

    protected override void OnInitializeMod()
    {
        Config.Load();
        GraphicsCustomizer.Load();
        GamePatches.Init();
    }
    
    protected override void OnSdkInitialized()
    {
        if(Config.ShouldLoadIntoMain)
        {
            LoadIntoMainHandler.DelayedSceneLoad().RunCoro();
            return;
        }

        LoadIntoMainHandler.GlobalOverlay.SetActive(false);

        ModManagerUi.Create();

        LoadTests();

        if (!string.IsNullOrEmpty(Config.LoadSaveGame) && int.TryParse(Config.LoadSaveGame, out var id))
        {
            Resources.FindObjectsOfTypeAll<LoadMenu>()
                .First(x=>x.name=="SinglePlayerLoadPanel")
                .LoadSlotActivated(id, SaveGameManager.GetData<GameStateData>(SaveGameType.SinglePlayer, id));
        }
    }

    [Conditional("DEBUG")]
    private void LoadTests()
    {
        //SuiTest.Create();
        //SoundTests.Init();
        //new DebugGizmoTest();
    }

    protected override void OnGameStart()
    {
        Log("======= GAME STARTED ========");

        // -- Enable debug console --
        DebugConsole.Instance.enabled = true;
        DebugConsole.SetCheatsAllowed(true);
        DebugConsole.Instance.SetBlockConsole(false);
        
        // -- Set player speed --
        if (Config.ShouldLoadIntoMain)
        {
            LocalPlayer.FpCharacter.SetWalkSpeed(LocalPlayer.FpCharacter.WalkSpeed * Config.PlayerDebugSpeed.Value);
            LocalPlayer.FpCharacter.SetRunSpeed(LocalPlayer.FpCharacter.RunSpeed * Config.PlayerDebugSpeed.Value);
        }
        
        // -- Skip Placing Animations --
        if (Config.SkipBuildingAnimations.Value && RepositioningUtils.Manager)
        {
            RepositioningUtils.Manager.SetSkipPlaceAnimations(true);
        }

        // -- Enable Bow Trajectory --
        if (Config.EnableBowTrajectory.Value)
        {
            BowTrajectory.Init();
        }
        
        GraphicsCustomizer.Apply();
    }

    protected override void OnSonsSceneInitialized(SdkEvents.ESonsScene sonsScene)
    {
        TogglePanel(ModManagerUi.MOD_INDICATOR_ID, sonsScene == SdkEvents.ESonsScene.Title);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(Config.ToggleConsoleKey.Value))
        {
            RConsole.ToggleConsole();
        }
    }

    #region DebugCommands

    /// <summary>
    /// Toggles the visibility of grass
    /// </summary>
    /// <param name="args"></param>
    /// <command>togglegrass [on/off]</command>
    [DebugCommand("togglegrass")]
    private void ToggleGrassCommand(string args)
    {
        if (!GrassManager._instance)
            return;

        if (string.IsNullOrEmpty(args))
        {
            SonsTools.ShowMessage("Usage: togglegrass [on/off]");
            return;
        }
        
        GrassManager._instance.DoRenderGrass = args == "on";
    }
    
    /// <summary>
    /// Adjusts the grass density and visibility distance
    /// </summary>
    /// <param name="args"></param>
    /// <command>grass [density] [distance]</command>
    /// <example>grass 0.5 120</example>
    [DebugCommand("grass")]
    private void GrassCommand(string args)
    {
        var parts = args.Split(' ').Select(float.Parse).ToArray();
        
        if (parts.Length != 2)
        {
            SonsTools.ShowMessage("Usage: grass [density] [distance]");
            return;
        }
        
        GraphicsCustomizer.SetGrassSettings(parts[0], parts[1]);
    }

    #endregion
}