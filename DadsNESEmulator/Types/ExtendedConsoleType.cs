/**
 *  @file           ExtendedConsoleType.cs
 *  @brief          Defines extended console types.
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
    public enum ExtendedConsoleType
    {
        RegularNESFamicomDendy = 0x0,                         /**< [Regular NES/Famicom/Dendy] */
        NintendoVsSystem = 0x1,                               /**< [Nintendo Vs. System] */
        Playchoice10 = 0x2,                                   /**< [Playchoice 10] */
        RegularFamicloneWithCPUThatSupportsDecimalMode = 0x3, /**< Regular Famiclone, but with CPU that supports Decimal Mode (e.g. Bit Corporation Creator) */
        VRTechnologyVT01WithMonochromePalette = 0x4,          /**< V.R. Technology VT01 with monochrome palette */
        VRTechnologyVT01WithRedCyanSTNPalette = 0x5,          /**< V.R. Technology VT01 with red/cyan STN palette */
        VRTechnologyVT02 = 0x6,                               /**< V.R. Technology VT02 */
        VRTechnologyVT03 = 0x7,                               /**< V.R. Technology VT03 */
        VRTechnologyVT09 = 0x8,                               /**< V.R. Technology VT09 */
        VRTechnologyVT32 = 0x9,                               /**< V.R. Technology VT32 */
        VRTechnologyVT369 = 0xA,                              /**< V.R. Technology VT369 */
        Reserved0 = 0xB,                                      /**< Reserved */
        Reserved1 = 0xC,                                      /**< Reserved */
        Reserved2 = 0xD,                                      /**< Reserved */
        Reserved3 = 0xE,                                      /**< Reserved */
        Reserved4 = 0xF                                       /**< Reserved */
    }
}