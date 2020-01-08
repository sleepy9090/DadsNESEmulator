﻿/**
 *  @file           VsSystemPPUType.cs
 *  @brief          Defines the Vs. system PPU type.
 *  
 *  @copyright      2020
 *  @date           1/08/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
namespace DadsNESEmulator.Types
{
    /** - https://wiki.nesdev.com/w/index.php/NES_2.0 */
    /*
     * Vs. PPU types (Header byte 13 D0..D3):
     * $0: RP2C03B
     * $1: RP2C03G
     * $2: RP2C04-0001
     * $3: RP2C04-0002
     * $4: RP2C04-0003
     * $5: RP2C04-0004
     * $6: RC2C03B
     * $7: RC2C03C
     * $8: RC2C05-01 ($2002 AND $?? =$1B)
     * $9: RC2C05-02 ($2002 AND $3F =$3D)
     * $A: RC2C05-03 ($2002 AND $1F =$1C)
     * $B: RC2C05-04 ($2002 AND $1F =$1B)
     * $C: RC2C05-05 ($2002 AND $1F =unknown)
     * $D-F: reserved
     */
    public enum VsSystemPPUType : byte
    {
        RP2C03B = 0x0,
        RP2C03G = 0x1,
        RP2C040001 = 0x2,
        RP2C040002 = 0x3,
        RP2C040003 = 0x4,
        RP2C040004 = 0x5,
        RC2C03B = 0x6,
        RC2C03C = 0x7,
        RC2C0501 = 0x8,
        RC2C0502 = 0x9,
        RC2C0503 = 0xA,
        RC2C0504 = 0xB,
        RC2C0505 = 0xC,
        Reserved0 = 0xD,
        Reserved1 = 0xE,
        Reserved2 = 0xF
    }
}