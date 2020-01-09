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
using System.Text;
using DadsNESEmulator.Types;

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

        public int PRGROMSize
        {
            get;
            protected set;
        }

        public int CHRROMSize
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

        public bool isVsUnisystem
        {
            get;
            protected set;
        }

        public bool isPlaychoice10
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

        public CPUPPUTiming CPUPPUTimingMode
        {
            get;
            protected set;
        }

        public VsSystemPPUType VsPPUType
        {
            get;
            protected set;
        }

        public VsSystemHardwareType VsHardwareType
        {
            get;
            protected set;
        }

        public ExtendedConsoleType ExtendedConsType
        {
            get;
            protected set;
        }

        public byte NumberOfMiscellaneousROMs
        {
            get;
            protected set;
        }

        public DefaultExpansionDevice DefaultExpansionDevice
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
                else
                {
                    PRGROMSize = 16384 * PRGROMSizeLSB;
                    CHRROMSize = 8192 * CHRROMSizeLSB;
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
                    isPlaychoice10 = true;
                }
                else if (!flags7[0] && flags7[1])
                {
                    isVsUnisystem = true;
                }
                else
                {
                    isNESFamilyComputer = true;
                }
            }
            else
            {
                isVsUnisystem = flags7[0];
                isPlaychoice10 = flags7[1];
            }

            Flag7MapperNibble = (byte)((flags & 0xF0) >> 4);
            /*
             * Get upper and lower nibble
             * byte x = 0xA7;
             * byte nibble1 = (byte) (x & 0x0F);       <-- 7 (last 4 bits)
             * byte nibble2 = (byte)((x & 0xF0) >> 4); <-- A (first 4 bits)
             */

            string mapperHexString = Flag7MapperNibble.ToString() + Flag6MapperNibble.ToString();
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

                if (PRGROMSizeMSB == 0xF)
                {

                }
                else
                {
                    int temp = PRGROMSizeMSB + PRGROMSizeLSB;
                    PRGROMSize = temp * 16384;
                    //string prgHexString = PRGROMSizeMSB.ToString() + PRGROMSizeLSB.ToString();
                    //string prgHexString = flags9[7].ToString() + flags9[6] + flags9[5] + flags9[4] + flags4[7] + flags4[6] + flags4[5] + flags4[4] + flags4[3] + flags4[2] + flags4[1] + flags4[0];
                    //int prgRomSize = int.Parse(prgHexString, System.Globalization.NumberStyles.HexNumber);
                    //PRGROMSize = prgRomSize;
                }

                if (CHRROMSizeMSB == 0xF)
                {

                }
                else
                {
                    int temp = CHRROMSizeMSB + CHRROMSizeLSB;
                    CHRROMSize = temp * 8192;
                    //string chrHexString = CHRROMSizeMSB.ToString() + CHRROMSizeLSB.ToString();
                    //int chrRomSize = int.Parse(chrHexString, System.Globalization.NumberStyles.HexNumber);
                    //CHRROMSize = chrRomSize;
                }
                
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
                    CPUPPUTimingMode = (CPUPPUTiming)1;
                }
                else
                {
                    CPUPPUTimingMode = (CPUPPUTiming)0;
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
                CPUPPUTimingMode = (CPUPPUTiming)3;
            }
            else if (flags12[0] && !flags12[1])
            {
                CPUPPUTimingMode = (CPUPPUTiming)1;
            }
            else if (!flags12[0] && flags12[1])
            {
                CPUPPUTimingMode = (CPUPPUTiming)2;
            }
            else
            {
                CPUPPUTimingMode = (CPUPPUTiming)0;
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

            if (isVsUnisystem)
            {
                /** - When the console type in Header byte 7 D0..D1 is 1 (Vs. System), the lower nibble of Header byte 13 specifies the
                 * Vs. PPU type, and the upper nibble the non-PPU-based protection type and whether the ROM is for the Vs. Unisystem or
                 * the Vs. Dual System.
                 */
                VsPPUType = (VsSystemPPUType) (flags & 0x0F);
                VsHardwareType = (VsSystemHardwareType) ((flags & 0xF0) >> 4);
            }

            if (isExtendedConsoleType)
            {
                /** - When the console type in Header byte 7 D0..D1 is 3 (Extended), the lower nibble of Header byte 13 specifies the
                 * type of console on which the ROM image is supposed to be run.
                 */
                ExtendedConsType = (ExtendedConsoleType) (flags & 0x0F);
            }
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

            DefaultExpansionDevice = (DefaultExpansionDevice)bytes[0];
        }

        public string GetCartridgeInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Cartridge Info for ROM: " + Path);
            stringBuilder.AppendLine("  Identification: " + Identification);
            stringBuilder.AppendLine("    Is iNES format: " + isiNESFormat);
            stringBuilder.AppendLine("    Is NES 2.0 format: " + isNES20Format);
            stringBuilder.AppendLine("  RAM/ROM: ");
            stringBuilder.AppendLine("    PRG ROM size: " + PRGROMSize.ToString());
            stringBuilder.AppendLine("      PRG ROM size LSB: 0x" + PRGROMSizeLSB.ToString("X"));
            stringBuilder.AppendLine("      PRG ROM size MSB: 0x" + PRGROMSizeMSB.ToString("X"));
            stringBuilder.AppendLine("    CHR ROM size: " + CHRROMSize.ToString());
            stringBuilder.AppendLine("      CHR ROM size LSB: 0x" + CHRROMSizeLSB.ToString("X"));
            stringBuilder.AppendLine("      CHR ROM size MSB: 0x" + CHRROMSizeMSB.ToString("X"));
            stringBuilder.AppendLine("    PRG RAM size: 0x" + PRGRAMSize.ToString("X"));
            stringBuilder.AppendLine("    PRG NVRAM size: 0x" + PRGNVRAMSize.ToString("X"));
            stringBuilder.AppendLine("    CHR RAM size: 0x" + CHRRAMSize.ToString("X"));
            stringBuilder.AppendLine("    CHR NVRAM size: 0x" + CHRNVRAMSize.ToString("X"));
            stringBuilder.AppendLine("  Mirroring: ");
            stringBuilder.AppendLine("    Has Vertical Mirroring: " + hasVerticalMirroring);
            stringBuilder.AppendLine("    Has Horizontal Mirroring: " + hasHorizontalMirroring);
            stringBuilder.AppendLine("  System Info:");
            stringBuilder.AppendLine("    Is Vs. Unisystem: " + isVsUnisystem);
            if (isVsUnisystem)
            {
                stringBuilder.AppendLine("      Vs. Unisystem PPU type: " + VsPPUType);
                stringBuilder.AppendLine("      Vs. Unisystem hardware type: " + VsHardwareType);
            }
            stringBuilder.AppendLine("    Is PlayChoice 10: " + isPlaychoice10);
            stringBuilder.AppendLine("    Is Nintendo Entertainment System/Family Computer: " + isNESFamilyComputer);
            stringBuilder.AppendLine("    Is Extended Console Type: " + isExtendedConsoleType);
            if (isExtendedConsoleType)
            {
                stringBuilder.AppendLine("    Extended Console Type: " + ExtendedConsType);
            }
            stringBuilder.AppendLine("  Mapper Info:");
            stringBuilder.AppendLine("    Mapper byte: 0x" + MapperByte.ToString("X"));
            stringBuilder.AppendLine("    Mapper: " + GetMapperName());
            stringBuilder.AppendLine("    Sub-mapper byte: 0x" + SubMapperByte.ToString("X"));
            stringBuilder.AppendLine("    Sub-mapper: " + GetSubMapperName());
            stringBuilder.AppendLine("    Plane: " + GetPlane());
            stringBuilder.AppendLine("      Flag 6 Mapper nibble: 0x" + Flag6MapperNibble.ToString("X"));
            stringBuilder.AppendLine("      Flag 7 Mapper nibble: 0x" + Flag7MapperNibble.ToString("X"));
            stringBuilder.AppendLine("      Flag 8 Mapper nibble: 0x" + Flag8MapperNibble.ToString("X"));
            stringBuilder.AppendLine("  Misc. Info:");
            stringBuilder.AppendLine("    Has Battery Backed PRG RAM: " + hasBatteryBackedPRGRAM);
            stringBuilder.AppendLine("    Has Trainer: " + hasTrainer);
            stringBuilder.AppendLine("    Has Four Screen VRAM: " + has4ScreenVRAM);
            stringBuilder.AppendLine("    CPU PPU timing mode: " + CPUPPUTimingMode);
            stringBuilder.AppendLine("    Number of miscellaneous ROMs: " + NumberOfMiscellaneousROMs);
            stringBuilder.AppendLine("    Default Expansion Device: " + DefaultExpansionDevice);

            return stringBuilder.ToString();
        }

        /** - @todo: Move below methods to separate class. */
        #region Mapper names
        private string GetMapperName()
        {
            string mapperString = "Unknown";
            int mapperInt = MapperByte;

            switch (mapperInt)
            {
                case 0:
                    mapperString = "No Mapper - NROM, or unknown mapper]";
                    break;
                case 1:
                    mapperString = "1 - Nintendo MMC1 Chipset / MMC1 - S(x)ROM";
                    break;
                case 2:
                    mapperString = "2 - ROM (PRG) Switch / U(x)ROM";
                    break;
                case 3:
                    mapperString = "3 - VROM (CHR) Switch / CNROM";
                    break;
                case 4:
                    mapperString = "4 - Nintendo MMC3 Chipset / MMC3 - T(x)ROM / MMC6 - H(x)ROM";
                    break;
                case 5:
                    mapperString = "5 - Nintendo MMC5 Chipset / MMC5 - E(x)ROM";
                    break;
                case 6:
                    mapperString = "6 - FFE F4XXX Games";
                    break;
                case 7:
                    mapperString = "7 - 32kb ROM (PRG) Switch - A(x)ROM";
                    break;
                case 8:
                    mapperString = "8 - FFE F3XXX Games";
                    break;
                case 9:
                    mapperString = "9 - Nintendo MMC2 Chipset / MMC2 - P(x)ROM";
                    break;
                case 10:
                    mapperString = "10 - Nintendo MMC4 Chipset / MMC4";
                    break;
                case 11:
                    mapperString = "11 - Color Dreams Chipset";
                    break;
                case 12:
                    mapperString = "12 - FFE F6XXX Games / DBZ5 (MMC3 Variant)";
                    break;
                case 13:
                    mapperString = "13 - CPROM";
                    break;
                case 15:
                    mapperString = "15 - 100-in-1 Cart Switch / Multicart";
                    break;
                case 16:
                    mapperString = "16 - Bandai Chipset";
                    break;
                case 17:
                    mapperString = "17 - FFE F8XXX Games";
                    break;
                case 18:
                    mapperString = "18 - Jaleco SS8806 Chipset";
                    break;
                case 19:
                    mapperString = "19 - Namcot 106 Chipset / N106";
                    break;
                case 20:
                    mapperString = "20 - Famicom Disk System";
                    break;
                case 21:
                    mapperString = "21 - Konami VRC4 2a Chipset";
                    break;
                case 22:
                    mapperString = "22 - Konami VRC4 1b Chipset";
                    break;
                case 23:
                    mapperString = "23 - Konami VRC4 1a Chipset";
                    break;
                case 24:
                    mapperString = "24 - Konami VRC6 Chipset";
                    break;
                case 25:
                    mapperString = "25 - Konami VRC4 Chipset";
                    break;
                case 26:
                    mapperString = "26 - VRC6 Variant (newer) Chipset";
                    break;
                case 27:
                    mapperString = "27 - World Hero";
                    break;
                case 32:
                    mapperString = "32 - Irem G-101 Chipset";
                    break;
                case 33:
                    mapperString = "33 - Taito TC0190/TC0350";
                    break;
                case 34:
                    mapperString = "34 - 32kb ROM (PRG) Switch B(x)ROM or NINA-001";
                    break;
                case 36:
                    mapperString = "36 - Strike Wolf";
                    break;
                case 37:
                    mapperString = "37 - MMC3 Multicart";
                    break;
                case 38:
                    mapperString = "38 - Brazil";
                    break;
                case 39:
                    mapperString = "39 - Subor";
                    break;
                case 40:
                    mapperString = "40 - Pirate";
                    break;
                case 41:
                    mapperString = "41 - Caltron Multicart";
                    break;
                case 42:
                    mapperString = "42 - Pirate";
                    break;
                case 43:
                    mapperString = "43 - Pirate Multicart";
                    break;
                case 44:
                    mapperString = "44 - MMC3 Based / Multicart";
                    break;
                case 45:
                    mapperString = "45 - Super (X)-in-1 MMC3 Based / Multicart";
                    break;
                case 46:
                    mapperString = "46 - Multicart";
                    break;
                case 47:
                    mapperString = "47 - Multicart - MMC3 Based";
                    break;
                case 48:
                    mapperString = "48 - MMC3 Variant";
                    break;
                case 49:
                    mapperString = "49 - Multicart - MMC3 Based";
                    break;
                case 50:
                    mapperString = "50 - Pirate";
                    break;
                case 51:
                    mapperString = "51 - Pirate Multicart";
                    break;
                case 52:
                    mapperString = "52 - Multicart - MMC3 Based";
                    break;
                case 53:
                    mapperString = "53 - Pirate Multicart";
                    break;
                case 54:
                    mapperString = "54 - Multicart";
                    break;
                case 55:
                    mapperString = "55 - Pirate";
                    break;
                case 56:
                    mapperString = "56 - Pirate";
                    break;
                case 57:
                    mapperString = "57 - Multicart";
                    break;
                case 58:
                    mapperString = "58 - Multicart";
                    break;
                case 60:
                    mapperString = "60 - Reset Based 4-in-1 / NROM / Multicart";
                    break;
                case 61:
                    mapperString = "61 - Multicart";
                    break;
                case 62:
                    mapperString = "62 - Multicart";
                    break;
                case 64:
                    mapperString = "64 - Tengen Rambo-1";
                    break;
                case 65:
                    mapperString = "65 - Irem H3001 Chipset / Misc (J)";
                    break;
                case 66:
                    mapperString = "66 - 74161/32 Chipset - G(x)ROM";
                    break;
                case 67:
                    mapperString = "67 - Sunsoft-3";
                    break;
                case 68:
                    mapperString = "68 - Sunsoft-4";
                    break;
                case 69:
                    mapperString = "69 - Sunsoft Mapper 4, FME-7, Sunsoft 5B";
                    break;
                case 70:
                    mapperString = "70 - 74161/32 Chipset";
                    break;
                case 71:
                    mapperString = "71 - Camerica";
                    break;
                case 72:
                    mapperString = "72 - Jaleco Early Mapper / Misc (J)";
                    break;
                case 73:
                    mapperString = "73 - VRC3 - Konami VRC";
                    break;
                case 74:
                    mapperString = "74 - Pirate MMC3 variant - Taiwan MMC3 / Pirate (CN)";
                    break;
                case 75:
                    mapperString = "75 - Jaleco Mapper SS8805 / VRC1";
                    break;
                case 76:
                    mapperString = "76 - Namco 109";
                    break;
                case 77:
                    mapperString = "77 - Irem Early Mapper 0";
                    break;
                case 78:
                    mapperString = "78 - 74161/32";
                    break;
                case 79:
                    mapperString = "79 - American Video Ent. / NINA-06";
                    break;
                case 80:
                    mapperString = "80 - X-005 Chipset";
                    break;
                case 81:
                    mapperString = "81 - C075 Chipset";
                    break;
                case 82:
                    mapperString = "82 - X1-17 Chipset";
                    break;
                case 83:
                    mapperString = "83 - Cony Mapper / Pirate";
                    break;
                case 84:
                    mapperString = "84 - PasoFami Mapper!";
                    break;
                case 85:
                    mapperString = "85 - Konami VRC7";
                    break;
                case 86:
                    mapperString = "86 - Misc (J)";
                    break;
                case 87:
                    mapperString = "87 - Misc (J)";
                    break;
                case 88:
                    mapperString = "88 - Misc (J)";
                    break;
                case 89:
                    mapperString = "89 - Sunsoft-2";
                    break;
                case 90:
                    mapperString = "90 - Pirate";
                    break;
                case 91:
                    mapperString = "91 - Pirate";
                    break;
                case 92:
                    mapperString = "92 - Misc (J)";
                    break;
                case 93:
                    mapperString = "93 - Sunsoft-2";
                    break;
                case 94:
                    mapperString = "94 - Misc (J)";
                    break;
                case 95:
                    mapperString = "95 - MMC3 variant";
                    break;
                case 96:
                    mapperString = "96 - Misc (J)";
                    break;
                case 97:
                    mapperString = "97 - Misc (J)";
                    break;
                case 99:
                    mapperString = "99 - VS (Arcade)";
                    break;
                case 100:
                    mapperString = "100 - Nestice - Buggy Mode";
                    break;
                case 101:
                    mapperString = "101 - Junk";
                    break;
                case 103:
                    mapperString = "103 - FDS Conversion";
                    break;
                case 104:
                    mapperString = "104 - Camerica";
                    break;
                case 105:
                    mapperString = "105 - NES-EVENT";
                    break;
                case 106:
                    mapperString = "106 - Pirate";
                    break;
                case 107:
                    mapperString = "107 - Unlicensed";
                    break;
                case 108:
                    mapperString = "108 - FDS Conversion";
                    break;
                case 111:
                    mapperString = "111 - Misc (CN)";
                    break;
                case 112:
                    mapperString = "112 - Misc (CN)";
                    break;
                case 113:
                    mapperString = "113 - Mislabeled Nina_006";
                    break;
                case 115:
                    mapperString = "115 - MMC3 variant";
                    break;
                case 116:
                    mapperString = "116 - MMC3 variant";
                    break;
                case 117:
                    mapperString = "117 - Chinese";
                    break;
                case 118:
                    mapperString = "118 - MMC3 variant / TLSROM";
                    break;
                case 119:
                    mapperString = "119 - MMC3 variant / TQROM";
                    break;
                case 120:
                    mapperString = "120 - FDS Conversion";
                    break;
                case 121:
                    mapperString = "121 - Pirate";
                    break;
                case 123:
                    mapperString = "123 - Pirate";
                    break;
                case 125:
                    mapperString = "124 - Pirate";
                    break;
                case 132:
                    mapperString = "132 - Misc";
                    break;
                case 133:
                    mapperString = "133 - Sachen";
                    break;
                case 134:
                    mapperString = "134 - Misc (CN)";
                    break;
                case 136:
                case 137:
                case 138:
                case 139:
                    mapperString = mapperInt + " - Sachen";
                    break;
                case 140:
                    mapperString = "140 - Misc (J)";
                    break;
                case 141:
                case 142:
                case 143:
                    mapperString = mapperInt + " - Sachen";
                    break;
                case 144:
                    mapperString = "144 - Variant of Mapper 11 - Color Dreams Chipset";
                    break;
                case 145:
                case 146:
                case 147:
                case 148:
                case 149:
                case 150:
                    mapperString = mapperInt + " - Sachen";
                    break;
                case 151:
                    mapperString = "151 - Vs. Unisystem";
                    break;
                case 152:
                    mapperString = "152 - Misc (J)";
                    break;
                case 153:
                    mapperString = "153 - Bandai";
                    break;
                case 154:
                    mapperString = "154 - Misc (J)";
                    break;
                case 155:
                    mapperString = "155 - MMC1 Clone";
                    break;
                case 156:
                    mapperString = "156 - Korean";
                    break;
                case 157:
                    mapperString = "157 - Bandai";
                    break;
                case 158:
                    mapperString = "158 - Tengen";
                    break;
                case 159:
                    mapperString = "159 - Clone of Mapper 16 - Bandai Chipset";
                    break;
                case 160:
                    mapperString = "160 - Sachen";
                    break;
                case 162:
                case 163:
                    mapperString = mapperInt + " - Chinese";
                    break;
                case 164:
                    mapperString = "164 - Pirate";
                    break;
                case 165:
                    mapperString = "165 - MMC3 Variant";
                    break;
                case 166:
                case 167:
                    mapperString = mapperInt + " - Subor";
                    break;
                case 168:
                    mapperString = "168 - Racemate Challenger II";
                    break;
                case 169:
                    mapperString = "169 - Pirate Multicart";
                    break;
                case 170:
                    mapperString = "170 - Shiko Game Syu";
                    break;
                case 171:
                    mapperString = "171 - Tui Do Woo Ma Jeung";
                    break;
                case 174:
                    mapperString = "174 - Multicart";
                    break;
                case 175:
                    mapperString = "175 - Pirate Multicart";
                    break;
                case 176:
                    mapperString = "176 - Chinese WXN";
                    break;
                case 177:
                    mapperString = "177 - Chinese";
                    break;
                case 178:
                    mapperString = "178 - Chinese WXN";
                    break;
                case 180:
                    mapperString = "180 - Misc (J)";
                    break;
                case 182:
                    mapperString = "182 - MMC3 Variant / Scrambled";
                    break;
                case 183:
                    mapperString = "183 - Shui Guan Pipe";
                    break;
                case 184:
                    mapperString = "184 - Sunsoft-1";
                    break;
                case 185:
                    mapperString = "185 - Misc (J)";
                    break;
                case 186:
                    mapperString = "186 - Study Box (J)";
                    break;
                case 187:
                    mapperString = "187 - Pirate";
                    break;
                case 188:
                    mapperString = "188 - Karaoke (J)";
                    break;
                case 189:
                    mapperString = "189 - MMC3 variant";
                    break;
                case 191:
                    mapperString = "191 - Pirate / MMC3 variant";
                    break;
                case 192:
                    mapperString = "192 - Chinese WXN / Pirate / MMC3 variant";
                    break;
                case 193:
                    mapperString = "193 - Unlicensed";
                    break;
                case 194:
                    mapperString = "194 - Chinese WXN / Pirate / MMC3 variant";
                    break;
                case 195:
                    mapperString = "195 - Chinese WXN";
                    break;
                case 196:
                    mapperString = "196 - Pirate";
                    break;
                case 197:
                    mapperString = "197 - Street Fighter III";
                    break;
                case 198:
                case 199:
                    mapperString = mapperInt + " - Pirate";
                    break;
                case 200:
                    mapperString = "200 - Multicart";
                    break;
                case 201:
                    mapperString = "201 - Multicart";
                    break;
                case 202:
                    mapperString = "202 - Pirate";
                    break;
                case 203:
                    mapperString = "203 - Multicart";
                    break;
                case 204:
                    mapperString = "204 - Pirate";
                    break;
                case 205:
                    mapperString = "205 - Multicart";
                    break;
                case 206:
                    mapperString = "206 - Namcot109";
                    break;
                case 207:
                    mapperString = "207 - Misc (J)";
                    break;
                case 209:
                    mapperString = "209 - Garbage";
                    break;
                case 210:
                    mapperString = "210 - Namcot";
                    break;
                case 211:
                    mapperString = "211 - Pirate";
                    break;
                case 212:
                case 213:
                    mapperString = mapperInt + " - Ten Million Games in One";
                    break;
                case 214:
                    mapperString = "214 - Multicart";
                    break;
                case 215:
                    mapperString = "215 - Pirate";
                    break;
                case 216:
                case 217:
                    mapperString = mapperInt + " - [Multicart]";
                    break;
                case 218:
                    mapperString = "218 - Single Chip Cartridge";
                    break;
                case 219:
                    mapperString = "219 - Garbage";
                    break;
                case 220:
                    mapperString = "220 - Summer Carnival 92";
                    break;
                case 221:
                    mapperString = "221 - Pirate Multicart";
                    break;
                case 222:
                case 223:
                case 224:
                    mapperString = mapperInt + " - [Pirate]";
                    break;
                case 225:
                    mapperString = "225 - Multicart";
                    break;
                case 226:
                    mapperString = "226 - Multicart";
                    break;
                case 227:
                    mapperString = "227 - Multicart";
                    break;
                case 228:
                    mapperString = "228 - Action 52";
                    break;
                case 229:
                    mapperString = "229 - Pirate Multicart";
                    break;
                case 230:
                    mapperString = "230 - Multicart";
                    break;
                case 231:
                    mapperString = "231 - Multicart";
                    break;
                case 232:
                    mapperString = "232 - Camerica";
                    break;
                case 233:
                    mapperString = "233 - Multicart";
                    break;
                case 234:
                    mapperString = "234 - Misc";
                    break;
                case 235:
                    mapperString = "235 - 260 in 1";
                    break;
                case 236:
                    mapperString = "236 - Multicart";
                    break;
                case 238:
                    mapperString = "238 - Garbage";
                    break;
                case 240:
                    mapperString = "240 - Misc. (CN)";
                    break;
                case 241:
                    mapperString = "241 - Misc. (CN)";
                    break;
                case 242:
                    mapperString = "242 - Misc. (CN)";
                    break;
                case 243:
                    mapperString = "243 - Misc";
                    break;
                case 244:
                    mapperString = "244 - Decathlon";
                    break;
                case 245:
                    mapperString = "245 - Chinese WXN";
                    break;
                case 246:
                    mapperString = "246 - Misc. (CN)";
                    break;
                case 248:
                    mapperString = "248 - Misc.";
                    break;
                case 249:
                    mapperString = "249 - Misc. (CN)";
                    break;
                case 250:
                    mapperString = "250 - Time Diver Avenger";
                    break;
                case 251:
                    mapperString = "251 - Pirate Multicart";
                    break;
                case 252:
                    mapperString = "252 - Chinese WXN";
                    break;
                case 253:
                    mapperString = "253 - Misc. (CN)";
                    break;
                case 255:
                    mapperString = "255 - Multicart";
                    break;
                default:
                    mapperString = mapperInt + " - Unknown";
                    break;
            }

            return mapperString;
        }

        #endregion

        #region Sub-mapper names

        private string GetSubMapperName()
        {
            string subMapperString = "Unknown";
            int mapperInt = MapperByte;
            int subMapperInt = SubMapperByte;

            if (subMapperInt == 0 && mapperInt == 1)
            {
                subMapperString = "Default Behavior";
            }
            else if (subMapperInt == 1 && mapperInt == 1)
            {
                subMapperString = "Deprecated: SUROM";
            }
            else if (subMapperInt == 2 && mapperInt == 1)
            {
                subMapperString = "Deprecated: SOROM";
            }
            else if (subMapperInt == 3 && mapperInt == 1)
            {
                subMapperString = "Deprecated: Already implemented as iNES Mapper 155.";
            }
            else if (subMapperInt == 4 && mapperInt == 1)
            {
                subMapperString = "Deprecated: SXROM";
            }
            else if (subMapperInt == 5 && mapperInt == 1)
            {
                subMapperString = "Fixed PRG: SEROM, SHROM, SH1ROM use a fixed 32k PRG ROM with no banking support.";
            }
            else if (subMapperInt == 0 && mapperInt == 2)
            {
                subMapperString = "UNROM/UN1ROM/UOROM - Default Behavior";
            }
            else if (subMapperInt == 1 && mapperInt == 2)
            {
                subMapperString = "UNROM/UN1ROM/UOROM - Bus conflicts do not occur";
            }
            else if (subMapperInt == 2 && mapperInt == 2)
            {
                subMapperString = "UNROM/UN1ROM/UOROM - Bus conflicts occur, producing the bitwise AND of the written value and the value in ROM";
            }
            else if (subMapperInt == 0 && mapperInt == 3)
            {
                subMapperString = "CNROM - Default Behavior";
            }
            else if (subMapperInt == 1 && mapperInt == 3)
            {
                subMapperString = "CNROM - Bus conflicts do not occur";
            }
            else if (subMapperInt == 2 && mapperInt == 3)
            {
                subMapperString = "CNROM - Bus conflicts occur, producing the bitwise AND of the written value and the value in ROM";
            }
            else if (subMapperInt == 0 && mapperInt == 4)
            {
                subMapperString = "MMC3C - Default Behavior";
            }
            else if (subMapperInt == 1 && mapperInt == 4)
            {
                subMapperString = "MMC6 - Alternative PRG-RAM enable and write protection scheme designed for its internal 1k PRG RAM. ";
            }
            else if (subMapperInt == 2 && mapperInt == 4)
            {
                subMapperString = "Deprecated: MMC3C - with hard wired mirroring. No games are known to require this. ";
            }
            else if (subMapperInt == 3 && mapperInt == 4)
            {
                subMapperString = "MC-ACC - Found in 13 second-source PCBs manufactured by Acclaim. ";
            }
            else if (subMapperInt == 0 && mapperInt == 7)
            {
                subMapperString = "ANROM/AN1ROM/AOROM/AMROM - Default Behavior";
            }
            else if (subMapperInt == 1 && mapperInt == 7)
            {
                subMapperString = "ANROM/AN1ROM/AOROM/AMROM - Bus conflicts do not occur";
            }
            else if (subMapperInt == 2 && mapperInt == 7)
            {
                subMapperString = "ANROM/AN1ROM/AOROM/AMROM - Bus conflicts occur, producing the bitwise AND of the written value and the value in ROM";
            }
            else if (subMapperInt == 0 && mapperInt == 32)
            {
                subMapperString = "Irem G101 - Normal (H/V mapper-controlled mirroring) ";
            }
            else if (subMapperInt == 1 && mapperInt == 32)
            {
                subMapperString = "Major League - CIRAM A10 is tied high (fixed one-screen mirroring) and PRG banking style is fixed as 8+8+16F";
            }
            else if (subMapperInt == 0 && mapperInt == 34)
            {
                subMapperString = "BNROM/NINA-001 - Default Behavior";
            }
            else if (subMapperInt == 1 && mapperInt == 34)
            {
                subMapperString = "NINA-001";
            }
            else if (subMapperInt == 2 && mapperInt == 34)
            {
                subMapperString = "BNROM - Some unlicensed boards by Union Bond were a variation of BNROM that included PRG RAM. These may also use this submapper if PRG RAM is specified in the NES 2.0 header.";
            }
            else if (subMapperInt == 0 && mapperInt == 68)
            {
                subMapperString = "Sunsoft 4 - Normal (max 256KiB PRG)";
            }
            else if (subMapperInt == 1 && mapperInt == 68)
            {
                subMapperString = "Sunsoft 4 - Sunsoft Dual Cartridge System a.k.a. NTB-ROM (max 128KiB PRG, licensing IC present, external option ROM of up to 128KiB should be selectable by a second menu)";
            }
            else if (subMapperInt == 0 && mapperInt == 71)
            {
                subMapperString = "Codemasters - Hardwired horizontal or vertical mirroring. ";
            }
            else if (subMapperInt == 1 && mapperInt == 71)
            {
                subMapperString = "Codemasters - Fire Hawk - Mapper controlled single-screen mirroring.";
            }
            else if (subMapperInt == 0 && mapperInt == 78)
            {
                subMapperString = "Unspecified. ";
            }
            else if (subMapperInt == 1 && mapperInt == 78)
            {
                subMapperString = "Cosmo Carrier - Single-screen mirroring (nibble-swapped mapper 152). ";
            }
            else if (subMapperInt == 2 && mapperInt == 78)
            {
                subMapperString = "Deprecated - This described a variation with fixed vertical mirroring, and WRAM. There is no known use case. ";
            }
            else if (subMapperInt == 3 && mapperInt == 78)
            {
                subMapperString = "Holy Diver - Mapper-controlled H/V mirroring.";
            }
            else if (subMapperInt == 0 && mapperInt == 210)
            {
                subMapperString = "No advisory statement is made (use runtime heuristics suggested at mapper 210).";
            }
            else if (subMapperInt == 1 && mapperInt == 210)
            {
                subMapperString = "N175 - Namco 175. Hardwired mirroring, no IRQ. ";
            }
            else if (subMapperInt == 2 && mapperInt == 210)
            {
                subMapperString = "N340 - Namco 340. 1/H/V mirroring, no IRQ, no internal or external RAM. ";
            }
            else if (subMapperInt == 0 && mapperInt == 232)
            {
                subMapperString = "Normal";
            }
            else if (subMapperInt == 1 && mapperInt == 232)
            {
                subMapperString = "Aladdin Deck Enhancer - Aladdin Deck Enhancer variation. Swap the bits of the outer bank number.";
            }
            else
            {
                subMapperString = "Nothing defined for this submapper.";
            }

            return subMapperString;
        }

        #endregion

        private string GetPlane()
        {
            string planeString;

            if (Flag8MapperNibble == 0x0)
            {
                planeString = "0 (0-255): Basic Multilingual.";
            }
            else if (Flag8MapperNibble == 0x1)
            {
                planeString = "1 (256-511): Mostly for new homebrew mappers.";
            }
            else if (Flag8MapperNibble == 0x2)
            {
                planeString = "2 (512-767): For new dumps of East Asian games.";
            }
            else if (Flag8MapperNibble == 0xF)
            {
                planeString = "15: Private use area (not for publicly distributed dumps).";
            }
            else
            {
                // Not defined yet.
                planeString = Flag8MapperNibble + ": Undefined.";
            }

            return planeString;
        }
    }
}