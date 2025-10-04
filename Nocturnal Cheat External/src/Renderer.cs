using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vortice.Direct3D11;
using Vortice.Win32;

namespace Noturnal_Cheat_External
{
    internal class Renderer : Overlay
    {
        // finally added auto updated offsets

        // renderer variables

        public Vector2 screenSize = new Vector2(1920, 1080); // use your own resolution

        // entities copy, using more thread save methods
        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity firstentity = new Entity();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();

        // config
        private ConfigManager configManager = new ConfigManager();
        private ConfigCreator config = new ConfigCreator();

        // Gui elements
        private bool showGUI = true;
        private bool prevKeyPressed = false;

        private bool enableBoxes;
        private bool enableLines;
        private bool enableTargetingLine;
        private bool enableArrow;
        private bool enableSkeleton;
        private bool enableSkeletonColorSeen;
        private bool enableHealthBar;
        private bool enableArmorBar;
        private bool enablePlayerNames;
        private bool enableWeaponNames;
        private bool enableSpectatorNames;
        public bool enableAimbot;
        public bool enableCircleAimbot;
        public bool enableAimbotSpotted;
        public bool enableTriggerBot;
        public bool enableBunnyHop;
        public bool enableAntiRecoil;
        public bool enableAntiFlash;
        public bool enableNoSniperScope;
        private bool enableNoScopeCrosshair;

        // customizations
        private bool enableBoxOutline;
        private bool enableHealthOutline;
        private bool enableArmorOutline;

        public int selectedHealth = 0;
        public int selectedDrawTargeting = 0;
        public int selectedSorting = 0;

        // match
        public bool matchStarted;

        public bool isScoped;

        // int
        public int fov = 90;
        public int circleSize = 30;

        public int aimbotSmoothing = 1;

        // Colors
        private Vector4 boxColor = new Vector4(1, 0, 0, 1); // default red
        private Vector4 lineColor = new Vector4(1, 0, 0, 1); // default red
        private Vector4 spottedColor = new Vector4(1, 1, 0, 1); // default yellow
        private Vector4 arrowColor = new Vector4(1, 1, 1, 1); // default white
        private Vector4 skeletonColor = new Vector4(1, 1, 1, 1); // default white
        private Vector4 circleColor = new Vector4(1, 1, 1, 0.1f); // default white and a little bit transparent
        private Vector4 weaponNameColor = new Vector4(1, 1, 1, 1); // default white
        private Vector4 nameColor = new Vector4(1, 1, 1, 1); // default white

        // spectators
        private List<string> spectators;

        // bone stuff
        float boneThickness = 8;

        // draw list
        ImDrawListPtr drawList;

        // hotkey import
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vkey);

        protected override void Render()
        {
            // get pressed
            bool keyPressed = (GetAsyncKeyState(0xA3) & 0x8000) != 0;

            if (keyPressed && !prevKeyPressed)
            {
                showGUI = !showGUI;
            }

            prevKeyPressed = keyPressed;

            // ImGui menu
            if (showGUI)
            {
                ImGui.SetNextWindowSize(new Vector2(320, 420), ImGuiCond.Once);
                ImGui.Begin("Nocturnal Cheat v2.0", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

                if (ImGui.BeginTabBar("##tabs"))
                {
                    // ================= VISUALS TAB =================
                    if (ImGui.BeginTabItem("Visuals"))
                    {
                        ImGui.Text("Wallhack Options");
                        ImGui.Separator();

                        ImGui.Checkbox("Enable Boxes", ref enableBoxes);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Draws boxes around enemies");

                        ImGui.Checkbox("Enable Lines", ref enableLines);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Draws lines that point to the position of enemies");

                        ImGui.Checkbox("Enable Arrow", ref enableArrow);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Draws an arrow pointing towards off-screen enemies");

                        ImGui.Checkbox("Enable Skeleton", ref enableSkeleton);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("show you to see the skeleton of enemies");

                        if (enableSkeleton)
                        {
                            ImGui.Checkbox("Change Skeleton Color", ref enableSkeletonColorSeen);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("If enabled, changes skeleton color when an enemy is spotted");
                        }

                        ImGui.Text("Enemy Stats");
                        ImGui.Separator();

                        ImGui.Checkbox("Enable Health", ref enableHealthBar);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Shows the current health of enemies");

                        ImGui.Checkbox("Enable Armor", ref enableArmorBar);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Shows the current armor of enemies (bar only)");

                        ImGui.Spacing();

                        ImGui.Text("Enemy Info");
                        ImGui.Separator();

                        ImGui.Checkbox("Enable Player Names", ref enablePlayerNames);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Displays the names of enemies above their heads");

                        ImGui.Checkbox("Enable Weapon Names", ref enableWeaponNames);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Displays the weapons enemies are holding");

                        ImGui.EndTabItem();
                    }

                    // ================= AIMBOT TAB =================
                    if (ImGui.BeginTabItem("Aimbot"))
                    {
                        ImGui.Text("Aimbot Settings");
                        ImGui.Separator();

                        ImGui.Checkbox("Enable Aimbot", ref enableAimbot);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Enables an Aimbot when the button C is pressed");

                        if (enableAimbot)
                        {
                            ImGui.Checkbox("Enable Spotted Aimbot", ref enableAimbotSpotted);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("If enabled, aimbot only works on spotted enemies (kinda laggy)");

                            ImGui.Checkbox("Enable Legit Aimbot", ref enableCircleAimbot);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("Enables an aimbot that locks onto enemies within a circular area");

                            ImGui.Checkbox("Show Aimbot Target", ref enableTargetingLine);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("Shows a line, that points to the head of the enemy which the aimbot is targeting (was lazy and didn't fix it)");
                        }

                        ImGui.Spacing();

                        ImGui.Checkbox("Enable Triggerbot", ref enableTriggerBot);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Enables an triggerbot which works when C is pressed");

                        ImGui.Spacing();

                        ImGui.SliderInt("FOV", ref fov, 1, 240);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Changes your in game FOV");

                        ImGui.Spacing();

                        if (enableAimbot)
                            ImGui.InputInt("Smooth", ref aimbotSmoothing, 5);
                                if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("Sets how long it takes to lock onto a target (in ms)");

                        ImGui.Spacing();

                        if (enableCircleAimbot)
                            ImGui.SliderInt("Circle Size", ref circleSize, 10, 240);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("Changes the area of the circle");

                        string[] sortingType = { "Closest To Crosshair", "Closest To You", "Lowest Health", "Lowest Armor"};

                        if (enableAimbot)
                        {
                            ImGui.SetNextItemWidth(160);
                            ImGui.Combo("Enemies Targeting", ref selectedSorting, sortingType, sortingType.Length);
                        }

                        ImGui.EndTabItem();
                    }

                    // ================= MISC TAB =================
                    if (ImGui.BeginTabItem("Misc"))
                    {
                        ImGui.Text("Misc Settings");
                        ImGui.Separator();

                        ImGui.Checkbox("Enable BunnyHop", ref enableBunnyHop);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Enables BunnyHop which works when space is pressed");

                        ImGui.Checkbox("Enable AntiFlash", ref enableAntiFlash);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Enables antiflash (self explanatory)");

                        ImGui.Checkbox("Disable Zoom Scope", ref enableNoSniperScope);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Disables the zoom when scoped in with a sniper");

                        ImGui.Checkbox("Enable Sniper Crosshair", ref enableNoScopeCrosshair);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("If the sniper is equiped, and there is no crosshair, it gives you one");

                        // ImGui.Checkbox("Enable Spectator List", ref enableSpectatorNames);
                        // if (ImGui.IsItemHovered())
                        // ImGui.SetTooltip("Show the current players names, that are spectating you");

                        ImGui.EndTabItem();
                    }

                    // ================= CONFIG TAB =================
                    if (ImGui.BeginTabItem("Configs"))
                    {
                        ImGui.Text("Config Manager");
                        ImGui.Separator();

                        configManager.Render(ref config,
                            ref enableBoxes, ref enableLines, ref enableArrow, ref enableSkeleton,
                            ref enableHealthBar, ref enableArmorBar, ref enablePlayerNames, ref enableWeaponNames,
                            ref enableSkeletonColorSeen, ref enableAimbot, ref enableCircleAimbot,
                            ref enableAimbotSpotted, ref enableTriggerBot, ref enableBunnyHop,
                            ref enableAntiRecoil, ref enableAntiFlash, ref enableBoxOutline, ref enableHealthOutline, ref enableArmorOutline, ref enableNoSniperScope, ref enableNoScopeCrosshair,
                            ref fov, ref circleSize, ref aimbotSmoothing, ref selectedHealth, ref selectedSorting,
                            ref boxColor, ref lineColor, ref spottedColor, ref arrowColor, ref skeletonColor,
                            ref circleColor, ref weaponNameColor, ref nameColor);

                        ImGui.EndTabItem();
                    }

                    // ================= COLORS TAB =================
                    if (ImGui.BeginTabItem("Settings"))
                    {
                        ImGui.Text("Information");
                        ImGui.Separator();

                        string[] healthSettings = { "Both", "Bars", "Numbers" };

                        ImGui.SetNextItemWidth(160);
                        ImGui.Combo("Health Information", ref selectedHealth, healthSettings, healthSettings.Length);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("Manage how the health displays information");

                        string[] drawTargetingSettings = { "Both", "Head", "Line" };

                        ImGui.SetNextItemWidth(160);
                        ImGui.Combo("Show Targeting", ref selectedDrawTargeting, drawTargetingSettings, drawTargetingSettings.Length);
                            if (ImGui.IsItemHovered())
                                ImGui.SetTooltip("Manage how the targeting option looks");

                        ImGui.Text("Outlines");
                        ImGui.Separator();

                        ImGui.Checkbox("Box Outline", ref enableBoxOutline);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Shows a outline around the box");

                        ImGui.Checkbox("Health Outline", ref enableHealthOutline);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Shows a outline around the health bar and numbers");

                        ImGui.Checkbox("Armorbar Outline", ref enableArmorOutline);
                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Shows a outline around the armor bar");

                        ImGui.Text("Color Settings");
                        ImGui.Separator();

                        // box color
                        if (enableBoxes) {
                            ImGui.Text("Box Color");
                            ImGui.ColorEdit4("##boxcolor", ref boxColor);
                        }

                        // line color
                        if (enableLines) {
                            ImGui.Text("Line Color");
                            ImGui.ColorEdit4("##linecolor", ref lineColor);
                        }


                        // arrow color
                        if (enableArrow) {
                            ImGui.Text("Arrow Color");
                            ImGui.ColorEdit4("##arrowcolor", ref arrowColor);
                        }

                        // weapon names color
                        if (enableWeaponNames) {
                            ImGui.Text("Weapon Names Color");
                            ImGui.ColorEdit4("##weaponNameColor", ref weaponNameColor);
                        }

                        // player names color
                        if (enablePlayerNames) {
                            ImGui.Text("Player Names Color");
                            ImGui.ColorEdit4("##nameColor", ref nameColor);
                        }

                        // skeleton color
                        if (enableSkeleton) {
                            ImGui.Text("Skeleton Color");
                            ImGui.ColorEdit4("##bonecolor", ref skeletonColor);
                        }

                        // skeleton color when spotted
                        if (enableSkeletonColorSeen) {
                            ImGui.Text("Skeleton Color When Spotted");
                            ImGui.ColorEdit4("##spottedColor", ref spottedColor);
                        }

                        ImGui.Spacing();

                        // circle color
                        if (enableCircleAimbot) {
                            ImGui.Text("Circle Color");
                            ImGui.ColorEdit4("##circlecolor", ref circleColor);
                        }


                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.End();
            }


            // end of ImGui


            // draw overlay
            DrawOverlay(screenSize);
            drawList = ImGui.GetWindowDrawList();

            // draw stuff

            foreach (var entity in entities)
            {
                if (EntityOnScreen(entity))
                {
                    // on screen

                    // draw boxes around enemies
                    if (enableBoxes)
                        DrawBox(entity);

                    // draw lines pointing to enemies
                    if (enableLines)
                        DrawLine(entity);

                    // draw the skeleton of enemies
                    if (enableSkeleton)
                        // if (enableTargetingLine && entity == firstentity)
                            DrawSkeleton(entity);

                    // draw enemies health
                    if (enableHealthBar)
                        DrawHealthBar(entity);

                    // draw enemies armor value
                    if (enableArmorBar && entity.armor > 0)
                        DrawArmorBar(entity);

                    // draw enemy names
                    if (enablePlayerNames)
                        DrawName(entity);

                    // draw enemy weapon names
                    if (enableWeaponNames)
                        DrawWeaponName(entity);

                    // draw head lines
                    if (enableTargetingLine)
                        DrawTargeting(firstentity);
                }
                else
                {
                    // off screen

                    // draw arrows pointing towards enemies
                    if (enableArrow)
                        DrawArrow(entity);
                }
            }

            // draw spectator list
            if (enableSpectatorNames)
                DrawSpectatorNames();

            // no-scope crosshair
            if(enableNoScopeCrosshair)
                NoScopeCorsshair();

            // draw Aimbot Circle
            if (enableCircleAimbot && enableAimbot)
                drawList.AddCircle(new Vector2(screenSize.X / 2, screenSize.Y / 2), circleSize, ImGui.ColorConvertFloat4ToU32(circleColor));
        }

        // drawing methods and functions

        private void DrawSkeleton(Entity entity)
        {
            uint color = ImGui.ColorConvertFloat4ToU32(skeletonColor);
            uint spottedcolor = ImGui.ColorConvertFloat4ToU32(spottedColor);

            if (enableSkeletonColorSeen)
                color = entity.spotted == true ? spottedcolor : color;

            float currentBoneThickness = boneThickness / entity.distance * 2 / 3; // not perfect

            // Get distance in pixels between head and neck bones
            float headPixelHeight = Vector2.Distance(entity.Skeleton2D[2], entity.Skeleton2D[1]);

            // Scale it up
            float headRadius = headPixelHeight * 1.3f;

            // pulse head
            float time = (float)ImGui.GetTime(); 
            float pulse = (float)((Math.Sin(time * 5.0f) + 1.0) / 2.0);
            float animatedRadius = Lerp(headRadius, headRadius * 1.6f, pulse);

            if (selectedDrawTargeting != 0 || selectedDrawTargeting != 1)
                drawList.AddCircle(entity.Skeleton2D[2], headRadius, color); // circle on head

            drawList.AddLine(entity.Skeleton2D[1], entity.Skeleton2D[2], color, currentBoneThickness); // neck to head
            drawList.AddLine(entity.Skeleton2D[1], entity.Skeleton2D[3], color, currentBoneThickness); // neck to left shoulder
            drawList.AddLine(entity.Skeleton2D[1], entity.Skeleton2D[6], color, currentBoneThickness); // neck to right shoulder
            drawList.AddLine(entity.Skeleton2D[3], entity.Skeleton2D[4], color, currentBoneThickness); // left shoulder to left arm
            drawList.AddLine(entity.Skeleton2D[6], entity.Skeleton2D[7], color, currentBoneThickness); // right shoulder to right arm
            drawList.AddLine(entity.Skeleton2D[4], entity.Skeleton2D[5], color, currentBoneThickness); // left arm to left hand
            drawList.AddLine(entity.Skeleton2D[7], entity.Skeleton2D[8], color, currentBoneThickness); // right arm to right hand
            drawList.AddLine(entity.Skeleton2D[1], entity.Skeleton2D[0], color, currentBoneThickness); // neck to waist
            drawList.AddLine(entity.Skeleton2D[0], entity.Skeleton2D[9], color, currentBoneThickness); // waist to left knee
            drawList.AddLine(entity.Skeleton2D[0], entity.Skeleton2D[11], color, currentBoneThickness); // waist to right knee
            drawList.AddLine(entity.Skeleton2D[9], entity.Skeleton2D[10], color, currentBoneThickness); // left knee to left foot
            drawList.AddLine(entity.Skeleton2D[11], entity.Skeleton2D[12], color, currentBoneThickness); // right knee to right foot
        }

        private void DrawHealthBar(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;
            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.position2D.X + entityHeight / 3;

            float barPercentWidth = 0.05f;
            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);
            float barHeight = entityHeight * (entity.health / 100f) * 1.11f;

            Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
            Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);

            float hp = Math.Clamp(entity.health / 100f, 0f, 1f);

            Vector4 green = new Vector4(0f, 1f, 0f, 1f);
            Vector4 orange = new Vector4(1f, 0.65f, 0f, 1f);
            Vector4 red = new Vector4(1f, 0f, 0f, 1f);
            Vector4 darkRed = new Vector4(0.1f, 0f, 0.05f, 1f);
            Vector4 outlineColor = new Vector4(0, 0, 0, 1);

            Vector4 barColor;

            if (hp > 0.5f)
            {
                // 50%–100% -> Orange -> Green
                float t = (hp - 0.5f) / 0.5f;
                barColor = LerpColors(orange, green, t);
            }
            else if (hp > 0.2f)
            {
                // 20%–50% -> Red -> Orange
                float t = (hp - 0.2f) / 0.3f;
                barColor = LerpColors(red, orange, t);
            }
            else
            {
                // 0%–20% -> Dark Red -> Red
                float t = hp / 0.2f;
                barColor = LerpColors(darkRed, red, t);
            }

            float backdropPadding = 1.0f;
            Vector2 backTop = new Vector2(barTop.X - backdropPadding, barTop.Y - backdropPadding);
            Vector2 backBottom = new Vector2(barBottom.X + backdropPadding, barBottom.Y + backdropPadding);

            if(selectedHealth == 1 || selectedHealth == 0)
            {
                if (enableHealthOutline)
                    drawList.AddRectFilled(backTop, backBottom, ImGui.ColorConvertFloat4ToU32(outlineColor));

                drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));
            }

            string hpText = $"{entity.health:0}";

            // calculate text position
            Vector2 textSize = ImGui.CalcTextSize(hpText);
            float textX = barTop.X + (barBottom.X - barTop.X) / 2 - textSize.X / 2;
            float textY = barBottom.Y;
            Vector2 textPos = new Vector2(textX, textY);

            // base font size (current ImGui font)
            float baseSize = ImGui.GetFontSize();

            // clamp so it doesn't get too tiny or too huge
            float fontSize = Math.Clamp(baseSize * entityHeight / 200f, baseSize * 0.8f, baseSize * 24f);

            // offsets for outline
            Vector2[] offsets = new Vector2[]
            {
                new Vector2(-0.5f, 0),
                new Vector2(0.5f, 0),
                new Vector2(0, -0.5f),
                new Vector2(0, 0.5f),
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(0.5f, 0.5f)
            };

            // draw
            if (selectedHealth == 2 || selectedHealth == 0)
            {
                foreach (var offset in offsets)
                    if (enableHealthOutline)
                        drawList.AddText(ImGui.GetFont(), fontSize, textPos + offset, ImGui.ColorConvertFloat4ToU32(outlineColor), hpText);

                drawList.AddText(ImGui.GetFont(), fontSize, textPos, ImGui.ColorConvertFloat4ToU32(barColor), hpText);
            }
        }
        private void DrawArmorBar(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;
            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.position2D.X + entityHeight / 3;

            float barPercentWidth = 0.05f;
            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);
            float barHeight = entityHeight * (entity.armor / 100f) * 1.11f;

            Vector2 barTop = new Vector2(boxRight, entity.position2D.Y - barHeight);
            Vector2 barBottom = new Vector2(boxRight + barPixelWidth, entity.position2D.Y);

            float armor = Math.Clamp(entity.armor / 100f, 0f, 1f);

            // Dark blue gradient colors
            Vector4 lightDarkBlue = new Vector4(0.3f, 0.2f, 1f, 1f); // lighter dark blue
            Vector4 darkBlue = new Vector4(0.05f, 0.05f, 0.2f, 1f); // full dark blue
            Vector4 outlineColor = new Vector4(0, 0, 0, 1);

            Vector4 barColor = LerpColors(lightDarkBlue, darkBlue, 1f - armor);

            // backdrop for outline
            float backdropPadding = 1.0f;
            Vector2 backTop = new Vector2(barTop.X - backdropPadding, barTop.Y - backdropPadding);
            Vector2 backBottom = new Vector2(barBottom.X + backdropPadding, barBottom.Y + backdropPadding);

            // draw bar
            if (enableArmorOutline)
                drawList.AddRectFilled(backTop, backBottom, ImGui.ColorConvertFloat4ToU32(outlineColor));
            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));

            // helmet icon
            string helmetText = "Yes";

            // calculate text position
            Vector2 textSize = ImGui.CalcTextSize(helmetText);
            float textX = barTop.X + (barBottom.X - barTop.X) / 2 + 2 - textSize.X / 2;
            float textY = barTop.Y - 10;
            Vector2 textPos = new Vector2(textX, textY);

            // base font size
            float baseSize = ImGui.GetFontSize();

            // clamp so it doesn't get too tiny or too huge
            float fontSize = Math.Clamp(baseSize * entityHeight / 200f, baseSize * 0.8f, baseSize * 24f);

            if (!entity.hasHeadArmor) // should be without the ! but the "hasHeadArmor" doesn't do its job
                drawList.AddText(ImGui.GetFont(), fontSize, textPos, ImGui.ColorConvertFloat4ToU32(barColor), helmetText);
        }
        private void DrawName(Entity entity)
        {
            // entity name
            string nameText = entity.name;

            // calculate box height
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            // box dimensions
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y - entityHeight * 0.12f);
            Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);
            Vector2 boxTopCenter = new Vector2((rectTop.X + rectBottom.X) / 2, rectTop.Y);

            // font size
            float fontSize = 10f;

            Vector2 textSize = ImGui.CalcTextSize(nameText) * (fontSize / ImGui.GetFontSize());

            // position the text
            Vector2 textLocation = new Vector2(
                boxTopCenter.X - textSize.X / 2,
                boxTopCenter.Y - textSize.Y
            );

            // draw texts
            drawList.AddText(ImGui.GetFont(), fontSize, textLocation + new Vector2(1, 1), ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0f, 1f)), nameText);
            drawList.AddText(ImGui.GetFont(), fontSize, textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), nameText);
        }

        private void DrawWeaponName(Entity entity)
        {
            // entity weapon name
            string weaponNameText = entity.currentWeaponName;

            // calculate box height
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            // box dimensions
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y - entityHeight * 0.12f);
            Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);
            Vector2 boxBottomCenter = new Vector2((rectTop.X + rectBottom.X) / 2, rectBottom.Y);

            // font size
            float fontSize = 10f;

            Vector2 textSize = ImGui.CalcTextSize(weaponNameText) * (fontSize / ImGui.GetFontSize());

            // position the text
            Vector2 textLocation = new Vector2(
                boxBottomCenter.X - textSize.X / 2,
                boxBottomCenter.Y + 2
            );

            if(entity.currentWeaponName != null)
            {
                drawList.AddText(ImGui.GetFont(), fontSize, textLocation + new Vector2(1, 1), ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0f, 1f)), weaponNameText);
                drawList.AddText(ImGui.GetFont(), fontSize, textLocation, ImGui.ColorConvertFloat4ToU32(weaponNameColor), weaponNameText);
            }
        }

        private void NoScopeCorsshair()
        {
            if (!isScoped && localPlayer.currentWeaponName == "Scout" || !isScoped && localPlayer.currentWeaponName == "AWP")
            {
                int size = 9; // size of the crosshair

                drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y / 2 - size), new Vector2(screenSize.X / 2, screenSize.Y / 2 + size), ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)));
                drawList.AddLine(new Vector2(screenSize.X / 2 - size, screenSize.Y / 2), new Vector2(screenSize.X / 2 + size, screenSize.Y / 2), ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)));
            }
        }

        private void DrawSpectatorNames()
        {
            ImGui.Begin("Spectators", ImGuiWindowFlags.AlwaysAutoResize);

            if (spectators != null && spectators.Count > 0)
            {
                foreach (var name in spectators)
                {
                    ImGui.Text(name);
                }
            }
            else
            {
                ImGui.Text("No spectators");
            }
        }

        private void DrawBox(Entity entity)
        {
            // calculate box height
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;

            // calculate box dimensions
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y - entityHeight * 0.12f);
            Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);

            // draw
            if (enableBoxOutline)
                drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 1f)), 0, ImDrawFlags.None, 2.0f);
            drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor), 0, ImDrawFlags.None, 1.0f);
        }

        private void DrawArrow(Entity entity)
        {
            Vector2 screenCenter = new Vector2(screenSize.X / 2, screenSize.Y / 2);

            // Calculate direction to enemy
            Vector2 dir = entity.pos2D - screenCenter;
            dir = Vector2.Normalize(dir);

            // Position the triangle from center
            Vector2 trianglePos = screenCenter + dir * 30;

            // Calculate rotation angle
            float angle = (float)Math.Atan2(dir.Y, dir.X);

            Vector2 tip = new Vector2(8, 0);
            Vector2 baseLeft = new Vector2(0, -3);
            Vector2 baseRight = new Vector2(0, 3);

            // Rotation matrix
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Rotate points
            Vector2 Rotate(Vector2 p)
            {
                return new Vector2(
                    p.X * cos - p.Y * sin,
                    p.X * sin + p.Y * cos
                );
            }

            Vector2 rotatedTip = Rotate(tip) + trianglePos;
            Vector2 rotatedBaseLeft = Rotate(baseLeft) + trianglePos;
            Vector2 rotatedBaseRight = Rotate(baseRight) + trianglePos;

            // Draw the filled triangle
            drawList.AddTriangleFilled(rotatedTip, rotatedBaseLeft, rotatedBaseRight, ImGui.ColorConvertFloat4ToU32(arrowColor));
        }
        private void DrawLine(Entity entity)
        {
            // draw line
            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }
        private void DrawTargeting(Entity entity)
        {
            // Get distance in pixels between head and neck bones
            float headPixelHeight = Vector2.Distance(entity.Skeleton2D[2], entity.Skeleton2D[1]);

            // pulse head
            float time = (float)ImGui.GetTime();
            float pulse = (float)((Math.Sin(time * 10f) + 1.0) / 2.0);
            float animatedRadius = Lerp(headPixelHeight * 1.3f, headPixelHeight * 1.3f * 1.6f, pulse);

            if (selectedDrawTargeting == 0 || selectedDrawTargeting == 2)
                drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y / 2), entity.head2D, ImGui.ColorConvertFloat4ToU32(Lighten(lineColor, 0f, 0.5f)));

            if (selectedDrawTargeting == 0 || selectedDrawTargeting == 1)
                drawList.AddCircle(entity.Skeleton2D[2], animatedRadius, ImGui.ColorConvertFloat4ToU32(Lighten(skeletonColor, 0.3f, 1f))); // circle on head
        }

        // transfer entity methods


        public void UpdateEntities(IEnumerable<Entity> newEntities) // update entities
        {
            entities = new ConcurrentQueue<Entity>(newEntities);
        }
        public void UpdateFirstEntity(Entity FEntity) // update first Eentity
        {
            firstentity = FEntity;
        }
        public void UpdateSpectators(List<string> spectators)
        {
            this.spectators = spectators;
        }
        public void UpdateLocalPlayer(Entity newEntity) // update localplayer
        {
            lock (entityLock)
            {
                localPlayer = newEntity;
            }
        }
        public Entity GetLocalPlayer() // get localplayer
        {
            lock (entityLock) {
            return localPlayer;
            }
        }

        // Linear interpolation between two colors
        private Vector4 LerpColors(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }

        // lighten the colors
        public static Vector4 Lighten(Vector4 color, float amount, float alpha)
        {
            return new Vector4(
                Math.Clamp(color.X + amount, 0f, 1f), // R
                Math.Clamp(color.Y + amount, 0f, 1f), // G
                Math.Clamp(color.Z + amount, 0f, 1f), // B
                Math.Clamp(color.W + alpha, 0f, 1f) // A
            );
        }

        // like MathF.Lerp
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;

        }

        // check position
        bool EntityOnScreen(Entity entity)
        {
            if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
            {
                return true;
            }
            return false;
        }

        void DrawOverlay(Vector2 screenSize) // Overlay window
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0)); // Overlay position
            ImGui.Begin("Cheat Overlay", 
                  ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }
    }
}

