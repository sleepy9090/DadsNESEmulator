/**
 *  @file           ProcessorFlags.cs
 *  @brief          Defines the Processor Flags.
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
    /**
     * Processor Flags References:
     *  - https://wiki.nesdev.com/w/index.php/Status_flags
     */

    /*
     * 7  bit  0
     * ---- ----
     * NVss DIZC
     * |||| ||||
     * |||| |||+- Carry
     * |||| ||+-- Zero
     * |||| |+--- Interrupt Disable
     * |||| +---- Decimal
     * ||++------ No CPU effect, see: the B flag
     * |+-------- Overflow
     * +--------- Negative
     */
    public enum ProcessorFlags : byte
    {
        N = 0x80, /**< 128 - Negative flag */
        V = 0x40, /**< 64 - Overflow flag */
        R = 0x20, /**< 32 - Reserved/Ignored */
        B = 0x10, /**< 16 - No CPU effect, see: the B flag */
        D = 0x8, /**< 8 - Decimal flag */
        I = 0x4, /**< 4 - Interrupt Disable flag */
        Z = 0x2, /**< 2 - Zero flag */
        C = 0x1  /**< 1 - Carry flag */
    }

    /**
     * C: Carry
     * After ADC, this is the carry result of the addition.
     * After SBC or CMP, this flag will be set if no borrow was the result, or alternatively a "greater than or equal" result.
     * After a shift instruction (ASL, LSR, ROL, ROR), this contains the bit that was shifted out.
     * Increment and decrement instructions do not affect the carry flag.
     * Can be set or cleared directly with SEC, CLC.
     *
     * Z: Zero
     * After most instructions that have a value result, if that value is zero, this flag will be set.
     *
     * I: Interrupt Disable
     * When set, all interrupts except the NMI are inhibited.
     * Can be set or cleared directly with SEI, CLI.
     * Automatically set by the CPU when an IRQ is triggered, and restored to its previous state by RTI.
     * If the /IRQ line is low (IRQ pending) when this flag is cleared, an interrupt will immediately be triggered.
     *
     * D: Decimal
     * On the NES, this flag has no effect.
     * On the original 6502, this flag causes some arithmetic instructions to use binary-coded decimal representation to make base 10 calculations easier.
     * Can be set or cleared directly with SED, CLD.
     *
     * V: Overflow
     * ADC and SBC will set this flag if the signed result would be invalid[1], necessary for making signed comparisons[2].
     * BIT will load bit 6 of the addressed value directly into the V flag.
     * Can be cleared directly with CLV. There is no corresponding set instruction.
     *
     * N: Negative
     * After most instructions that have a value result, this flag will contain bit 7 of that result.
     * BIT will load bit 7 of the addressed value directly into the N flag.
     *
     * The B flag
     * While there are only six flags in the processor status register within the CPU, when transferred to the stack, there are two additional bits.
     * These do not represent a register that can hold a value but can be used to distinguish how the flags were pushed.
     *
     * Some 6502 references call this the "B flag", though it does not represent an actual CPU register.
     *
     * Two interrupts (/IRQ and /NMI) and two instructions (PHP and BRK) push the flags to the stack. In the byte pushed, bit 5 is always set to 1,
     * and bit 4 is 1 if from an instruction (PHP or BRK) or 0 if from an interrupt line being pulled low (/IRQ or /NMI). This is the only time and place
     * where the B flag actually exists: not in the status register itself, but in bit 4 of the copy that is written to the stack.
     *
     * Instruction	Bits 5 and 4	Side effects after pushing
     * PHP          11              None
     * BRK          11              I is set to 1
     * /IRQ         10              I is set to 1
     * /NMI         10              I is set to 1
     *
     * Two instructions (PLP and RTI) pull a byte from the stack and set all the flags. They ignore bits 5 and 4.
     *
     * The only way for an IRQ handler to distinguish /IRQ from BRK is to read the flags byte from the stack and test bit 4. The slowness of this is one
     * reason why BRK wasn't used as a syscall mechanism. Instead, it was more often used to trigger a patching mechanism that hung off the /IRQ vector:
     * a single byte in PROM, UVEPROM, flash, etc. would be forced to 0, and the IRQ handler would pick something to do instead based on the program counter.
     *
     * Unlike bits 5 and 4, bit 3 actually exists in P, even though it doesn't affect the ALU operation on the 2A03 or 2A07 CPU the way it does in MOS Technology's own chips.
     */
}