/**
 *  @file           APU.cs
 *  @brief          Defines the audio processing unit (APU) portion of the 2A03 (RP2A03[G]) NTSC NES CPU which generates sound for games.
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
    public class APU
    {
        /** - https://wiki.nesdev.com/w/index.php/APU */

        /** - https://wiki.nesdev.com/w/index.php/APU_registers */

        /** - https://wiki.nesdev.com/w/index.php/2A03 */

        /** - @brief Duty and volume for square wave 1 */
        public const ushort _SQ1_VOL = 0x4000;

        /** - @brief Sweep control register for square wave 1 */
        public const ushort _SQ1_SWEEP = 0x4001;

        /** - @brief Low byte of period for square wave 1 */
        public const ushort _SQ1_LO = 0x4002;

        /** - @brief High byte of period and length counter value for square wave 1 */
        public const ushort _SQ1_HI = 0x4003;

        /** - @brief Duty and volume for square wave 2 */
        public const ushort _SQ2_VOL = 0x4004;

        /** - @brief Sweep control register for square wave 2 */
        public const ushort _SQ2_SWEEP = 0x4005;

        /** - @brief Low byte of period for square wave 2 */
        public const ushort _SQ2_LO = 0x4006;

        /** - @brief High byte of period and length counter value for square wave 2 */
        public const ushort _SQ2_HI = 0x4007;

        /** - @brief Triangle wave linear counter */
        public const ushort _TRI_LINEAR = 0x4008;

        /** - @brief Unused, but is eventually accessed in memory-clearing loops */
        public const ushort _UNUSED_1 = 0x4009;

        /** - @brief Low byte of period for triangle wave */
        public const ushort _TRI_LO = 0x400A;

        /** - @brief High byte of period and length counter value for triangle wave */
        public const ushort _TRI_HI = 0x400B;

        /** - @brief Volume for noise generator */
        public const ushort _NOISE_VOL = 0x400C;

        /** - @brief Unused, but is eventually accessed in memory-clearing loops */
        public const ushort _UNUSED_2 = 0x400D;

        /** - @brief Period and waveform shape for noise generator */
        public const ushort _NOISE_LO = 0x400E;

        /** - @brief Length counter value for noise generator */
        public const ushort _NOISE_HI = 0x400F;

        /** - @brief Play mode and frequency for DMC samples */
        public const ushort _DMC_FREQ = 0x4010;

        /** - @brief 7-bit DAC */
        public const ushort _DMC_RAW = 0x4011;

        /** - @brief Start of DMC waveform is at address $C000 + $40*$xx */
        public const ushort _DMC_START = 0x4012;

        /** - @brief Length of DMC waveform is $10*$xx + 1 bytes (128*$xx + 8 samples) */
        public const ushort _DMC_LEN = 0x4013;

        /** - @brief Writing $xx copies 256 bytes by reading from $xx00-$xxFF and writing to OAMDATA ($2004) */
        public const ushort _OAMDMA = 0x4014;

        /** - @brief Sound channels enable and status */
        public const ushort _SND_CHN = 0x4015;

        /** - @brief Joystick 1 data (R) and joystick strobe (W) */
        public const ushort _JOY1 = 0x4016;

        /** - @brief Joystick 2 data (R) and frame counter control (W) */
        public const ushort _JOY2 = 0x4017;

        /** - $4018-$401F	APU and I/O functionality that is normally disabled. See CPU Test Mode. */
        /** - https://wiki.nesdev.com/w/index.php/CPU_Test_Mode */
    }
}


 
 