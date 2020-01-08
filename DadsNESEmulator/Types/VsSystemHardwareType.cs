/**
 *  @file           VsSystemHardwareType.cs
 *  @brief          Defines the Vs. system hardware type.
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
    /** - https://wiki.nesdev.com/w/index.php/NES_2.0
     *    Vs. Hardware type (Header byte 13 D4..D7):
     */
    public enum VsSystemHardwareType : byte
    {
        VsUnisystemNormal = 0x0,                       /**< Vs. Unisystem (normal) */
        VsUnisystemRBIBaseballProtection = 0x1,        /**< Vs. Unisystem (RBI Baseball protection) */
        VsUnisystemTKOBoxingProtection = 0x2,          /**< Vs. Unisystem (TKO Boxing protection) */
        VsUnisystemSuperXeviousProtection = 0x3,       /**< Vs. Unisystem (Super Xevious protection) */
        VsUnisystemVsIceClimberJapanProtection = 0x4,  /**< Vs. Unisystem (Vs. Ice Climber Japan protection) */
        VsDualSystemNormal = 0x5,                      /**< Vs. Dual System (normal) */
        VsDualSystemRaidOnBungelingBayProtection = 0x6 /**< Vs. Dual System (Raid on Bungeling Bay protection) */
    }
}