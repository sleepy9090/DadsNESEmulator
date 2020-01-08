/**
 *  @file           CPUPPUTiming.cs
 *  @brief          Defines CPU PPU timing.
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
     * Value  Meaning    Regions
     * 0      RP2C02     North America, Japan, South Korea, Taiwan
     * 1      RP2C07     Western Europe, Australia
     * 2      ("multiple-region") is used either if a game was released with identical ROM content in both NTSC and PAL countries, such as Nintendo's early games, or if the game detects the console's timing and adjusts itself. 
     * 3      UMC 6527P  Eastern Europe, Russia, Mainland China, India, Africa
     */
    public enum CPUPPUTiming
    {
        RP2C02 = 0x0,
        RP2C07 = 0x1,
        MultiRegion = 0x2,
        UMC6527P = 0x3
    }
}