/**
 *  @file           PPUMemoryMap.cs
 *  @brief          Defines the 2C02 PPU Memory Map.
 *  
 *  @copyright      2020
 *  @date           03/05/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */

using System;

namespace DadsNESEmulator.NESHardware
{
    public class PPUMemoryMap
    {
        /** - Reference:
         * http://www.fceux.com/web/help/fceux.html?NESRAMMappingFindingValues.html
         * https://wiki.nesdev.com/w/index.php/PPU_memory_map
         * https://everything2.com/title/NES%2520memory%2520map
         */

        /*
         *      __________________________________________
         * 0000| Pattern table 0                          |
         *     |__________________________________________|
         * 1000| Pattern table 1                          |
         *     |__________________________________________|   _____ _____
         * 2000| Nametable 0                              |  |     |     |
         *     |__________________________________________|  |  0  |  1  |
         * 2400| Nametable 1                              |  |_____|_____|
         *     |__________________________________________|  |     |     |
         * 2800| Nametable 2                              |  |  2  |  3  |
         *     |__________________________________________|  |_____|_____|
         * 2c00| Nametable 3                              |
         *     |__________________________________________|
         * 3000| Mirror of $2000-$2eff                    |
         *     |__________________________________________|
         * 3f00| Palette                                  |
         *     |__________________________________________|
         * 3f20| Mirrors of $3f00-$3f1f                   |
         *     |__________________________________________|
         */

        /*
         * Address range 	Size 	Description
         * $0000-$0FFF 	$1000 	Pattern table 0
         * $1000-$1FFF 	$1000 	Pattern table 1
         * $2000-$23FF 	$0400 	Nametable 0
         * $2400-$27FF 	$0400 	Nametable 1
         * $2800-$2BFF 	$0400 	Nametable 2
         * $2C00-$2FFF 	$0400 	Nametable 3
         * $3000-$3EFF 	$0F00 	Mirrors of $2000-$2EFF
         * $3F00-$3F1F 	$0020 	Palette RAM indexes
         * $3F20-$3FFF 	$00E0 	Mirrors of $3F00-$3F1F 
         */

        /*
         * Address 	Size 	Description
         * $0000 	$1000 	Pattern Table 0
         * $1000 	$1000 	Pattern Table 1
         * $2000 	$3C0 	Name Table 0
         * $23C0 	$40 	Attribute Table 0
         * $2400 	$3C0 	Name Table 1
         * $27C0 	$40 	Attribute Table 1
         * $2800 	$3C0 	Name Table 2
         * $2BC0 	$40 	Attribute Table 2
         * $2C00 	$3C0 	Name Table 3
         * $2FC0 	$40 	Attribute Table 3
         * $3000 	$F00 	Mirror of 2000h-2EFFh
         * $3F00 	$10 	BG Palette
         * $3F10 	$10 	Sprite Palette
         * $3F20 	$E0 	Mirror of 3F00h-3F1Fh 
         */

        // Total: 0x4000 / 16384kB
        private Memory patternTables = new Memory(0x2000); // 8192 ($0000-$0FFF + $1000-$1FFF)
        private Memory nameTables = new Memory(0x1F00); // 7936 ($2000-$23FF + $2400-$27FF + $2800-$2BFF + $2C00-$2FFF + $3000-$3EFF)
        private Memory palettes = new Memory(0x100); // 256 ($3F00-$3F1FF + $3F20-$3FFF)

        public byte ReadByte(ushort address)
        {
            byte byteRead;

            if (address < 0x2000)
            {
                byteRead = patternTables.Read(address);
            }
            else if (address < 0x3F00)
            {
                byteRead = nameTables.Read((byte)(address - 0x2000));
            }
            else if (address < 0x4000)
            {
                byteRead = palettes.Read((byte)(address - 0x3F00));
            }
            else
            {
                Console.WriteLine(string.Format(@"Invalid read from PPU address: {0:X4}", address));
                throw new Exception(string.Format(@"Invalid read from PPU address: {0:X4}", address));
            }
            return byteRead;
        }

        public void WriteByte(ushort address, byte value)
        {
            if (address < 0x2000)
            {
                patternTables.Write(address, value);
            }
            else if (address < 0x3F00)
            {
                nameTables.Write((byte)(address - 0x2000), value);
            }
            else if (address < 0x4000)
            {
                palettes.Write((byte)(address - 0x3F00), value);
            }
            else
            {
                Console.WriteLine(string.Format(@"Invalid write to PPU address: {0:X4}", address));
                throw new Exception(string.Format(@"Invalid write to PPU address: {0:X4}", address));
            }
        }
    }
}