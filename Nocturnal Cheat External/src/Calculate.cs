using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ProcessMemory64;

namespace Noturnal_Cheat_External
{
    public static class Calculate
    {
        public static Vector2 WorldToScreen(float[] matrix, Vector3 pos, Vector2 windowSize)
        {
            // calculate screenW
            float screenW = (matrix[12] * pos.X) + (matrix[13] * pos.Y) + (matrix[14] * pos.Z) + matrix[15];
            
            // if entity is in front of us
            if (screenW > 0.001f)
            {
                // calculate screen X and Y
                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                // perform perspective division
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                // return coordinates
                return new Vector2 (X, Y);
            }
            else
            {
                return new Vector2 (-99, -99);
            }
        }

        private const float Zoom = 1.0f;

        public static Vector2 WorldToScreenUnclamped(float[] matrix, Vector3 pos, Vector2 windowSize)
        {
            // calculate screenW
            float screenW = (matrix[12] * pos.X) + (matrix[13] * pos.Y) + (matrix[14] * pos.Z) + matrix[15];

            // if entity is in front of us
            if (screenW > 0.001f)
            {
                // calculate screen X and Y
                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                // perform perspective division
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                // return coordinates
                return new Vector2(X, Y);
            }
            else
            {
                // calculate screen X and Y
                float screenX = (matrix[0] * pos.X) + (matrix[1] * pos.Y) + (matrix[2] * pos.Z) + matrix[3];
                float screenY = (matrix[4] * pos.X) + (matrix[5] * pos.Y) + (matrix[6] * pos.Z) + matrix[7];

                // perform perspective division
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                // return coordinates
                return new Vector2(-X, -Y);
            }
        }

        public static Vector2 CalculateAngles(Vector3 from, Vector3 to)
        {
            float yaw;
            float pitch;

            // calculate yaw
            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;
            yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI); // convert to degrees

            // calculate pitch

            float deltaZ = to.Z - from.Z;
            double distance = Math.Sqrt(Math.Pow(deltaX,2) + Math.Pow(deltaY,2));
            pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI); // convert to degrees

            return new Vector2(yaw, pitch);
        }

        public static float NormalizeYaw(float yaw)
        {
            while (yaw > 180f) yaw -= 360f;
            while (yaw < -180f) yaw += 360f;
            return yaw;
        }

        public static float NormalizePitch(float pitch)
        {
            while (pitch > 180f) pitch -= 360f;
            while (pitch < -180f) pitch += 360f;
            if (pitch > 89f) pitch = 89f;
            if (pitch < -89f) pitch = -89f;
            return pitch;
        }

        public static List<Vector3> ReadBones(IntPtr boneAddress, ProcessMemory mem)
        {
            byte[] boneBytes = mem.ReadBytes(boneAddress, 27 * 32 + 16); // get max, 27 = id, 32 = step
            List<Vector3> bones = new List<Vector3>();
            foreach (var BoneId in Enum.GetValues(typeof(BoneIds))) // loop through enum
            {
                float x = BitConverter.ToSingle(boneBytes, (int)BoneId * 32 + 0);
                float y = BitConverter.ToSingle(boneBytes, (int)BoneId * 32 + 4); // float = 4 bytes
                float z = BitConverter.ToSingle(boneBytes, (int)BoneId * 32 + 8);
                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }
            return bones;
        }
        public static List<Vector2> ReadBones2d(List<Vector3> bones, float[] viewMatrix, Vector2 screenSize)
        {
            List<Vector2> bones2d = new List<Vector2>();
            foreach (Vector3 bone in bones)
            {
                Vector2 bone2d = WorldToScreen(viewMatrix, bone, screenSize);
                bones2d.Add(bone2d);
            }
            return bones2d;
        }
    }
}
