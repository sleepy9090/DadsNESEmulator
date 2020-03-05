/**
 *  @file           PPUMemoryMap.cs
 *  @brief          Defines the PPU Memory Map.
 *  
 *  @copyright      2020
 *  @date           03/05/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
namespace DadsNESEmulator.NESHardware
{
    public class PPUMemoryMap
    {
        public byte Read(ushort address)
        {
            return 0x0;
        }

        public byte Write(ushort address, byte value)
        {
            return 0x0;
        }
    }
}