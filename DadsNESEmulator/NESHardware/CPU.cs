/**
 *  @file           CPU.cs
 *  @brief          Defines the MOS6502 (lacking decimal mode) CPU portion of the 2A03 (RP2A03[G]) NTSC NES CPU.
 *  
 *  @copyright      2019
 *  @date           11/04/2019
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
namespace DadsNESEmulator.NESHardware
{
    /** @brief  Class that defines the NMOS6502 CPU. */
    public class CPU
    {
        /** - https://wiki.nesdev.com/w/index.php/CPU_registers */

        /** - @brief Accumulator - A is byte-wide and along with the arithmetic logic unit (ALU), supports using the status register for carrying, overflow detection, and so on. */
        private byte A;

        /** - @brief X is byte-wide and used for several addressing modes. It can be used as a loop counter easily, using INC/DEC and branch instructions. Not being the accumulator,
         * it has limited addressing modes when loading and saving.
         */
        private byte X;

        /** - @brief Y is byte-wide and used for several addressing modes. It can be used as a loop counter easily, using INC/DEC and branch instructions. Not being the accumulator,
         * it has limited addressing modes when loading and saving.
         */
        private byte Y;

        /** - @brief Program Counter - The 2-byte program counter PC supports 65536 direct (unbanked) memory locations, however not all values are sent to the cartridge. It can be
         *    accessed either by allowing CPU's internal fetch logic increment the address bus, an interrupt (NMI, Reset, IRQ/BRQ), and using the RTS/JMP/JSR/Branch instructions.
         */
        private ushort PC;

        /** - @brief Stack Pointer - SP is byte-wide and can be accessed using interrupts, pulls, pushes, and transfers. */
        private byte SP;

        /** - @brief SR - P has 6 bits used by the ALU but is byte-wide. PHP, PLP, arithmetic, testing, and branch instructions can access this register. */
        private byte SR;

        public void Run()
        {
            RAM ram = new RAM(0x8000);
        }

        public void PowerUP()
        {
            /** - https://wiki.nesdev.com/w/index.php/CPU_power_up_state */

            /**
             * P = $34[1] (IRQ disabled)
             * A, X, Y = 0
             * S = $FD
             * $4017 = $00 (frame irq enabled)
             * $4015 = $00 (all channels disabled)
             * $4000-$400F = $00
             * $4010-$4013 = $00
             * All 15 bits of noise channel LFSR = $0000[5]. The first time the LFSR is clocked from the all-0s state, it will shift in a 1.
             * 2A03G: APU Frame Counter reset. (but 2A03letterless: APU frame counter powers up at a value equivalent to 15)
             * Internal memory ($0000-$07FF) has unreliable startup state. Some machines may have consistent RAM contents at power-on, but others do not.
             * Emulators often implement a consistent RAM startup state (e.g. all $00 or $FF, or a particular pattern), and flash carts like the PowerPak may
             * partially or fully initialize RAM before starting a program, so an NES programmer must be careful not to rely on the startup contents of RAM.
             */
            A = 0x00;
            X = 0x00;
            Y = 0x00;
            SR = 0x34;
            SP = 0xFD;
            PC = 0;
        }

        public void Reset()
        {

            /**
             * A, X, Y were not affected
             * S was decremented by 3 (but nothing was written to the stack)[3]
             * The I (IRQ disable) flag was set to true (status ORed with $04)
             * The internal memory was unchanged
             * APU mode in $4017 was unchanged
             * APU was silenced ($4015 = 0)
             * APU triangle phase is reset to 0 (i.e. outputs a value of 15, the first step of its waveform)
             * APU DPCM output ANDed with 1 (upper 6 bits cleared)
             * 2A03G: APU Frame Counter reset. (but 2A03letterless: APU frame counter retains old value)
             */
        }


    }
}
 
 