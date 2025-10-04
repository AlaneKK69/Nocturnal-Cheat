using System.Numerics;
using System.Text.Json.Nodes;

namespace Noturnal_Cheat_External
{
    public class ConfigCreator
    {
        // bools
        public bool Boxes { get; set; }
        public bool Lines { get; set; }
        public bool Arrow { get; set; }
        public bool Skeleton { get; set; }
        public bool HealthBar { get; set; }
        public bool ArmorBar { get; set; }
        public bool Names { get; set; }
        public bool WeaponNames { get; set; }
        public bool SkeletonColorSeen { get; set; }
        public bool Aimbot { get; set; }
        public bool CircleAimbot { get; set; }
        public bool AimbotSpotted { get; set; }
        public bool TriggerBot { get; set; }
        public bool BunnyHop { get; set; }
        public bool AntiRecoil { get; set; }
        public bool AntiFlash { get; set; }
        public bool BoxOutline { get; set; }
        public bool HealthOutline { get; set; }
        public bool ArmorOutline { get; set; }
        public bool NoSniperScope { get; set; }
        public bool NoScopeCrosshair { get; set; }
        // ints
        public int fov { get; set; }
        public int circleSize { get; set; }
        public int aimbotSmoothing { get; set; }
        public int selectedHealth {  get; set; }
        public int selectedSorting { get; set; }
        // colors
        public Vector4 BoxColor { get; set; }
        public Vector4 LineColor { get; set; }
        public Vector4 SpottedColor { get; set; }
        public Vector4 ArrowColor { get; set; }
        public Vector4 SkeletonColor { get; set; }
        public Vector4 CircleColor { get; set; }
        public Vector4 WeaponNameColor { get; set; }
        public Vector4 NameColor { get; set; }
    }
}
