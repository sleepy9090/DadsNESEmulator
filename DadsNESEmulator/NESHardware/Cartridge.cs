/**
 *  @file           Cartridge.cs
 *  @brief          Defines the Cartridge.
 *  
 *  @copyright      2020
 *  @date           1/07/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */

using System.Collections;
using System.Dynamic;
using System.IO;

namespace DadsNESEmulator.NESHardware
{
    public class Cartridge
    {

        public string Path
        {
            get;
            protected set;
        }

        public string Identification
        {
            get;
            protected set;
        }

        public byte PRGROMSize
        {
            get;
            protected set;
        }

        public byte CHRROMSize
        {
            get;
            protected set;
        }

        public bool hasVerticalMirroring
        {
            get;
            protected set;
        }

        public bool hasHorizontalMirroring
        {
            get;
            protected set;
        }

        public bool hasBatteryBackedPRGRAM
        {
            get;
            protected set;
        }

        public bool hasTrainer
        {
            get;
            protected set;
        }

        public bool has4ScreenVRAM
        {
            get;
            protected set;
        }

        public bool isVSUnisystem
        {
            get;
            protected set;
        }

        public bool isPlayChoice10
        {
            get;
            protected set;
        }

        public bool isNES20Format
        {
            get;
            protected set;
        }

        public Cartridge(string path)
        {
            Path = path;
            ParseHeader();
        }

        private void ParseHeader()
        {
            // iNES
            byte flags6;
            byte flags7;
            byte flags8;
            byte flags9;

            // NES 2.0
            byte flags10;
            byte flags11;
            byte flags12;
            byte flags13;
            byte flags14;
            byte flags15;

            using (BinaryReader nesProgramBinaryReader = new BinaryReader(File.Open(Path, FileMode.Open)))
            {
                Identification = new string(nesProgramBinaryReader.ReadChars(4));  // flags(0..3)
                PRGROMSize = nesProgramBinaryReader.ReadByte(); // flags4
                CHRROMSize = nesProgramBinaryReader.ReadByte(); // flags5
                flags6 = nesProgramBinaryReader.ReadByte();
                flags7 = nesProgramBinaryReader.ReadByte();
                flags8 = nesProgramBinaryReader.ReadByte();
                flags9 = nesProgramBinaryReader.ReadByte();
                flags10 = nesProgramBinaryReader.ReadByte();
                flags11 = nesProgramBinaryReader.ReadByte();
                flags12 = nesProgramBinaryReader.ReadByte();
                flags13 = nesProgramBinaryReader.ReadByte();
                flags14 = nesProgramBinaryReader.ReadByte();
                flags15 = nesProgramBinaryReader.ReadByte();

            };
            
        }

        private void ParseFlags6(byte flags)
        {
            /*
             * 76543210
             * ||||||||
             * |||||||+- Mirroring: 0: horizontal (vertical arrangement) (CIRAM A10 = PPU A11)
             * |||||||              1: vertical (horizontal arrangement) (CIRAM A10 = PPU A10)
             * ||||||+-- 1: Cartridge contains battery-backed PRG RAM ($6000-7FFF) or other persistent memory
             * |||||+--- 1: 512-byte trainer at $7000-$71FF (stored before PRG data)
             * ||||+---- 1: Ignore mirroring control or above mirroring bit; instead provide four-screen VRAM
             * ++++----- Lower nybble of mapper number
             */
             BitArray flags6 = new BitArray(new byte[] {flags});
             if (flags6[0])
             {
                 hasHorizontalMirroring = flags6[0];
                 hasVerticalMirroring = !flags6[0];
             }
             else
             {
                 hasHorizontalMirroring = !flags6[0];
                 hasVerticalMirroring = flags6[0];
             }

            hasBatteryBackedPRGRAM = flags6[1];
            hasTrainer = flags6[2];
            has4ScreenVRAM = flags6[3];

            byte lowerMapperNibble = (byte)((flags & 0xF0) >> 4);
            /*
             * byte x = 0xA7;  // For example...
                byte nibble1 = (byte) (x & 0x0F);
                byte nibble2 = (byte)((x & 0xF0) >> 4);
                // Or alternatively...
                nibble2 = (byte)((x >> 4) & 0x0F);
                byte original = (byte)((nibble2 << 4) | nibble1);
             */
        }

        private void ParseFlags7(byte flags)
        {
            /*
             * 76543210
             * ||||||||
             * |||||||+- VS Unisystem
             * ||||||+-- PlayChoice-10 (8KB of Hint Screen data stored after CHR data)
             * ||||++--- If equal to 2, flags 8-15 are in NES 2.0 format
             * ++++----- Upper nybble of mapper number
             */
            BitArray flags7 = new BitArray(new byte[] { flags });

            isNES20Format = false;

            isVSUnisystem = flags7[0];
            isPlayChoice10 = flags7[1];
        }

        private void ParseFlags8(byte flags)
        {
            /*
             * 76543210
             * ||||||||
             * ++++++++- PRG RAM size
             */
            BitArray flags8 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags9(byte flags)
        {
            /*
             * 76543210
             * ||||||||
             * |||||||+- TV system (0: NTSC; 1: PAL)
             * +++++++-- Reserved, set to zero
             */
            BitArray flags9 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags10(byte flags)
        {
            /*
             * 76543210
             * ||  ||
             * ||  ++- TV system (0: NTSC; 2: PAL; 1/3: dual compatible)
             * |+----- PRG RAM ($6000-$7FFF) (0: present; 1: not present)
             * +------ 0: Board has no bus conflicts; 1: Board has bus conflicts
             */
            BitArray flags10 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags11(byte flags)
        {
            BitArray flags11 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags12(byte flags)
        {
            BitArray flags12 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags13(byte flags)
        {
            BitArray flags13 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags14(byte flags)
        {
            BitArray flags14 = new BitArray(new byte[] { flags });
        }

        private void ParseFlags15(byte flags)
        {
            BitArray flags15 = new BitArray(new byte[] { flags });
        }
    }
}