/**
 *  @file           PPU.cs
 *  @brief          Defines the NES Picture Processing Unit (PPU) that is used to generate a composite video signal with 240 lines of pixels, designed to be received by a television.
 *  
 *  @copyright      2019
 *  @date           11/05/2019
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
using System;
using System.Collections;
using System.Linq.Expressions;

namespace DadsNESEmulator.NESHardware
{
    public class PPU
    {
        /**
         *  - https://wiki.nesdev.com/w/index.php/PPU_registers
         *  - https://en.wikibooks.org/wiki/NES_Programming/Memory_Map
         */


        /*
         * +---------------+
         * |PPU base timing|
         * +---------------+
         * Other than the 3-stage Johnson counter, the 21.48 MHz signal is not used directly by any other PPU hardware.
         * Instead, the signal is divided by 4 to get 5.37 MHz, and is used as the smallest unit of timing in the PPU.
         * All following references to PPU clock cycle (abbr. "cc") timing in this document will be in respect to this
         * timing base, unless otherwise indicated.
         *
         * - Pixels are rendered at the same rate as the base PPU clock. In other words, 1 clock cycle= 1 pixel.
         * - 341 PPU cc's make up the time of a typical scanline (or 341/3 CPU cc's).
         * - One frame consists of 262 scanlines. This equals 341*262 PPU cc's per frame (divide by 3 for # of CPU cc's).
         */
        public const int _PPU_CLOCKS_PER_SCANLINE = 341;
        public const int _CPU_CLOCKS_PER_SCANLINE = _PPU_CLOCKS_PER_SCANLINE / 3; //113 (actually 341*4/12= 113 and 2/3)

        /* If rendering is off, each frame will be 341*262/3 = 29780 2/3 CPU clocks long. */
        public const int _PPU_CLOCKS_PER_FRAME = 89342; //341 × 261 + 340.5 = 89341.5
        public const int _CPU_CLOCKS_PER_FRAME = _PPU_CLOCKS_PER_FRAME / 3; //29780,  	89341.5 ÷ 3 = 29780.5 

        public const int _PICTURE_HEIGHT_SCANLINES = 240;
        public const int _VISIBLE_PICTURE_HEIGHT_SCANLINES = 224;

        #region Constants (registers)

        /**
         * Common Name  Address Bits        Notes
         * PPUCTRL      $2000   VPHB SINN   NMI enable (V), PPU master/slave (P), sprite height (H), background tile select (B), sprite tile select (S), increment mode (I), nametable select (NN)
         * PPUMASK      $2001   BGRs bMmG   color emphasis (BGR), sprite enable (s), background enable (b), sprite left column enable (M), background left column enable (m), greyscale (G)
         * PPUSTATUS    $2002   VSO- ----   vblank (V), sprite 0 hit (S), sprite overflow (O); read resets write pair for $2005/$2006
         * OAMADDR      $2003   aaaa aaaa   OAM read/write address
         * OAMDATA      $2004   dddd dddd   OAM data read/write
         * PPUSCROLL	$2005   xxxx xxxx   fine scroll position (two writes: X scroll, Y scroll)
         * PPUADDR      $2006   aaaa aaaa   PPU read/write address (two writes: most significant byte, least significant byte)
         * PPUDATA      $2007   dddd dddd   PPU data read/write
         * OAMDMA       $4014   aaaa aaaa   OAM DMA high address
         */

        /** - @brief Controller ($2000) > write */
        public const ushort _PPU_CTRL = 0x2000;

        /** - @brief Mask ($2001) > write */
        public const ushort _PPU_MASK = 0x2001;

        /** - @brief Status ($2002) < read */
        public const ushort _PPU_STATUS = 0x2002;

        /** - @brief OAM address ($2003) > write */
        public const ushort _OAM_ADDR = 0x2003;

        /** - @brief OAM data ($2004) <> read/write */
        public const ushort _OAM_DATA = 0x2004;

        /** - @brief Scroll ($2005) >> write x2 */
        public const ushort _PPU_SCROLL = 0x2005;

        /** - @brief Address ($2006) >> write x2 */
        public const ushort _PPU_ADDR = 0x2006;

        /** - @brief Data ($2007) <> read/write */
        public const ushort _PPU_DATA = 0x2007;

        /** - @brief OAM DMA ($4014) > write */
        public const ushort _OAM_DMA = 0x4014;

        #endregion

        #region Fields

        //private byte _ppuScrollRegister;

        //private byte _ppuAddressRegister;

        #endregion

        #region Properties

        public PPUMemoryMap PPUMemMap
        {
            get;
            protected set;
        }

        /*
         * 7  bit  0
         * ---- ----
         * VPHB SINN
         * |||| ||||
         * |||| ||++- Base nametable address
         * |||| ||    (0 = $2000; 1 = $2400; 2 = $2800; 3 = $2C00)
         * |||| |+--- VRAM address increment per CPU read/write of PPUDATA
         * |||| |     (0: add 1, going across; 1: add 32, going down)
         * |||| +---- Sprite pattern table address for 8x8 sprites
         * ||||       (0: $0000; 1: $1000; ignored in 8x16 mode)
         * |||+------ Background pattern table address (0: $0000; 1: $1000)
         * ||+------- Sprite size (0: 8x8 pixels; 1: 8x16 pixels)
         * |+-------- PPU master/slave select
         * |          (0: read backdrop from EXT pins; 1: output color on EXT pins)
         * +--------- Generate an NMI at the start of the vertical blanking interval (0: off; 1: on)
         */
        public BitArray PPUControlRegister
        {
            get;
            protected set;
        }

        /*
         * 7  bit  0
         * ---- ----
         * BGRs bMmG
         * |||| ||||
         * |||| |||+- Greyscale (0: normal color, 1: produce a greyscale display)
         * |||| ||+-- 1: Show background in leftmost 8 pixels of screen, 0: Hide
         * |||| |+--- 1: Show sprites in leftmost 8 pixels of screen, 0: Hide
         * |||| +---- 1: Show background
         * |||+------ 1: Show sprites
         * ||+------- Emphasize red
         * |+-------- Emphasize green
         * +--------- Emphasize blue
         */
        public BitArray PPUMaskRegister
        {
            get;
            protected set;
        }

        /*
         * 7  bit  0
         * ---- ----
         * VSO. ....
         * |||| ||||
         * |||+-++++- Least significant bits previously written into a PPU register
         * |||        (due to register not being updated for this address)
         * ||+------- Sprite overflow. The intent was for this flag to be set
         * ||         whenever more than eight sprites appear on a scanline, but a
         * ||         hardware bug causes the actual behavior to be more complicated
         * ||         and generate false positives as well as false negatives; see
         * ||         PPU sprite evaluation. This flag is set during sprite
         * ||         evaluation and cleared at dot 1 (the second dot) of the
         * ||         pre-render line.
         * |+-------- Sprite 0 Hit.  Set when a nonzero pixel of sprite 0 overlaps
         * |          a nonzero background pixel; cleared at dot 1 of the pre-render
         * |          line.  Used for raster timing.
         * +--------- Vertical blank has started (0: not in vblank; 1: in vblank).
         *            Set at dot 1 of line 241 (the line *after* the post-render
         *            line); cleared after reading $2002 and at dot 1 of the
         *            pre-render line.
         */
        public BitArray PPUStatusRegister
        {
            get;
            protected set;
        }

        public byte OAMAddressRegister
        {
            get;
            protected set;
        }

        public byte OAMDataRegister
        {
            get;
            protected set;
        }

        public byte PPUScrollRegister
        {
            get;
            protected set;
        }

        public byte PPUAddressRegister
        {
            get;
            protected set;
        }

        public byte PPUDataRegister
        {
            get;
            protected set;
        }

        public byte OAMDMARegister
        {
            get;
            protected set;
        }

        #endregion

        #region Class methods

        /**
         * @brief   This method is the class constructor.
         *
         * @param   ppuMemoryMap = The PPU Memory map
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        public PPU(PPUMemoryMap ppuMemoryMap)
        {
            PPUMemMap = ppuMemoryMap;
        }

        public byte ReadRegister(ushort address)
        {
            byte returnByte;

            switch (address)
            {
                case _PPU_CTRL:
                    // Write only
                    returnByte = ConvertBitArrayToByte(PPUControlRegister);
                    break;
                case _PPU_MASK:
                    // Write only
                    returnByte = ConvertBitArrayToByte(PPUMaskRegister);
                    break;
                case _PPU_STATUS:
                    // Read only
                    returnByte = ConvertBitArrayToByte(PPUStatusRegister);
                    break;
                case _OAM_ADDR:
                    // Write only
                    returnByte = OAMAddressRegister;
                    break;
                case _OAM_DATA:
                    // Read/Write
                    returnByte = OAMDataRegister;
                    break;
                case _PPU_SCROLL:
                    // Write only (write twice)
                    returnByte = PPUScrollRegister;
                    break;
                case _PPU_ADDR:
                    // Write only (write twice)
                    returnByte = PPUAddressRegister;
                    break;
                case _PPU_DATA:
                    // Read/Write
                    returnByte = PPUDataRegister;
                    break;
                case _OAM_DMA:
                    // Write only
                    returnByte = OAMDMARegister;
                    break;
                default:
                    Console.WriteLine(string.Format(@"Invalid read from PPU address: {0:X4}", address));
                    throw new Exception(string.Format(@"Invalid read from PPU address: {0:X4}", address));
            }

            return returnByte;
        }

        public void WriteRegister(ushort address, byte value)
        {
            switch (address)
            {
                case _PPU_CTRL:
                    // Write only
                    PPUControlRegister[7] = (value & 0x80) != 0;
                    PPUControlRegister[6] = (value & 0x40) != 0;
                    PPUControlRegister[5] = (value & 0x20) != 0;
                    PPUControlRegister[4] = (value & 0x10) != 0;
                    PPUControlRegister[3] = (value & 0x8) != 0;
                    PPUControlRegister[2] = (value & 0x4) != 0;
                    PPUControlRegister[1] = (value & 0x2) != 0;
                    PPUControlRegister[0] = (value & 0x1) != 0;
                    break;
                case _PPU_MASK:
                    // Write only
                    PPUMaskRegister[7] = (value & 0x80) != 0;
                    PPUMaskRegister[6] = (value & 0x40) != 0;
                    PPUMaskRegister[5] = (value & 0x20) != 0;
                    PPUMaskRegister[4] = (value & 0x10) != 0;
                    PPUMaskRegister[3] = (value & 0x8) != 0;
                    PPUMaskRegister[2] = (value & 0x4) != 0;
                    PPUMaskRegister[1] = (value & 0x2) != 0;
                    PPUMaskRegister[0] = (value & 0x1) != 0;
                    break;
                case _PPU_STATUS:
                    // Read only
                    PPUStatusRegister[7] = (value & 0x80) != 0;
                    PPUStatusRegister[6] = (value & 0x40) != 0;
                    PPUStatusRegister[5] = (value & 0x20) != 0;
                    PPUStatusRegister[4] = (value & 0x10) != 0;
                    PPUStatusRegister[3] = (value & 0x8) != 0;
                    PPUStatusRegister[2] = (value & 0x4) != 0;
                    PPUStatusRegister[1] = (value & 0x2) != 0;
                    PPUStatusRegister[0] = (value & 0x1) != 0;
                    break;
                case _OAM_ADDR:
                    // Write only
                    OAMAddressRegister = value;
                    break;
                case _OAM_DATA:
                    // Read/Write
                    OAMDataRegister = value;
                    break;
                case _PPU_SCROLL:
                    // Write only (write twice)
                    PPUScrollRegister = value;
                    break;
                case _PPU_ADDR:
                    // Write only (write twice)
                    PPUAddressRegister = value;
                    break;
                case _PPU_DATA:
                    // Read/Write
                    PPUDataRegister = value;
                    break;
                case _OAM_DMA:
                    // Write only
                    OAMDMARegister = value;
                    break;
                default:
                    Console.WriteLine(string.Format(@"Invalid write to PPU address: {0:X4} byte: {1:X2}", address, value));
                    throw new Exception(string.Format(@"Invalid write to PPU address: {0:X4} byte: {1:X2}", address, value));
            }
        }

        /**
         * @brief   This method converts a bit array into a byte.
         *
         * @param   bits = The bit array to convert to byte.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private byte ConvertBitArrayToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                /* Should not happen. */
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        #endregion

    }
}