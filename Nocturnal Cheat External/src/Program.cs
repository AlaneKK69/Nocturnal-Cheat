using Noturnal_Cheat_External;
using ProcessMemory64;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Vortice.DXGI;

// main logic

// start dumper
Dumper.Run();

// int memory
ProcessMemory mem = new ProcessMemory("cs2");

// get client module
IntPtr client = mem.GetModuleBase("client.dll");

// init renderer
Renderer renderer = new Renderer();
Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
renderThread.Start();

// get screen size from renderer
Vector2 screenSize = renderer.screenSize;

// store entities
List<Entity> entities = new List<Entity>();
Entity localPlayer = new Entity();

// spectator list
List<string> spectatorNames = new List<string>();

// hotkeys
const int aimbotHotkey = 0x43;
const int triggerHotkey = 0x43;
const int jumpHotkey = 0x20;

// spotted bool
bool spotted = false;

// offsets

// dumper directory
string dumperDirectory = $"{AppContext.BaseDirectory}Dumper\\output\\";

// offsets.cs
int dwEntityList = FileSearcher.SearchFileForInt(dumperDirectory + "offsets.cs", "dwEntityList");
int dwViewAngles = FileSearcher.SearchFileForInt(dumperDirectory + "offsets.cs", "dwViewAngles");
int dwViewMatrix = FileSearcher.SearchFileForInt(dumperDirectory + "offsets.cs", "dwViewMatrix");
int dwLocalPlayerPawn = FileSearcher.SearchFileForInt(dumperDirectory + "offsets.cs", "dwLocalPlayerPawn");

// buttons.cs

int attack = FileSearcher.SearchFileForInt(dumperDirectory + "buttons.cs", "attack");
int jump = FileSearcher.SearchFileForInt(dumperDirectory + "buttons.cs", "jump", occurrence: 1);

// client_dll.cs

// main information
int m_vOldOrigin = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_vOldOrigin");
int m_iTeamNum = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iTeamNum");
int m_iIDEntIndex = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iIDEntIndex");
int m_lifeState = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_lifeState");
int m_hPlayerPawn = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_hPlayerPawn");
int m_vecViewOffset = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_vecViewOffset");

// position
int m_pCameraServices = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_pCameraServices");
int m_bIsScoped = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_bIsScoped");
int m_iFOV = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iFOV");

// spotted
int m_entitySpottedState = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_entitySpottedState", occurrence: 2);
int m_bSpotted = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_bSpotted");

// velocity
int m_vecVelocity = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_vecVelocity");

// important info
int m_iszPlayerName = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iszPlayerName");
int m_iHealth = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iHealth");
int m_ArmorValue = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_ArmorValue");
int m_bHasHelmet = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_bHasHelmet");
int m_bHasDefuser = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_bHasDefuser");

// flashbang duration
int m_flFlashBangTime = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_flFlashBangTime");

// model state
int m_modelState = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_modelState");
int m_pGameSceneNode = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_pGameSceneNode");

// triggerbot stuff
int m_pWeaponServices = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_pWeaponServices");
int m_hActiveWeapon = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_hActiveWeapon");
int m_fAccuracyPenalty = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_fAccuracyPenalty");

// bunny hop
int m_fFlags = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_fFlags", occurrence: 1);
int FL_ONGROUND = 1 << 0;

// weapon
int m_pClippingWeapon = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_pClippingWeapon");
int m_iItemDefinitionIndex = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iItemDefinitionIndex");
int m_AttributeManager = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_AttributeManager", occurrence: 2);
int m_Item = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_Item");

// match data
int dwGameRules = FileSearcher.SearchFileForInt(dumperDirectory + "offsets.cs", "dwGameRules");
int m_bHasMatchStarted = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_bHasMatchStarted");

// anti recoil
int m_aimPunchAngle = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_aimPunchAngle");
int m_iShotsFired = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_iShotsFired");

// spectator
int m_pObserverServices = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_pObserverServices");
int m_hObserverTarget = FileSearcher.SearchFileForInt(dumperDirectory + "client_dll.cs", "m_hObserverTarget");

// now ESP loop
while (true)
{
    entities.Clear(); // clean list
    spectatorNames.Clear(); // clear spectator list

    // get entity list
    IntPtr entityList = mem.ReadPointer(client, dwEntityList);

    // make entry
    IntPtr listEntry = mem.ReadPointer(entityList, 0x10);

    // get localplayer
    IntPtr localPlayerPawn = mem.ReadPointer(client, dwLocalPlayerPawn);

    // get camera
    IntPtr cameraServices = mem.ReadPointer(localPlayerPawn, m_pCameraServices);

    // get team
    localPlayer.team = mem.ReadInt(localPlayerPawn, m_iTeamNum);

    // get pawn address
    localPlayer.pawnAddress = mem.ReadPointer(client, dwLocalPlayerPawn);

    // get match data
    IntPtr gameRules = mem.ReadPointer(client + dwGameRules);
    renderer.matchStarted = mem.ReadBool(gameRules, m_bHasMatchStarted);

    // get origin and view
    localPlayer.origin = mem.ReadVec(localPlayer.pawnAddress, m_vOldOrigin);
    localPlayer.view = mem.ReadVec(localPlayer.pawnAddress, m_vecViewOffset);

    // get FOV
    uint currentFov = mem.ReadUInt(cameraServices + m_iFOV);

    // get scoped
    bool isScoped = mem.ReadBool(localPlayerPawn, m_bIsScoped);
    renderer.isScoped = isScoped;

    // get desired FOV
    uint desiredFov = (uint)renderer.fov;

    if (renderer.enableNoSniperScope)
    {
        if (currentFov != desiredFov)
            mem.WriteUInt(cameraServices + m_iFOV, desiredFov); // write new FOV
    }
    else if (!isScoped && currentFov != desiredFov)
        mem.WriteUInt(cameraServices + m_iFOV, desiredFov); // write new FOV

    // antiflash
    float flashDuration = mem.ReadFloat(localPlayerPawn, m_flFlashBangTime);

    if (renderer.enableAntiFlash)
    {
        if (flashDuration > 0)
        {
            mem.WriteFloat(localPlayerPawn, m_flFlashBangTime, 0); // remove flash
        }
    }

    // bunnyhop
    if (renderer.enableBunnyHop && GetAsyncKeyState(jumpHotkey) < 0)
    {
        int flags = mem.ReadInt(localPlayerPawn, m_fFlags);

        if ((flags & FL_ONGROUND) != 0)
        {
            // on ground -> jump
            mem.WriteInt(client, jump, 65537);
        }
        else
        {
            // in air -> release jump
            mem.WriteInt(client, jump, 256);
        }
    }

    // trigger bot
    int teamTrigger = mem.ReadInt(localPlayerPawn, m_iTeamNum);
    int entIndex = mem.ReadInt(localPlayerPawn, m_iIDEntIndex);

    if (renderer.enableTriggerBot && GetAsyncKeyState(triggerHotkey) < 0)
    {
        if (entIndex != -1)
        {
            // get controller from entity index
            IntPtr listEntryTrigger = mem.ReadPointer(entityList, 0x8 * ((entIndex & 0x7FFF) >> 9) + 0x10);
            IntPtr currentPawn = mem.ReadPointer(listEntryTrigger, 0x78 * (entIndex & 0x1FF));

            int entityTeam = mem.ReadInt(currentPawn, m_iTeamNum);

            if (teamTrigger != entityTeam)
            {
                // read weapon pointer
                IntPtr weaponServices = mem.ReadPointer(localPlayerPawn, m_pWeaponServices);
                IntPtr activeWeapon = mem.ReadPointer(weaponServices, m_hActiveWeapon & 0xFFFF); // handle

                // read accuracy penalty
                float accuracyPenalty = mem.ReadFloat(activeWeapon, m_fAccuracyPenalty);

                // check if weapon is accurate enough
                if (accuracyPenalty < 0.01f)
                {
                    mem.WriteInt(client, attack, 65537);
                    Thread.Sleep(1);
                    mem.WriteInt(client, attack, 256);
                }
            }
        }
    }

    // loop through entity list
    for (int i = 0; i < 64; i++)
    {
        // get current controller
        IntPtr currentController = mem.ReadPointer(listEntry, i * 0x78);
        if (currentController == IntPtr.Zero) continue; // check

        // get pawn handle
        int pawnHandle = mem.ReadInt(currentController, m_hPlayerPawn);
        if (pawnHandle == 0) continue;

        // get list
        IntPtr listEntry2 = mem.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
        if (listEntry2 == IntPtr.Zero) continue;

        // get current pawn
        IntPtr currentPawn = mem.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
        if (currentPawn == localPlayer.pawnAddress) continue;

        // spectator pawn
        IntPtr spectatorPawn = mem.ReadPointer(currentController, m_hPlayerPawn);

        // Observer services from pawn
        IntPtr obsServices = mem.ReadPointer(spectatorPawn, m_pObserverServices);

        // Observer target handle
        int obsTargetHandle = mem.ReadInt(obsServices, m_hObserverTarget);

        // Resolve handle -> pawn
        IntPtr listEntryObs = mem.ReadPointer(entityList, 0x8 * ((obsTargetHandle & 0x7FFF) >> 9) + 0x10);

        IntPtr obsPawn = mem.ReadPointer(listEntryObs, 0x78 * (obsTargetHandle & 0x1FF));

        // Check if watching local player
        if (obsPawn == localPlayer.pawnAddress)
        {
            string specName = mem.ReadString(currentController, m_iszPlayerName, 16).Split('\0')[0];
            if (!string.IsNullOrEmpty(specName) && !spectatorNames.Contains(specName))
                spectatorNames.Add(specName);
        }

        // check if all lifestate
        byte lifeState = mem.ReadBytes(currentPawn, m_lifeState, 1)[0];
        if (lifeState != 0) continue;  // Skip if not alive (0 == alive)

        // check team
        int team = mem.ReadInt(currentPawn, m_iTeamNum);
        if (team == localPlayer.team) continue; // skip if team

        // get matrix
        float[] viewMatrix = mem.ReadMatrix(client + dwViewMatrix);

        // get bones
        IntPtr sceneNode = mem.ReadPointer(currentPawn, m_pGameSceneNode);
        IntPtr boneMatrix = mem.ReadPointer(sceneNode, m_modelState + 0x80);

        // get current weapon
        IntPtr currentWeapon = mem.ReadPointer(currentPawn, m_pClippingWeapon);
        IntPtr playerWeapon = mem.ReadPointer(localPlayerPawn, m_pClippingWeapon);

        // get item definition index
        short weaponDefinitionIndex = mem.ReadShort(currentWeapon, m_AttributeManager + m_Item + m_iItemDefinitionIndex);
        short pweaponDefinitionIndex = mem.ReadShort(playerWeapon, m_AttributeManager + m_Item + m_iItemDefinitionIndex);

        // player weapons
        localPlayer.currentWeaponIndex = pweaponDefinitionIndex;
        localPlayer.currentWeaponName = Enum.GetName(typeof(Weapon), pweaponDefinitionIndex);

        // populate entity
        Entity entity = new Entity();

        renderer.matchStarted = mem.ReadBool(dwGameRules, m_bHasMatchStarted);

        entity.pawnAddress = currentPawn;
        entity.controllerAddress = currentController;
        entity.origin = mem.ReadVec(currentPawn, m_vOldOrigin);
        entity.view = mem.ReadVec(currentPawn, m_vecViewOffset);

        entity.velocity = mem.ReadVec(currentPawn, m_vecVelocity);

        entity.head = mem.ReadVec(boneMatrix, 6 * 32);
        entity.neck = mem.ReadVec(boneMatrix, 5 * 32);
        entity.torso = mem.ReadVec(boneMatrix, 3 * 32);
        entity.feet = mem.ReadVec(boneMatrix, 23 * 32);

        entity.head2D = Calculate.WorldToScreen(viewMatrix, entity.head, screenSize);
        entity.pixelDistance = Vector2.Distance(entity.head2D, new Vector2(screenSize.X / 2, screenSize.Y / 2));

        entity.spotted = mem.ReadBool(currentPawn, m_entitySpottedState + m_bSpotted);
        spotted = entity.spotted;

        entity.currentWeaponIndex = weaponDefinitionIndex;
        entity.currentWeaponName = Enum.GetName(typeof(Weapon), weaponDefinitionIndex);

        entity.name = mem.ReadString(currentController, m_iszPlayerName, 16).Split("\0")[0];
        entity.team = mem.ReadInt(currentPawn, m_iTeamNum);
        entity.health = mem.ReadInt(currentPawn, m_iHealth);
        entity.armor = mem.ReadInt(currentPawn, m_ArmorValue);
        entity.hasHeadArmor = mem.ReadBool(currentPawn, m_bHasHelmet);
        entity.hasDefuseKit = mem.ReadBool(currentPawn, m_bHasDefuser);

        entity.position = mem.ReadVec(currentPawn, m_vOldOrigin);
        entity.viewOffset = mem.ReadVec(currentPawn, m_vecViewOffset);
        entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
        entity.pos2D = Calculate.WorldToScreenUnclamped(viewMatrix, entity.position, screenSize);
        entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);
        entity.distance = Vector3.Distance(entity.position, localPlayer.position);
        entity.distance2 = Vector3.Distance(entity.origin, localPlayer.origin);
        entity.skeleton = Calculate.ReadBones(boneMatrix, mem);
        entity.Skeleton2D = Calculate.ReadBones2d(entity.skeleton, viewMatrix, screenSize);

        entities.Add(entity);
    }

    // update renderer data
    renderer.UpdateLocalPlayer(localPlayer);
    renderer.UpdateSpectators(spectatorNames);
    renderer.UpdateEntities(entities);

    // sorting type
    if (renderer.selectedSorting == 0)
        entities = entities.OrderBy(o => o.pixelDistance).ToList(); // from closest to the crosshair to the furthest

    if (renderer.selectedSorting == 1)
        entities = entities.OrderBy(o => o.distance2).ToList(); // from closest to the player to the furthest

    if (renderer.selectedSorting == 2)
        entities = entities.OrderBy(o => o.health).ToList(); // from lowest health to highest

    if (renderer.selectedSorting == 3)
        entities = entities.OrderBy(o => o.armor).ToList(); // from lowest armor to highest

    // update first entity
    if (entities.Count > 0)
        renderer.UpdateFirstEntity(entities[0]);

    // check if spotted
    if (spotted == false && renderer.enableAimbotSpotted) continue;

    // execute aimbot
    if (entities.Count > 0 && GetAsyncKeyState(aimbotHotkey) < 0 && renderer.enableAimbot)
    {
        // get view angles
        Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
        Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

        if (renderer.enableCircleAimbot && entities[0].pixelDistance > renderer.circleSize) continue;

        // Predict target position based on velocity
        Vector3 predictedTarget = entities[0].head + entities[0].velocity * 0.01f;

        // get current angles
        Vector3 currentAnglesVec3 = mem.ReadVec(client, dwViewAngles);
        Vector2 currentAngles = new Vector2(currentAnglesVec3.Y, currentAnglesVec3.X);

        // get target angles
        Vector2 targetAngles = Calculate.CalculateAngles(playerView, predictedTarget);

        // calculate delta
        Vector2 delta = targetAngles - currentAngles;

        // normalize delta
        delta.X = Calculate.NormalizeYaw(delta.X); // X = yaw
        delta.Y = Calculate.NormalizePitch(delta.Y); // Y = pitch

        // apply smoothing
        float smoothFactor = renderer.aimbotSmoothing;
        delta.X /= smoothFactor;
        delta.Y /= smoothFactor;

        // compute smoothed angles
        Vector2 smoothedAngles = currentAngles + delta;

        // clamp if necessary (optional, for safety)
        smoothedAngles.Y = Math.Clamp(smoothedAngles.Y, -89f, 89f); // Pitch
        smoothedAngles.X = Calculate.NormalizeYaw(smoothedAngles.X); // Yaw

        // prepare to write
        Vector3 newAnglesVec3 = new Vector3(smoothedAngles.Y, smoothedAngles.X, 0.0f);

        // write new angles
        mem.WriteVec(client, dwViewAngles, newAnglesVec3);
    }
}
// hotkey import
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vkey);