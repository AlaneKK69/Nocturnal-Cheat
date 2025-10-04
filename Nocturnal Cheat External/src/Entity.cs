using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Noturnal_Cheat_External
{
    public class Entity
    {
        public string name { get; set; }
        public string currentWeaponName { get; set; }
        public IntPtr pawnAddress { get; set; }
        public IntPtr controllerAddress { get; set; }
        public List<Vector3> skeleton {  get; set; }
        public List<Vector2> Skeleton2D { get; set; }
        public Vector3 position {  get; set; }
        public Vector3 viewOffset { get; set; }
        public Vector2 position2D { get; set; }
        public Vector2 pos2D { get; set; }
        public Vector2 viewPosition2D { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 origin { get; set; }
        public Vector3 view { get; set; }
        public Vector3 head { get; set; }
        public Vector3 torso { get; set; }
        public Vector3 neck { get; set; }
        public Vector3 feet { get; set; }
        public Vector2 head2D { get; set; }
        public bool spotted { get; set; }
        public bool hasDefuseKit { get; set; }
        public bool hasHeadArmor { get; set; }
        public bool matchStarted { get; set; }
        public int health { get; set; }
        public int armor { get; set; }
        public int team { get; set; }
        public float distance { get; set; }
        public float distance2 { get; set; } // for aimbot
        public float pixelDistance { get; set; }
        public short currentWeaponIndex { get; set; }
    }

    public enum BoneIds // these are from the perspective of the enemy (HandRight, their right hand)
    {
        Waist = 0,
        Neck = 5,
        Head = 6,
        ShoulderLeft = 8,
        ForeLeft = 9,
        HandLeft = 11,
        ShoulderRight = 13,
        ForeRight = 14,
        HandRight = 16,
        KneeLeft = 23,
        FeetLeft = 24,
        KneeRight = 26,
        FeetRight = 27
    }

    public enum Weapon
    {
        // weapons
        Deagle = 1,
        Elite = 2,
        FiveSeven = 3,
        Glock = 4,
        AK47 = 7,
        AUG = 8,
        AWP = 9,
        Famas = 10,
        G3Sg1 = 11,
        Galil = 13,
        M249 = 14,
        M4A4 = 16,
        Mac10 = 17,
        P90 = 19,
        MP5 = 23,
        UMP45 = 24,
        XM1014 = 25,
        PPBizon = 26,
        MAG7 = 27,
        Negev = 28,
        SawedOff = 29,
        Tec9 = 30,
        Zeus = 31,
        P2000 = 32,
        MP7 = 33,
        MP9 = 34,
        Nova = 35,
        P250 = 36,
        Scar20 = 38,
        SG556 = 39,
        Scout = 40,
        Flashbang = 43,
        Grenade = 44,
        Smoke = 45,
        Molotov = 46,
        Decoy = 47,
        Incendiary = 48,
        C4 = 49,
        MediShot = 57,
        M4A1_S = 60,
        USP = 61,
        CZ75 = 63,
        Revolver = 64,

        // knifes
        CTKnife = 42,
        TKnife = 59,
        Bayonet = 500,
        Bowie = 514,
        Butterfly = 515,
        Falchion = 512,
        FlipKnife = 505,
        GutKnife = 506,
        Karambit = 507,
        M9Bayonet = 508,
        Huntsman = 509,
        ShadowDaggers = 516,
        Talon = 518,
        Ursus = 519,
        Navaja = 520,
        Stiletto = 521,
        ClassicKnife = 522,
        Paracord = 523,
        SurvivalKnife = 524,
        SkeletonKnife = 525
    }
}
