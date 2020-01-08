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

        public byte PRGROMSizeLSB
        {
            get;
            protected set;
        }

        public byte CHRROMSizeLSB
        {
            get;
            protected set;
        }

        public byte PRGROMSizeMSB
        {
            get;
            protected set;
        }

        public byte CHRROMSizeMSB
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

        public bool isNESFamilyComputer
        {
            get;
            protected set;
        }

        public bool isExtendedConsoleType
        {
            get;
            protected set;
        }

        public bool isiNESFormat
        {
            get;
            protected set;
        }

        public bool isNES20Format
        {
            get;
            protected set;
        }

        public byte Flag6MapperNibble
        {
            get;
            protected set;
        }

        public byte Flag7MapperNibble
        {
            get;
            protected set;
        }

        public byte Flag8MapperNibble
        {
            get;
            protected set;
        }

        public byte MapperByte
        {
            get;
            protected set;
        }

        public byte SubMapperByte
        {
            get;
            protected set;
        }

        public ushort PRGRAMSize
        {
            get;
            protected set;
        }

        public ushort PRGNVRAMSize
        {
            get;
            protected set;
        }

        public ushort CHRRAMSize
        {
            get;
            protected set;
        }

        public ushort CHRNVRAMSize
        {
            get;
            protected set;
        }

        public byte CPUPPUTimingMode
        {
            get;
            protected set;
        }

        public byte VSPPUType
        {
            get;
            protected set;
        }

        public byte VSHardwareType
        {
            get;
            protected set;
        }

        public byte NumberOfMiscellaneousROMs
        {
            get;
            protected set;
        }

        public byte DefaultExpansionDevice
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
                PRGROMSizeLSB = nesProgramBinaryReader.ReadByte(); // flags4
                CHRROMSizeLSB = nesProgramBinaryReader.ReadByte(); // flags5
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

            if (Identification[0] == 'N' 
                && Identification[1] == 'E' 
                && Identification[2] == 'S' 
                && Identification[3] == 0x1A)
            {
                isiNESFormat = true;
            }

            if (isiNESFormat)
            {
                ParseFlags6(flags6);
                ParseFlags7(flags7);
                ParseFlags8(flags8);
                ParseFlags9(flags9);
                ParseFlags10(flags10);

                if (isNES20Format)
                {
                    ParseFlags11(flags11);
                    ParseFlags12(flags12);
                    ParseFlags13(flags13);
                    ParseFlags14(flags14);
                    ParseFlags15(flags15);
                }
            }

            
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

            Flag6MapperNibble = (byte)((flags & 0xF0) >> 4);
            /*
             * Get upper and lower nibble
             * byte x = 0xA7;
             * byte nibble1 = (byte) (x & 0x0F);       <-- 7 (last 4 bits)
             * byte nibble2 = (byte)((x & 0xF0) >> 4); <-- A (first 4 bits)
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

            if (flags7[2] == false && flags7[3] == true)
            {
                isNES20Format = true;
            }

            if (isNES20Format)
            {
                /*
                 * 7654 3210
                 * ---------
                 * NNNN 10TT
                 * |||| ||++- Console type
                 * |||| ||     0: Nintendo Entertainment System/Family Computer
                 * |||| ||     1: Nintendo Vs. System
                 * |||| ||     2: Nintendo Playchoice 10
                 * |||| ||     3: Extended Console Type
                 * |||| ++--- NES 2.0 identifier
                 * ++++------ Mapper Number D4..D7
                 */
                if (flags7[0] && flags7[1])
                {
                    isExtendedConsoleType = true;
                }
                else if (flags7[0] && !flags7[1])
                {
                    isPlayChoice10 = true;
                }
                else if (!flags7[0] && flags7[1])
                {
                    isVSUnisystem = true;
                }
                else
                {
                    isNESFamilyComputer = true;
                }
            }
            else
            {
                isVSUnisystem = flags7[0];
                isPlayChoice10 = flags7[1];
            }

            Flag7MapperNibble = (byte)((flags & 0xF0) >> 4);
            /*
             * Get upper and lower nibble
             * byte x = 0xA7;
             * byte nibble1 = (byte) (x & 0x0F);       <-- 7 (last 4 bits)
             * byte nibble2 = (byte)((x & 0xF0) >> 4); <-- A (first 4 bits)
             */

            string mapperHexString = Flag6MapperNibble.ToString() + Flag7MapperNibble.ToString();
            int mapperInt = int.Parse(mapperHexString, System.Globalization.NumberStyles.HexNumber);
            MapperByte = (byte)mapperInt;
        }

        private void ParseFlags8(byte flags)
        {
            BitArray flags8 = new BitArray(new byte[] { flags });

            if (isNES20Format)
            {
                /*
                 * 7654 3210
                 * ---------
                 * SSSS NNNN
                 * |||| ++++- Mapper number D8..D11
                 * ++++------ Submapper number
                 */
                Flag8MapperNibble = (byte)(flags & 0x0F);
                SubMapperByte = (byte)((flags & 0xF0) >> 4);
            }
            else
            {
                /*
                 * 76543210
                 * ||||||||
                 * ++++++++- PRG RAM size
                 */
                PRGRAMSize = flags;
            }
        }

        private void ParseFlags9(byte flags)
        {
            BitArray flags9 = new BitArray(new byte[] { flags });

            if (isNES20Format)
            {
                /*
                 * 7654 3210
                 * ---------
                 * CCCC PPPP
                 * |||| ++++- PRG-ROM size MSB
                 * ++++------ CHR-ROM size MSB
                 */
                PRGROMSizeMSB = (byte)(flags & 0x0F);
                CHRROMSizeMSB = (byte)((flags & 0xF0) >> 4);
            }
            else
            {
                /*
                 * 76543210
                 * ||||||||
                 * |||||||+- TV system (0: NTSC; 1: PAL)
                 * +++++++-- Reserved, set to zero
                 */
                if (flags9[0])
                {
                    CPUPPUTimingMode = 1;
                }
                else
                {
                    CPUPPUTimingMode = 0;
                }
            }
        }

        private void ParseFlags10(byte flags)
        {
            BitArray flags10 = new BitArray(new byte[] { flags });

            if (isNES20Format)
            {
                /*
                 * 7654 3210
                 * ---------
                 * pppp PPPP
                 * |||| ++++- PRG-RAM (volatile) shift count
                 * ++++------ PRG-NVRAM/EEPROM (non-volatile) shift count
                 * If the shift count is zero, there is no PRG-(NV)RAM.
                 * If the shift count is non-zero, the actual size is
                 * "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
                 */
                byte PRGRAMShiftCount = (byte)(flags & 0x0F);
                byte PRGNVRAMShiftCount = (byte)((flags & 0xF0) >> 4);

                if (PRGRAMShiftCount == 0)
                {
                    PRGRAMSize = 0;
                }
                else
                {
                    PRGRAMSize = (ushort) (64 << PRGRAMShiftCount);
                }

                if (PRGNVRAMShiftCount == 0)
                {
                    PRGNVRAMSize = 0;
                }
                else
                {
                    PRGNVRAMSize = (ushort)(64 << PRGNVRAMShiftCount);
                }

            }
            else
            {

                /*
                 * 76543210
                 * ||  ||
                 * ||  ++- TV system (0: NTSC; 2: PAL; 1/3: dual compatible)
                 * |+----- PRG RAM ($6000-$7FFF) (0: present; 1: not present)
                 * +------ 0: Board has no bus conflicts; 1: Board has bus conflicts
                 */

                //This byte is not part of the official specification, and relatively few emulators honor it. 
            }

        }

        private void ParseFlags11(byte flags)
        {
            BitArray flags11 = new BitArray(new byte[] { flags });

            /*
             * 7654 3210
             * ---------
             * cccc CCCC
             * |||| ++++- CHR-RAM size (volatile) shift count
             * ++++------ CHR-NVRAM size (non-volatile) shift count
             * If the shift count is zero, there is no CHR-(NV)RAM.
             * If the shift count is non-zero, the actual size is
             * "64 << shift count" bytes, i.e. 8192 bytes for a shift count of 7.
             */
            byte CHRRAMShiftCount = (byte)(flags & 0x0F);
            byte CHRNVRAMShiftCount = (byte)((flags & 0xF0) >> 4);

            if (CHRRAMShiftCount == 0)
            {
                CHRRAMSize = 0;
            }
            else
            {
                CHRRAMSize = (ushort)(64 << CHRRAMShiftCount);
            }

            if (CHRNVRAMShiftCount == 0)
            {
                CHRNVRAMSize = 0;
            }
            else
            {
                CHRNVRAMSize = (ushort)(64 << CHRNVRAMShiftCount);
            }
        }

        private void ParseFlags12(byte flags)
        {
            BitArray flags12 = new BitArray(new byte[] { flags });

            /*
             * 7654 3210
             * ---------
             * .... ..VV
             *        ++- CPU/PPU timing mode
             *             0: RP2C02 ("NTSC NES")
             *             1: RP2C07 ("Licensed PAL NES")
             *             2: Multiple-region
             *             3: UMC 6527P ("Dendy")
             */
            if (flags12[0] && flags12[1])
            {
                CPUPPUTimingMode = 3;
            }
            else if (flags12[0] && !flags12[1])
            {
                CPUPPUTimingMode = 1;
            }
            else if (!flags12[0] && flags12[1])
            {
                CPUPPUTimingMode = 2;
            }
            else
            {
                CPUPPUTimingMode = 0;
            }
        }

        private void ParseFlags13(byte flags)
        {
            BitArray flags13 = new BitArray(new byte[] { flags });

            /*
             * 7654 3210
             * ---------
             * MMMM PPPP
             * |||| ++++- Vs. PPU Type
             * ++++------ Vs. Hardware Type
             */
            VSPPUType = (byte)(flags & 0x0F);
            VSHardwareType = (byte)((flags & 0xF0) >> 4);
        }

        private void ParseFlags14(byte flags)
        {
            BitArray flags14 = new BitArray(new byte[] { flags });

            /*
             * 7654 3210
             * ---------
             * .... ..RR
             *        ++- Number of miscellaneous ROMs present
             */
            if (flags14[0] && flags14[1])
            {
                NumberOfMiscellaneousROMs = 3;
            }
            else if (flags14[0] && !flags14[1])
            {
                NumberOfMiscellaneousROMs = 1;
            }
            else if (!flags14[0] && flags14[1])
            {
                NumberOfMiscellaneousROMs = 2;
            }
            else
            {
                NumberOfMiscellaneousROMs = 0;
            }
            
        }

        private void ParseFlags15(byte flags)
        {
            BitArray flags15 = new BitArray(new byte[] { flags });

            /*
             * 7654 3210
             * ---------
             * ..DD DDDD
             *   ++-++++- Default Expansion Device
             */
            flags15[7] = false;
            flags15[6] = false;

            byte[] bytes = new byte[1];
            flags15.CopyTo(bytes, 0);

            DefaultExpansionDevice = bytes[0];
        }
    }
}