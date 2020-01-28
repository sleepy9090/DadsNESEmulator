/**
 *  @file           Memory.cs
 *  @brief          Defines the Memory.
 *  
 *  @copyright      2019
 *  @date           1/28/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */

using System;

namespace DadsNESEmulator.NESHardware
{
    public class Memory
    {
        private byte[] _memory
        {
            get;
            set;
        }

        public Memory(int size)
        {
            _memory = new byte[size];
        }

        public byte Read(ushort address)
        {
            return _memory[address];
        }

        public void Write(ushort address, byte value)
        {
            _memory[address] = value;
        }

        public void Write(ushort address, byte[] value)
        {
            Array.Copy(value, 0, _memory, address, value.Length);
        }
    }
}