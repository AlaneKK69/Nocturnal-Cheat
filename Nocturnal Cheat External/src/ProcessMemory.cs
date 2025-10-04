// Read/Write class
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessMemory64;

public class ProcessMemory
{
    private Process proc;

    [DllImport("Kernel32.dll")]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    public ProcessMemory(string procName)
    {
        proc = SetProcess(procName);
    }

    public Process GetProcess()
    {
        return proc;
    }

    public Process SetProcess(string procName)
    {
        proc = Process.GetProcessesByName(procName)[0];
        if (proc == null)
        {
            throw new InvalidOperationException("Process was not found, are you using the correct bit version and have no miss-spellings?");
        }

        return proc;
    }

    public IntPtr GetModuleBase(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            throw new InvalidOperationException("moduleName was either null or empty.");
        }

        if (proc == null)
        {
            throw new InvalidOperationException("process is invalid, check your init.");
        }

        try
        {
            if (moduleName.Contains(".exe") && proc.MainModule != null)
            {
                return proc.MainModule.BaseAddress;
            }

            foreach (ProcessModule module in proc.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    return module.BaseAddress;
                }
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Failed to find the specified module, search string might have miss-spellings.");
        }

        return IntPtr.Zero;
    }

    public IntPtr ReadPointer(IntPtr addy)
    {
        byte[] array = new byte[8];
        ReadProcessMemory(proc.Handle, addy, array, array.Length, IntPtr.Zero);
        return (IntPtr)BitConverter.ToInt64(array);
    }

    public IntPtr ReadPointer(IntPtr addy, int offset)
    {
        byte[] array = new byte[8];
        ReadProcessMemory(proc.Handle, addy + offset, array, array.Length, IntPtr.Zero);
        return (IntPtr)BitConverter.ToInt64(array);
    }

    public IntPtr ReadPointer(long addy, long offset)
    {
        byte[] array = new byte[8];
        ReadProcessMemory(proc.Handle, (IntPtr)(addy + offset), array, array.Length, IntPtr.Zero);
        return (IntPtr)BitConverter.ToInt64(array);
    }

    public IntPtr ReadPointer(IntPtr addy, long offset)
    {
        return ReadPointer(addy, offset);
    }

    public IntPtr ReadPointer(IntPtr addy, int[] offsets)
    {
        IntPtr intPtr = addy;
        foreach (int offset in offsets)
        {
            intPtr = ReadPointer(intPtr, offset);
        }

        return intPtr;
    }

    public IntPtr ReadPointer(IntPtr addy, long[] offsets)
    {
        IntPtr intPtr = addy;
        foreach (long offset in offsets)
        {
            intPtr = ReadPointer(intPtr, offset);
        }

        return intPtr;
    }

    public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2)
    {
        return ReadPointer(addy, new int[2] { offset1, offset2 });
    }

    public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3)
    {
        return ReadPointer(addy, new int[3] { offset1, offset2, offset3 });
    }

    public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4)
    {
        return ReadPointer(addy, new int[4] { offset1, offset2, offset3, offset4 });
    }

    public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5)
    {
        return ReadPointer(addy, new int[5] { offset1, offset2, offset3, offset4, offset5 });
    }

    public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5, int offset6)
    {
        return ReadPointer(addy, new int[6] { offset1, offset2, offset3, offset4, offset5, offset6 });
    }

    public IntPtr ReadPointer(IntPtr addy, int offset1, int offset2, int offset3, int offset4, int offset5, int offset6, int offset7)
    {
        return ReadPointer(addy, new int[7] { offset1, offset2, offset3, offset4, offset5, offset6, offset7 });
    }

    public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2)
    {
        return ReadPointer(addy, new long[2] { offset1, offset2 });
    }

    public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3)
    {
        return ReadPointer(addy, new long[3] { offset1, offset2, offset3 });
    }

    public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4)
    {
        return ReadPointer(addy, new long[4] { offset1, offset2, offset3, offset4 });
    }

    public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4, long offset5)
    {
        return ReadPointer(addy, new long[5] { offset1, offset2, offset3, offset4, offset5 });
    }

    public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4, long offset5, long offset6)
    {
        return ReadPointer(addy, new long[6] { offset1, offset2, offset3, offset4, offset5, offset6 });
    }

    public IntPtr ReadPointer(IntPtr addy, long offset1, long offset2, long offset3, long offset4, long offset5, long offset6, long offset7)
    {
        return ReadPointer(addy, new long[7] { offset1, offset2, offset3, offset4, offset5, offset6, offset7 });
    }

    public IntPtr ReadPointer(IntPtr addy, IntPtr offset1, int offset2)
    {
        IntPtr addy2 = ReadPointer((IntPtr)((long)addy + (long)offset1));
        return ReadPointer(addy2, offset2);
    }

    public byte[] ReadBytes(IntPtr addy, int bytes)
    {
        byte[] array = new byte[bytes];
        ReadProcessMemory(proc.Handle, addy, array, array.Length, IntPtr.Zero);
        return array;
    }

    public byte[] ReadBytes(IntPtr addy, int offset, int bytes)
    {
        byte[] array = new byte[bytes];
        ReadProcessMemory(proc.Handle, addy + offset, array, array.Length, IntPtr.Zero);
        return array;
    }

    public int ReadInt(IntPtr address)
    {
        return BitConverter.ToInt32(ReadBytes(address, 4));
    }

    public int ReadInt(IntPtr address, int offset)
    {
        return BitConverter.ToInt32(ReadBytes(address + offset, 4));
    }

    public IntPtr ReadLong(IntPtr address)
    {
        return (IntPtr)BitConverter.ToInt64(ReadBytes(address, 8));
    }

    public IntPtr ReadLong(IntPtr address, int offset)
    {
        return (IntPtr)BitConverter.ToInt64(ReadBytes(address + offset, 8));
    }

    public float ReadFloat(IntPtr address)
    {
        return BitConverter.ToSingle(ReadBytes(address, 4));
    }

    public float ReadFloat(IntPtr address, int offset)
    {
        return BitConverter.ToSingle(ReadBytes(address + offset, 4));
    }

    public double ReadDouble(IntPtr address)
    {
        return BitConverter.ToDouble(ReadBytes(address, 8));
    }

    public double ReadDouble(IntPtr address, int offset)
    {
        return BitConverter.ToDouble(ReadBytes(address + offset, 4));
    }

    public Vector3 ReadVec(IntPtr address)
    {
        byte[] value = ReadBytes(address, 12);
        Vector3 result = default(Vector3);
        result.X = BitConverter.ToSingle(value, 0);
        result.Y = BitConverter.ToSingle(value, 4);
        result.Z = BitConverter.ToSingle(value, 8);
        return result;
    }

    public Vector3 ReadVec(IntPtr address, int offset)
    {
        byte[] value = ReadBytes(address + offset, 12);
        Vector3 result = default(Vector3);
        result.X = BitConverter.ToSingle(value, 0);
        result.Y = BitConverter.ToSingle(value, 4);
        result.Z = BitConverter.ToSingle(value, 8);
        return result;
    }

    public double[] ReadDoubleVec(IntPtr address)
    {
        byte[] value = ReadBytes(address, 24);
        return new double[3]
        {
            BitConverter.ToDouble(value, 0),
            BitConverter.ToDouble(value, 8),
            BitConverter.ToDouble(value, 16)
        };
    }

    public double[] ReadDoubleVec(IntPtr address, int offset)
    {
        byte[] value = ReadBytes(address + offset, 24);
        return new double[3]
        {
            BitConverter.ToDouble(value, 0),
            BitConverter.ToDouble(value, 8),
            BitConverter.ToDouble(value, 16)
        };
    }

    public short ReadShort(IntPtr address)
    {
        return BitConverter.ToInt16(ReadBytes(address, 2));
    }

    public short ReadShort(IntPtr address, int offset)
    {
        return BitConverter.ToInt16(ReadBytes(address + offset, 2));
    }

    public ushort ReadUShort(IntPtr address)
    {
        return BitConverter.ToUInt16(ReadBytes(address, 2));
    }

    public ushort ReadUShort(IntPtr address, int offset)
    {
        return BitConverter.ToUInt16(ReadBytes(address + offset, 2));
    }

    public uint ReadUInt(IntPtr address)
    {
        return BitConverter.ToUInt32(ReadBytes(address, 4));
    }

    public uint ReadUInt(IntPtr address, int offset)
    {
        return BitConverter.ToUInt32(ReadBytes(address + offset, 4));
    }

    public ulong ReadULong(IntPtr address)
    {
        return BitConverter.ToUInt64(ReadBytes(address, 8));
    }

    public ulong ReadULong(IntPtr address, int offset)
    {
        return BitConverter.ToUInt64(ReadBytes(address + offset, 8));
    }

    public bool ReadBool(IntPtr address)
    {
        return BitConverter.ToBoolean(ReadBytes(address, 1));
    }

    public bool ReadBool(IntPtr address, int offset)
    {
        return BitConverter.ToBoolean(ReadBytes(address + offset, 1));
    }

    public string ReadString(IntPtr address, int length)
    {
        return Encoding.UTF8.GetString(ReadBytes(address, length));
    }

    public string ReadString(IntPtr address, int offset, int length)
    {
        return Encoding.UTF8.GetString(ReadBytes(address + offset, length));
    }

    public char ReadChar(IntPtr address)
    {
        return BitConverter.ToChar(ReadBytes(address, 2));
    }

    public char ReadChar(IntPtr address, int offset)
    {
        return BitConverter.ToChar(ReadBytes(address + offset, 2));
    }

    public float[] ReadMatrix(IntPtr address)
    {
        byte[] array = ReadBytes(address, 64);
        float[] array2 = new float[array.Length];
        array2[0] = BitConverter.ToSingle(array, 0);
        array2[1] = BitConverter.ToSingle(array, 4);
        array2[2] = BitConverter.ToSingle(array, 8);
        array2[3] = BitConverter.ToSingle(array, 12);
        array2[4] = BitConverter.ToSingle(array, 16);
        array2[5] = BitConverter.ToSingle(array, 20);
        array2[6] = BitConverter.ToSingle(array, 24);
        array2[7] = BitConverter.ToSingle(array, 28);
        array2[8] = BitConverter.ToSingle(array, 32);
        array2[9] = BitConverter.ToSingle(array, 36);
        array2[10] = BitConverter.ToSingle(array, 40);
        array2[11] = BitConverter.ToSingle(array, 44);
        array2[12] = BitConverter.ToSingle(array, 48);
        array2[13] = BitConverter.ToSingle(array, 52);
        array2[14] = BitConverter.ToSingle(array, 56);
        array2[15] = BitConverter.ToSingle(array, 60);
        return array2;
    }

    public bool WriteBytes(IntPtr address, byte[] newbytes)
    {
        return WriteProcessMemory(proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
    }

    public bool WriteBytes(IntPtr address, int offset, byte[] newbytes)
    {
        return WriteProcessMemory(proc.Handle, address + offset, newbytes, newbytes.Length, IntPtr.Zero);
    }

    public bool WriteBytes(IntPtr address, string newbytes)
    {
        byte[] array = (from b in newbytes.Split(' ')
                        select Convert.ToByte(b, 16)).ToArray();
        return WriteProcessMemory(proc.Handle, address, array, array.Length, IntPtr.Zero);
    }

    public bool WriteBytes(IntPtr address, int offset, string newbytes)
    {
        byte[] array = (from b in newbytes.Split(' ')
                        select Convert.ToByte(b, 16)).ToArray();
        return WriteProcessMemory(proc.Handle, address + offset, array, array.Length, IntPtr.Zero);
    }

    public byte ReadByte(IntPtr address, int offset = 1)
    {
        return ReadBytes(address, offset, 1)[0];
    }

    public bool WriteDoubleVec(IntPtr address, double[] value)
    {
        byte[] array = new byte[24];
        byte[] bytes = BitConverter.GetBytes(value[0]);
        byte[] bytes2 = BitConverter.GetBytes(value[1]);
        byte[] bytes3 = BitConverter.GetBytes(value[2]);
        bytes.CopyTo(array, 0);
        bytes2.CopyTo(array, 8);
        bytes3.CopyTo(array, 16);
        return WriteBytes(address, array);
    }

    public bool WriteDoubleVec(IntPtr address, int offset, double[] value)
    {
        byte[] array = new byte[24];
        byte[] bytes = BitConverter.GetBytes(value[0]);
        byte[] bytes2 = BitConverter.GetBytes(value[1]);
        byte[] bytes3 = BitConverter.GetBytes(value[2]);
        bytes.CopyTo(array, 0);
        bytes2.CopyTo(array, 8);
        bytes3.CopyTo(array, 16);
        return WriteBytes(address + offset, array);
    }

    public bool WriteInt(IntPtr address, int value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteInt(IntPtr address, int offset, int value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteShort(IntPtr address, short value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteShort(IntPtr address, int offset, short value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteUShort(IntPtr address, ushort value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteUShort(IntPtr address, int offset, ushort value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteUInt(IntPtr address, uint value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteUInt(IntPtr address, int offset, uint value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteLong(IntPtr address, long value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteLong(IntPtr address, int offset, long value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteULong(IntPtr address, ulong value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteULong(IntPtr address, int offset, ulong value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteFloat(IntPtr address, float value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteFloat(IntPtr address, int offset, float value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteDouble(IntPtr address, double value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteDouble(IntPtr address, int offset, double value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteBool(IntPtr address, bool value)
    {
        return WriteBytes(address, BitConverter.GetBytes(value));
    }

    public bool WriteBool(IntPtr address, int offset, bool value)
    {
        return WriteBytes(address + offset, BitConverter.GetBytes(value));
    }

    public bool WriteString(IntPtr address, string value)
    {
        return WriteBytes(address, Encoding.UTF8.GetBytes(value));
    }

    public bool WriteVec(IntPtr address, Vector3 value)
    {
        byte[] array = new byte[12];
        byte[] bytes = BitConverter.GetBytes(value.X);
        byte[] bytes2 = BitConverter.GetBytes(value.Y);
        byte[] bytes3 = BitConverter.GetBytes(value.Z);
        bytes.CopyTo(array, 0);
        bytes2.CopyTo(array, 4);
        bytes3.CopyTo(array, 8);
        return WriteBytes(address, array);
    }

    public bool WriteVec(IntPtr address, int offset, Vector3 value)
    {
        byte[] array = new byte[12];
        byte[] bytes = BitConverter.GetBytes(value.X);
        byte[] bytes2 = BitConverter.GetBytes(value.Y);
        byte[] bytes3 = BitConverter.GetBytes(value.Z);
        bytes.CopyTo(array, 0);
        bytes2.CopyTo(array, 4);
        bytes3.CopyTo(array, 8);
        return WriteBytes(address + offset, array);
    }

    public bool Nop(IntPtr address, int length)
    {
        byte[] array = new byte[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = 144;
        }

        return WriteBytes(address, array);
    }

    public bool Nop(IntPtr address, int offset, int length)
    {
        byte[] array = new byte[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = 144;
        }

        return WriteBytes(address + offset, array);
    }

    public IntPtr ScanForBytes32(string moduleName, string needle)
    {
        string moduleName2 = moduleName;
        ProcessModule processModule = proc.Modules.OfType<ProcessModule>().FirstOrDefault((ProcessModule x) => x.ModuleName == moduleName2);
        if (processModule == null)
        {
            throw new InvalidOperationException("module was not found. Check your module name.");
        }

        byte[] needle2 = (from b in needle.Split(' ')
                          select Convert.ToByte(b, 16)).ToArray();
        if (processModule.FileName == null)
        {
            throw new InvalidOperationException("module filename was null.");
        }

        byte[] array;
        using (FileStream fileStream = new FileStream(processModule.FileName, FileMode.Open, FileAccess.Read))
        {
            array = new byte[fileStream.Length];
            fileStream.Read(array, 0, (int)fileStream.Length);
        }

        return (IntPtr)ScanForBytes32(array, needle2);
    }

    public long ScanForBytes32(byte[] haystack, byte[] needle)
    {
        for (int i = 0; i < haystack.Length - needle.Length; i++)
        {
            bool flag = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (needle[j] != byte.MaxValue && haystack[i + j] != needle[j])
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                return i;
            }
        }

        return -1L;
    }
}