using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using ImGuiNET;

namespace Noturnal_Cheat_External
{
    public class ConfigManager
    {
        private readonly string configFolder;
        private readonly float saveTimer = 2.0f; // seconds
        private Dictionary<string, float> saveNotificationTimers = new();

        private bool showConfigNameInput = false;
        private string newConfigName = "";
        private string currentConfigName = "";

        public ConfigManager()
        {
            configFolder = Path.Combine(AppContext.BaseDirectory, "Configs");
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);
        }

        public void Render(ref ConfigCreator config,
                           ref bool enableBoxes,
                           ref bool enableLines,
                           ref bool enableArrow,
                           ref bool enableSkeleton,
                           ref bool enableHealthBar,
                           ref bool enableArmorBar,
                           ref bool enablePlayerNames,
                           ref bool enableWeaponNames,
                           ref bool enableSkeletonColorSeen,
                           ref bool enableAimbot,
                           ref bool enableCircleAimbot,
                           ref bool enableAimbotSpotted,
                           ref bool enableTriggerBot,
                           ref bool enableBunnyHop,
                           ref bool enableAntiRecoil,
                           ref bool enableAntiFlash,
                           ref bool boxOutline,
                           ref bool healthOutline,
                           ref bool armorOutline,
                           ref bool noSniperScope,
                           ref bool noScopeCrosshair,
                           ref int fov,
                           ref int circleSize,
                           ref int aimbotSmoothing,
                           ref int selectedHealth,
                           ref int selectedSorting,
                           ref Vector4 boxColor,
                           ref Vector4 lineColor,
                           ref Vector4 spottedColor,
                           ref Vector4 arrowColor,
                           ref Vector4 skeletonColor,
                           ref Vector4 circleColor,
                           ref Vector4 weaponNameColor,
                           ref Vector4 nameColor)
        {
            DrawCreateConfigUI(ref config,
                               enableBoxes, enableLines, enableArrow, enableSkeleton,
                               enableHealthBar, enableArmorBar, enablePlayerNames, enableWeaponNames,
                               enableSkeletonColorSeen, enableAimbot, enableCircleAimbot,
                               enableAimbotSpotted, enableTriggerBot, enableBunnyHop,
                               enableAntiRecoil, enableAntiFlash,
                               boxOutline, healthOutline, armorOutline, noSniperScope, noScopeCrosshair,
                               fov, circleSize, aimbotSmoothing, selectedHealth, selectedSorting,
                               boxColor, lineColor, spottedColor, arrowColor, skeletonColor,
                               circleColor, weaponNameColor, nameColor);

            ImGui.Spacing();
            ImGui.Spacing();

            DrawConfigList(ref config,
                           ref enableBoxes, ref enableLines, ref enableArrow, ref enableSkeleton,
                           ref enableHealthBar, ref enableArmorBar, ref enablePlayerNames, ref enableWeaponNames,
                           ref enableSkeletonColorSeen, ref enableAimbot, ref enableCircleAimbot,
                           ref enableAimbotSpotted, ref enableTriggerBot, ref enableBunnyHop,
                           ref enableAntiRecoil, ref enableAntiFlash,
                           ref boxOutline, ref healthOutline, ref armorOutline, ref noSniperScope, ref noScopeCrosshair,
                           ref fov, ref circleSize, ref aimbotSmoothing, ref selectedHealth, ref selectedSorting,
                           ref boxColor, ref lineColor, ref spottedColor, ref arrowColor, ref skeletonColor,
                           ref circleColor, ref weaponNameColor, ref nameColor);
        }

        private void DrawCreateConfigUI(ref ConfigCreator config,
                                        bool enableBoxes, bool enableLines, bool enableArrow, bool enableSkeleton,
                                        bool enableHealthBar, bool enableArmorBar, bool enablePlayerNames, bool enableWeaponNames,
                                        bool enableSkeletonColorSeen, bool enableAimbot, bool enableCircleAimbot,
                                        bool enableAimbotSpotted, bool enableTriggerBot, bool enableBunnyHop,
                                        bool enableAntiRecoil, bool enableAntiFlash,
                                        bool boxOutline, bool healthOutline, bool armorOutline, bool noSniperScope, bool noScopeCrosshair,
                                        int fov, int circleSize, int aimbotSmoothing, int selectedHealth, int selectedSorting,
                                        Vector4 enemyColor, Vector4 lineColor, Vector4 spottedColor, Vector4 arrowColor, Vector4 skeletonColor,
                                        Vector4 circleColor, Vector4 weaponNameColor, Vector4 nameColor)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.4f, 0.1f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0f, 0.7f, 0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.1f, 0.2f, 0.1f, 1.0f));

            if (ImGui.Button("Create New Config"))
            {
                showConfigNameInput = true;
                newConfigName = "";
            }
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("Creates a new config with your currently selected settings");

            ImGui.PopStyleColor(3);

            if (showConfigNameInput)
            {
                ImGui.SetNextItemWidth(160);
                ImGui.InputText("Name", ref newConfigName, 32);

                ImGui.SameLine();
                if (ImGui.Button("Enter"))
                {
                    if (!string.IsNullOrWhiteSpace(newConfigName))
                    {
                        SaveConfig(newConfigName, new ConfigCreator
                        {
                            Boxes = enableBoxes,
                            Lines = enableLines,
                            Arrow = enableArrow,
                            Skeleton = enableSkeleton,
                            HealthBar = enableHealthBar,
                            ArmorBar = enableArmorBar,
                            Names = enablePlayerNames,
                            WeaponNames = enableWeaponNames,
                            SkeletonColorSeen = enableSkeletonColorSeen,
                            Aimbot = enableAimbot,
                            CircleAimbot = enableCircleAimbot,
                            AimbotSpotted = enableAimbotSpotted,
                            TriggerBot = enableTriggerBot,
                            BunnyHop = enableBunnyHop,
                            AntiRecoil = enableAntiRecoil,
                            AntiFlash = enableAntiFlash,
                            BoxOutline = boxOutline,
                            HealthOutline = healthOutline,
                            ArmorOutline = armorOutline,
                            NoSniperScope = noSniperScope,
                            NoScopeCrosshair = noScopeCrosshair,
                            fov = fov,
                            circleSize = circleSize,
                            aimbotSmoothing = aimbotSmoothing,
                            BoxColor = enemyColor,
                            LineColor = lineColor,
                            SpottedColor = spottedColor,
                            ArrowColor = arrowColor,
                            SkeletonColor = skeletonColor,
                            CircleColor = circleColor,
                            WeaponNameColor = weaponNameColor,
                            NameColor = nameColor,
                        });

                        showConfigNameInput = false;
                        newConfigName = "";
                    }
                    else
                        Console.WriteLine("Please enter a valid config name!");
                }

                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    showConfigNameInput = false;
                    newConfigName = "";
                }
            }
        }

        private void DrawConfigList(ref ConfigCreator config,
                                    ref bool enableBoxes, ref bool enableLines, ref bool enableArrow, ref bool enableSkeleton,
                                    ref bool enableHealthBar, ref bool enableArmorBar, ref bool enablePlayerNames, ref bool enableWeaponNames,
                                    ref bool enableSkeletonColorSeen, ref bool enableAimbot, ref bool enableCircleAimbot,
                                    ref bool enableAimbotSpotted, ref bool enableTriggerBot, ref bool enableBunnyHop,
                                    ref bool enableAntiRecoil, ref bool enableAntiFlash,
                                    ref bool boxOutline, ref bool healthOutline, ref bool armorOutline, ref bool noSniperScope, ref bool noScopeCrosshair,
                                    ref int fov, ref int circleSize, ref int aimbotSmoothing, ref int selectedHealth, ref int selectedSorting,
                                    ref Vector4 enemyColor, ref Vector4 lineColor, ref Vector4 spottedColor, ref Vector4 arrowColor, ref Vector4 skeletonColor,
                                    ref Vector4 circleColor, ref Vector4 weaponNameColor, ref Vector4 nameColor)
        {
            string[] configFiles = Directory.GetFiles(configFolder, "*.json");
            foreach (string file in configFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                if (saveNotificationTimers.ContainsKey(fileName) && saveNotificationTimers[fileName] > 0f)
                {
                    float alpha = Math.Clamp(saveNotificationTimers[fileName] / saveTimer, 0f, 1f);
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.1f, 1f, 0.1f, alpha));
                    ImGui.Text($"{fileName} has been saved!");
                    ImGui.PopStyleColor();
                    saveNotificationTimers[fileName] -= ImGui.GetIO().DeltaTime;
                }

                ImGui.PushStyleColor(ImGuiCol.Text, fileName == currentConfigName
                    ? new Vector4(0.2f, 1.0f, 0.8f, 1.0f)
                    : new Vector4(0.4f, 0.4f, 0.4f, 1.0f));

                ImGui.Text(fileName);
                ImGui.PopStyleColor();

                ImGui.SameLine();

                if (ImGui.Button($"Open##{fileName}"))
                {
                    string json = File.ReadAllText(file);
                    var options = new JsonSerializerOptions { IncludeFields = true };
                    ConfigCreator loaded = JsonSerializer.Deserialize<ConfigCreator>(json, options);
                    ApplyConfig(loaded,
                                ref enableBoxes, ref enableLines, ref enableArrow, ref enableSkeleton,
                                ref enableHealthBar, ref enableArmorBar, ref enablePlayerNames, ref enableWeaponNames,
                                ref enableSkeletonColorSeen, ref enableAimbot, ref enableCircleAimbot,
                                ref enableAimbotSpotted, ref enableTriggerBot, ref enableBunnyHop,
                                ref enableAntiRecoil, ref enableAntiFlash,
                                ref boxOutline, ref healthOutline, ref armorOutline, ref noSniperScope, ref noScopeCrosshair, 
                                ref fov, ref circleSize, ref aimbotSmoothing, ref selectedHealth, ref selectedSorting,
                                ref enemyColor, ref lineColor, ref spottedColor, ref arrowColor, ref skeletonColor,
                                ref circleColor, ref weaponNameColor, ref nameColor);

                    currentConfigName = fileName;
                }
                ImGui.SameLine();

                if (ImGui.Button($"Save##{fileName}"))
                {
                    saveNotificationTimers[fileName] = saveTimer;
                    SaveConfig(fileName, new ConfigCreator
                    {
                        Boxes = enableBoxes,
                        Lines = enableLines,
                        Arrow = enableArrow,
                        Skeleton = enableSkeleton,
                        HealthBar = enableHealthBar,
                        ArmorBar = enableArmorBar,
                        Names = enablePlayerNames,
                        WeaponNames = enableWeaponNames,
                        SkeletonColorSeen = enableSkeletonColorSeen,
                        Aimbot = enableAimbot,
                        CircleAimbot = enableCircleAimbot,
                        AimbotSpotted = enableAimbotSpotted,
                        TriggerBot = enableTriggerBot,
                        BunnyHop = enableBunnyHop,
                        AntiRecoil = enableAntiRecoil,
                        AntiFlash = enableAntiFlash,
                        BoxOutline = boxOutline,
                        HealthOutline = healthOutline,
                        ArmorOutline = armorOutline,
                        NoSniperScope = noSniperScope,
                        NoScopeCrosshair = noScopeCrosshair,
                        fov = fov,
                        circleSize = circleSize,
                        aimbotSmoothing = aimbotSmoothing,
                        BoxColor = enemyColor,
                        LineColor = lineColor,
                        SpottedColor = spottedColor,
                        ArrowColor = arrowColor,
                        SkeletonColor = skeletonColor,
                        CircleColor = circleColor,
                        WeaponNameColor = weaponNameColor,
                        NameColor = nameColor,
                    });
                }
                ImGui.SameLine();

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.6f, 0.1f, 0.1f, 1.0f));
                if (ImGui.Button($"X##{fileName}"))
                    ImGui.OpenPopup($"ConfirmDelete##{fileName}");
                ImGui.PopStyleColor();

                bool isOpen = true;
                if (ImGui.BeginPopupModal($"ConfirmDelete##{fileName}", ref isOpen, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    ImGui.Text($"Are you sure you want to delete {fileName} forever?");
                    if (ImGui.Button("Yes", new Vector2(160, 0)))
                    {
                        File.Delete(file);
                        if (currentConfigName == fileName) currentConfigName = "";
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Nah", new Vector2(160, 0)))
                        ImGui.CloseCurrentPopup();
                }
            }
        }

        public void SaveConfig(string name, ConfigCreator config)
        {
            string safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
            string path = Path.Combine(configFolder, $"{safeName}.json");
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
            File.WriteAllText(path, JsonSerializer.Serialize(config, options));
        }

        private void ApplyConfig(ConfigCreator loaded,
                                 ref bool enableBoxes, ref bool enableLines, ref bool enableArrow, ref bool enableSkeleton,
                                 ref bool enableHealthBar, ref bool enableArmorBar, ref bool enablePlayerNames, ref bool enableWeaponNames,
                                 ref bool enableSkeletonColorSeen, ref bool enableAimbot, ref bool enableCircleAimbot,
                                 ref bool enableAimbotSpotted, ref bool enableTriggerBot, ref bool enableBunnyHop,
                                 ref bool enableAntiRecoil, ref bool enableAntiFlash,
                                 ref bool boxOutline, ref bool healthOutline, ref bool armorOutline, ref bool noSniperScope, ref bool noScopeCrosshair,
                                 ref int fov, ref int circleSize, ref int aimbotSmoothing, ref int selectedHealth, ref int selectedSorting,
                                 ref Vector4 enemyColor, ref Vector4 lineColor, ref Vector4 spottedColor, ref Vector4 arrowColor, ref Vector4 skeletonColor,
                                 ref Vector4 circleColor, ref Vector4 weaponNameColor, ref Vector4 nameColor)
        {
            enableBoxes = loaded.Boxes;
            enableLines = loaded.Lines;
            enableArrow = loaded.Arrow;
            enableSkeleton = loaded.Skeleton;
            enableHealthBar = loaded.HealthBar;
            enableArmorBar = loaded.ArmorBar;
            enablePlayerNames = loaded.Names;
            enableWeaponNames = loaded.WeaponNames;
            enableSkeletonColorSeen = loaded.SkeletonColorSeen;
            enableAimbot = loaded.Aimbot;
            enableCircleAimbot = loaded.CircleAimbot;
            enableAimbotSpotted = loaded.AimbotSpotted;
            enableTriggerBot = loaded.TriggerBot;
            enableBunnyHop = loaded.BunnyHop;
            enableAntiRecoil = loaded.AntiRecoil;
            enableAntiFlash = loaded.AntiFlash;
            boxOutline = loaded.BoxOutline;
            healthOutline = loaded.HealthOutline;
            armorOutline = loaded.ArmorOutline;
            noSniperScope = loaded.NoSniperScope;
            noScopeCrosshair = loaded.NoScopeCrosshair;
            fov = loaded.fov;
            circleSize = loaded.circleSize;
            aimbotSmoothing = loaded.aimbotSmoothing;
            selectedHealth = loaded.selectedHealth;
            selectedSorting = loaded.selectedSorting;
            enemyColor = loaded.BoxColor;
            lineColor = loaded.LineColor;
            spottedColor = loaded.SpottedColor;
            arrowColor = loaded.ArrowColor;
            skeletonColor = loaded.SkeletonColor;
            circleColor = loaded.CircleColor;
            weaponNameColor = loaded.WeaponNameColor;
            nameColor = loaded.NameColor;
        }
    }
}
