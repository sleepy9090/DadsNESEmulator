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
namespace DadsNESEmulator.NESHardware
{
    public class PPU
    {
        /** - https://wiki.nesdev.com/w/index.php/PPU_registers */

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
    }
}