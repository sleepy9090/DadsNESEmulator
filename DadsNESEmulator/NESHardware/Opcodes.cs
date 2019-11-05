/**
 *  @file           Opcodes.cs
 *  @brief          Defines the NMOS6502 Opcodes.
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
    /** @brief  Class that defines the NMOS6502 Opcodes. */
    public static class Opcodes
    {
        /**
         * Opcode References:
         *  - https://wiki.nesdev.com/w/index.php/CPU_unofficial_opcodes
         *  - http://nesdev.com/6502_cpu.txt
         *  - http://www.oxyron.de/html/opcodes02.html
         *  - http://6502.org/tutorials/6502opcodes.html
         *  - http://nesdev.com/undocumented_opcodes.txt
         *  - https://emudev.de/nes-emulator/opcodes-and-addressing-modes-the-6502/
         */

        /**
         * CPU addressing modes:
         * immediate           - This is the address right after the opcode (PC+1)
         * zeropage	           - This is the address of the value at PC+1. (8 bit force it to be on zeropage, 0x00nn)
         * zeropage, x-indexed - Same as zeropage, but offset with register X
         * zeropage, y-indexed - Same as zeropage, but offset with register Y
         * indirect, x-indexed - Read the value of the immediate byte. Use this value + X (low nibble), and this value + X + 1 (high nibble) as an address
         * indirect, y-indexed - Read the value of the immediate byte. Use this value (low nibble), and this value + 1 (high nibble) as an address. Add Y to this address
         * absolute            - This gives a complete address with the next 2 bytes (little Endian, so low nibble comes first)
         * absolute, x-indexed - Same as absolute, but offset with register X
         * absolute, y-indexed - Same as absolute, but offset with register Y
         */

        /** - Opcode, bytes, cycles, notes */

        #region Official Opcodes

        /** - ADC (ADd with Carry) */
        /** - Affects Flags: N V Z C */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _ADC_IMMEDIATE = 0x69; // 2, 2
        public const byte _ADC_ZERO_PAGE = 0x65; // 2, 3
        public const byte _ADC_ZERO_PAGE_X = 0x75; // 2, 4
        public const byte _ADC_ABSOLUTE = 0x6D; // 3, 4
        public const byte _ADC_ABSOULTE_X = 0x7D; // 3, 4+
        public const byte _ADC_ABSOLUTE_Y = 0x79; // 3, 4+
        public const byte _ADC_INDIRECT_X = 0x61; // 2, 6
        public const byte _ADC_INDIRECT_Y = 0x71; // 2, 5+

        /** - AND (bitwise AND with accumulator) */
        /** - Affects Flags: N V */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _AND_IMMEDIATE = 0x29; // 2, 2
        public const byte _AND_ZERO_PAGE = 0x25; // 2, 3
        public const byte _AND_ZERO_PAGE_X = 0x35; // 2, 4
        public const byte _AND_ABSOLUTE = 0x2D; // 3, 4
        public const byte _AND_ABSOULTE_X = 0x3D; // 3, 4+
        public const byte _AND_ABSOLUTE_Y = 0x39; // 3, 4+
        public const byte _AND_INDIRECT_X = 0x21; // 2, 6
        public const byte _AND_INDIRECT_Y = 0x31; // 2, 5+

        /** - ASL (Arithmetic Shift Left) */
        /** - Affects Flags: N Z C */
        public const byte _ASL_ACCUMULATOR = 0x0A; // 1, 2
        public const byte _ASL_ZERO_PAGE = 0x06; // 2, 5
        public const byte _ASL_ZERO_PAGE_X = 0x16; // 2, 6
        public const byte _ASL_ABSOLUTE = 0x0E; // 3, 6
        public const byte _ASL_ABSOULTE_X = 0x1E; // 3, 7

        /** - BIT (test BITs) */
        /** - Affects Flags: N V Z */
        public const byte _BIT_ZERO_PAGE = 0x24; // 2, 3
        public const byte _BIT_ABSOLUTE = 0x2C; // 3, 4

        /** - Branch Instructions */
        /** - Affects Flags: none */
        /** - + Branches are dependent on the status of the flag bits when the op code is encountered. A branch not taken requires two machine cycles. Add one if the branch is taken and add one more if the branch crosses a page boundary. */
        public const byte _BPL = 0x10; // 2, 2+, (Branch on PLus)
        public const byte _BMI = 0x30; // 2, 2+, (Branch on MInus)
        public const byte _BVC = 0x50; // 2, 2+, (Branch on oVerflow Clear)
        public const byte _BVS = 0x70; // 2, 2+, (Branch on oVerflow Set)
        public const byte _BCC = 0x90; // 2, 2+, (Branch on Carry Clear)
        public const byte _BCS = 0xB0; // 2, 2+, (Branch on Carry Set)
        public const byte _BNE = 0xD0; // 2, 2+, (Branch on Not Equal) 
        public const byte _BEQ = 0xF0; // 2, 2+, (Branch on EQual) 

        /** - BRK (BReaK) */
        /** - Affects Flags: B */
        public const byte _BRK = 0x00;

        /** - CMP (CoMPare accumulator) */
        /** - Affects Flags: N Z C */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _CMP_IMMEDIATE = 0xC9; // 2, 2
        public const byte _CMP_ZERO_PAGE = 0xC5; // 2, 3
        public const byte _CMP_ZERO_PAGE_X = 0xD5; // 2, 4
        public const byte _CMP_ABSOLUTE = 0xCD; // 3, 4
        public const byte _CMP_ABSOULTE_X = 0xDD; // 3, 4+
        public const byte _CMP_ABSOLUTE_Y = 0xD9; // 3, 4+
        public const byte _CMP_INDIRECT_X = 0xC1; // 2, 6
        public const byte _CMP_INDIRECT_Y = 0xD1; // 2, 5+

        /** - CPX (ComPare X register) */
        /** - Affects Flags: N Z C */
        public const byte _CPX_IMMEDIATE = 0xE0; // 2, 2
        public const byte _CPX_ZERO_PAGE = 0xE4; // 2, 3
        public const byte _CPX_ABSOLUTE = 0xEC; // 3, 4

        /** - CPY (ComPare Y register) */
        /** - Affects Flags: N Z C */
        public const byte _CPY_IMMEDIATE = 0xC0; // 2, 2
        public const byte _CPY_ZERO_PAGE = 0xC4; // 2, 3
        public const byte _CPY_ABSOLUTE = 0xCC; // 3, 4

        /** - DEC (DECrement memory) */
        /** - Affects Flags: N Z */
        public const byte _DEC_ZERO_PAGE = 0xC6; // 2, 5
        public const byte _DEC_ZERO_PAGE_X = 0xD6; // 2, 6
        public const byte _DEC_ABSOLUTE = 0xCE; // 3, 6
        public const byte _DEC_ABSOULTE_X = 0xDE; // 3, 7

        /** - EOR (bitwise Exclusive OR) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _EOR_IMMEDIATE = 0x29; // 2, 2
        public const byte _EOR_ZERO_PAGE = 0x25; // 2, 3
        public const byte _EOR_ZERO_PAGE_X = 0x35; // 2, 4
        public const byte _EOR_ABSOLUTE = 0x2D; // 3, 4
        public const byte _EOR_ABSOULTE_X = 0x3D; // 3, 4+
        public const byte _EOR_ABSOLUTE_Y = 0x39; // 3, 4+
        public const byte _EOR_INDIRECT_X = 0x21; // 2, 6
        public const byte _EOR_INDIRECT_Y = 0x31; // 2, 5+

        /** - Flag (Processor Status) Instructions */
        /** - Affects Flags: as noted */
        /** -
         * The Interrupt flag is used to prevent (SEI) or enable (CLI) maskable interrupts (aka IRQ's). It does not signal the presence or absence of an interrupt condition. The 6502 will set this flag automatically in response to an
         * interrupt and restore it to its prior status on completion of the interrupt service routine. If you want your interrupt service routine to permit other maskable interrupts, you must clear the I flag in your code. The Decimal
         * flag controls how the 6502 adds and subtracts. If set, arithmetic is carried out in packed binary coded decimal. This flag is unchanged by interrupts and is unknown on power-up. The implication is that a CLD should be
         * included in boot or interrupt coding. The Overflow flag is generally misunderstood and therefore under-utilised. After an ADC or SBC instruction, the overflow flag will be set if the twos complement result is less than -128
         * or greater than +127, and it will cleared otherwise. In twos complement, $80 through $FF represents -128 through -1, and $00 through $7F represents 0 through +127. Thus, after:
         * CLC
         * LDA #$7F ;   +127
         * ADC #$01 ; +   +1
         * the overflow flag is 1 (+127 + +1 = +128), and after:
         * CLC
         * LDA #$81 ;   -127
         * ADC #$FF ; +   -1
         * the overflow flag is 0 (-127 + -1 = -128). The overflow flag is not affected by increments, decrements, shifts and logical operations i.e. only ADC, BIT, CLV, PLP, RTI and SBC affect it. There is no op code to set the
         * overflow but a BIT test on an RTS instruction will do the trick.
         */
        public const byte _CLC = 0x18; // 1, 2, (CLear Carry)
        public const byte _SEC = 0x38; // 1, 2, (SEt Carry) 
        public const byte _CLI = 0x58; // 1, 2, (CLear Interrupt) 
        public const byte _SEI = 0x78; // 1, 2, (SEt Interrupt)
        public const byte _CLV = 0xB8; // 1, 2, (CLear oVerflow)
        public const byte _CLD = 0xD8; // 1, 2, (CLear Decimal)
        public const byte _SED = 0xF8; // 1, 2, (SEt Decimal)

        /** - INC (INCrement memory) */
        /** - Affects Flags: N Z */
        public const byte _INC_ZERO_PAGE = 0xE6; // 2, 5
        public const byte _INC_ZERO_PAGE_X = 0xF6; // 2, 6
        public const byte _INC_ABSOLUTE = 0xEE; // 3, 6
        public const byte _INC_ABSOULTE_X = 0xFE; // 3, 7

        /** - JMP (JuMP) */
        /** - Affects Flags: none */
        public const byte _JMP_ABSOLUTE = 0x4C; // 3, 3
        public const byte _JMP_INDIRECT = 0x6C; // 3, 5

        /** - JSR (Jump to SubRoutine) */
        /** - Affects Flags: none */
        public const byte _JSR_ABSOLUTE = 0x20; // 3, 6

        /** - LDA (LoaD Accumulator) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _LDA_IMMEDIATE = 0xA9; // 2, 2
        public const byte _LDA_ZERO_PAGE = 0xA5; // 2, 3
        public const byte _LDA_ZERO_PAGE_X = 0xB5; // 2, 4
        public const byte _LDA_ABSOLUTE = 0xAD; // 3, 4
        public const byte _LDA_ABSOULTE_X = 0xBD; // 3, 4+
        public const byte _LDA_ABSOLUTE_Y = 0xB9; // 3, 4+
        public const byte _LDA_INDIRECT_X = 0xA1; // 2, 6
        public const byte _LDA_INDIRECT_Y = 0xB1; // 2, 5+

        /** - LDX (LoaD X register) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _LDX_IMMEDIATE = 0xA2; // 2, 2
        public const byte _LDX_ZERO_PAGE = 0xA6; // 2, 3
        public const byte _LDX_ZERO_PAGE_Y = 0xB6; // 2, 4
        public const byte _LDX_ABSOLUTE = 0xAE; // 3, 4
        public const byte _LDX_ABSOULTE_Y = 0xBE; // 3, 4+

        /** - LDY (LoaD Y register) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _LDY_IMMEDIATE = 0xA0; // 2, 2
        public const byte _LDY_ZERO_PAGE = 0xA4; // 2, 3
        public const byte _LDY_ZERO_PAGE_X = 0xB4; // 2, 4
        public const byte _LDY_ABSOLUTE = 0xAC; // 3, 4
        public const byte _LDY_ABSOULTE_X = 0xBC; // 3, 4+

        /** - LSR (Logical Shift Right) */
        /** - Affects Flags: N Z C */
        public const byte _LSR_ACCUMULATOR = 0x4A; // 1, 2
        public const byte _LSR_ZERO_PAGE = 0x46; // 2, 5
        public const byte _LSR_ZERO_PAGE_X = 0x56; // 2, 6
        public const byte _LSR_ABSOLUTE = 0x4E; // 3, 6
        public const byte _LSR_ABSOULTE_X = 0x5E; // 3, 7

        /** - NOP (No OPeration) */
        /** - Affects Flags: none */
        public const byte _NOP = 0xEA; // 1, 2

        /** - ORA (bitwise OR with Accumulator) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _ORA_IMMEDIATE = 0x09; // 2, 2
        public const byte _ORA_ZERO_PAGE = 0x05; // 2, 3
        public const byte _ORA_ZERO_PAGE_X = 0x15; // 2, 4
        public const byte _ORA_ABSOLUTE = 0x0D; // 3, 4
        public const byte _ORA_ABSOULTE_X = 0x1D; // 3, 4+
        public const byte _ORA_ABSOLUTE_Y = 0x19; // 3, 4+
        public const byte _ORA_INDIRECT_X = 0x01; // 2, 6
        public const byte _ORA_INDIRECT_Y = 0x11; // 2, 5+

        /** - Register Instructions */
        /** - Affects Flags: N Z */
        public const byte _TAX = 0xAA; // 1, 2, (Transfer A to X)
        public const byte _TXA = 0x8A; // 1, 2, (Transfer X to A)
        public const byte _DEX = 0xCA; // 1, 2, (DEcrement X) 
        public const byte _INX = 0xE8; // 1, 2, (INcrement X) 
        public const byte _TAY = 0xA8; // 1, 2, (Transfer A to Y)
        public const byte _TYA = 0x98; // 1, 2, (Transfer Y to A)
        public const byte _DEY = 0x88; // 1, 2, (DEcrement Y)
        public const byte _INY = 0xC8; // 1, 2, (INcrement Y)

        /** - ROL (ROtate Left) */
        /** - Affects Flags: N Z C */
        public const byte _ROL_ACCUMULATOR = 0x2A; // 1, 2
        public const byte _ROL_ZERO_PAGE = 0x26; // 2, 5
        public const byte _ROL_ZERO_PAGE_X = 0x36; // 2, 6
        public const byte _ROL_ABSOLUTE = 0x2E; // 3, 6
        public const byte _ROL_ABSOULTE_X = 0x3E; // 3, 7

        /** - ROR (ROtate Right) */
        /** - Affects Flags: N Z C */
        public const byte _ROR_ACCUMULATOR = 0x6A; // 1, 2
        public const byte _ROR_ZERO_PAGE = 0x66; // 2, 5
        public const byte _ROR_ZERO_PAGE_X = 0x76; // 2, 6
        public const byte _ROR_ABSOLUTE = 0x6E; // 3, 6
        public const byte _ROR_ABSOULTE_X = 0x7E; // 3, 7

        /** - RTI (ReTurn from Interrupt) */
        /** - Affects Flags: all */
        public const byte _RTI_IMPLIED = 0x40; // 1, 6

        /** - RTS (ReTurn from Subroutine) */
        /** - Affects Flags: none */
        public const byte _RTS_IMPLIED = 0x60; // 1, 6

        /** - SBC (SuBtract with Carry) */
        /** - Affects Flags: N V Z C */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _SBC_IMMEDIATE = 0xE9; // 2, 2
        public const byte _SBC_ZERO_PAGE = 0xE5; // 2, 3
        public const byte _SBC_ZERO_PAGE_X = 0xF5; // 2, 4
        public const byte _SBC_ABSOLUTE = 0xED; // 3, 4
        public const byte _SBC_ABSOULTE_X = 0xFD; // 3, 4+
        public const byte _SBC_ABSOLUTE_Y = 0xF9; // 3, 4+
        public const byte _SBC_INDIRECT_X = 0xE1; // 2, 6
        public const byte _SBC_INDIRECT_Y = 0xF1; // 2, 5+

        /** - STA (STore Accumulator) */
        /** - Affects Flags: none */
        public const byte _STA_ZERO_PAGE = 0x85; // 2, 3
        public const byte _STA_ZERO_PAGE_X = 0x95; // 2, 4
        public const byte _STA_ABSOLUTE = 0x8D; // 3, 4
        public const byte _STA_ABSOULTE_X = 0x9D; // 3, 5
        public const byte _STA_ABSOLUTE_Y = 0x99; // 3, 5
        public const byte _STA_INDIRECT_X = 0x81; // 2, 6
        public const byte _STA_INDIRECT_Y = 0x91; // 2, 6

        /** - Stack Instructions */
        /** -
         * These instructions are implied mode, have a length of one byte and require machine cycles as indicated. The "PuLl" operations are known as "POP" on most other microprocessors. With the 6502, the stack is always on page one
         * ($100-$1FF) and works top down.
         */
        public const byte _TXS = 0x9A; // 1, 2, (Transfer X to Stack ptr)
        public const byte _TSX = 0xBA; // 1, 2, (Transfer Stack ptr to X)
        public const byte _PHA = 0x4A; // 1, 3, (PusH Accumulator) 
        public const byte _PLA = 0x68; // 1, 4, (PuLl Accumulator)
        public const byte _PHP = 0x08; // 1, 3, (PusH Processor status)
        public const byte _PLP = 0x28; // 1, 4, (PuLl Processor status)

        /** - STX (STore X register) */
        /** - Affects Flags: none */
        public const byte _STX_ZERO_PAGE = 0x86; // 2, 3
        public const byte _STX_ZERO_PAGE_Y = 0x96; // 2, 4
        public const byte _STX_ABSOLUTE = 0x8E; // 3, 4

        /** - STY (STore Y register) */
        /** - Affects Flags: none */
        public const byte _STY_ZERO_PAGE = 0x84; // 2, 3
        public const byte _STY_ZERO_PAGE_Y = 0x94; // 2, 4
        public const byte _STY_ABSOLUTE = 0x8C; // 3, 4

        #endregion

        #region Unofficial / Illegal Opcodes

        /** - Unofficial / Illegal Opcodes */

        /** - AAC (ANC) [ANC] (AND byte with accumulator. If result is negative then carry is set.) */
        /** - Affects Flags: N Z C */
        public const byte _AAC_IMMEDIATE = 0x0B; // 2, 2
        public const byte _AAC_IMMEDIATE_ALT = 0x2B; // 2, 2

        /** - AAX (SAX) [AXS] (AND X register with accumulator and store result in memory.) */
        /** - Affects Flags: N Z */
        public const byte _AAX_ZERO_PAGE = 0x87; // 2, 3
        public const byte _AAX_ZERO_PAGE_Y = 0x97; // 2, 4
        public const byte _AAX_INDIRECT_X = 0x83; // 2, 6
        public const byte _AAX_ABSOLUTE = 0x8F; // 3, 4

        /** - ARR (ARR) [ARR] (AND byte with accumulator, then rotate one bit right in accumulator and check bit 5 and 6:
         * If both bits are 1: set C, clear V.
         * If both bits are 0: clear C and V.
         * If only bit 5 is 1: set V, clear C.
         * If only bit 6 is 1: set C and V.
         * )
         */
        /** - Affects Flags: N V Z C */
        public const byte _ARR_IMMEDIATE = 0x6B; // 2, 2

        /** - ASR (ASR) [ALR] (AND byte with accumulator, then shift right one bit in accumulator.) */
        /** - Affects Flags: N V Z C */
        public const byte _ASR_IMMEDIATE = 0x4B; // 2, 2

        /** - ATX (LXA) [OAL] (AND byte with accumulator, then transfer accumulator to X register.) */
        /** - Affects Flags: N Z */
        public const byte _ATX_IMMEDIATE = 0xAB; // 2, 2

        /** - AXA (SHA) [AXA] (AND X register with accumulator then AND result with 7 and store in memory.) */
        /** - Affects Flags: none */
        public const byte _AXA_ABSOLUTE_Y = 0x9F; // 3, 5
        public const byte _AXA_INDIRECT_Y = 0x93; // 2, 6

        /** - AXS (SBX) [SAX] (AND X register with accumulator and store result in X regis-ter, then subtract byte from X register (without borrow).) */
        /** - Affects Flags: N Z C */
        public const byte _AXS_IMMEDIATE = 0xCB; // 2, 2

        /** - DCP (DCP) [DCM] (Subtract 1 from memory (without borrow).) */
        /** - Affects Flags: C */
        public const byte _DCP_ZERO_PAGE = 0xC7; // 2, 5
        public const byte _DCP_ZERO_PAGE_X = 0xD7; // 2, 6
        public const byte _DCP_ABSOLUTE = 0xCF; // 3, 6
        public const byte _DCP_ABSOLUTE_X = 0xDF; // 3, 7
        public const byte _DCP_ABSOLUTE_Y = 0xDB; // 3, 7
        public const byte _DCP_INDIRECT_X = 0xC3; // 2, 8
        public const byte _DCP_INDIRECT_Y = 0xD3; // 2, 8

        /** - DOP (NOP) [SKB] (No operation (double NOP). The argument has no significance.) */
        /** - Affects Flags: none */
        public const byte _DOP_ZERO_PAGE = 0x04; // 2, 3
        public const byte _DOP_ZERO_PAGE_X = 0x14; // 2, 4
        public const byte _DOP_ZERO_PAGE_X_ALT = 0x34; // 2, 4
        public const byte _DOP_ZERO_PAGE_ALT = 0x44; // 2, 3
        public const byte _DOP_ZERO_PAGE_X_ALT_2 = 0x54; // 2, 4
        public const byte _DOP_ZERO_PAGE_ALT_2 = 0x64; // 2, 3
        public const byte _DOP_ZERO_PAGE_X_ALT_3 = 0x74; // 2, 4
        public const byte _DOP_IMMEDIATE = 0x80; // 2, 2
        public const byte _DOP_IMMEDIATE_ALT = 0x82; // 2, 2
        public const byte _DOP_IMMEDIATE_ALT_2 = 0x89; // 2, 2
        public const byte _DOP_IMMEDIATE_ALT_3 = 0xC2; // 2, 2
        public const byte _DOP_ZERO_PAGE_X_ALT_4 = 0xD4; // 2, 4
        public const byte _DOP_IMMEDIATE_ALT_4 = 0xE2; // 2, 2
        public const byte _DOP_ZERO_PAGE_X_ALT_5 = 0xF4; // 2, 4

        /** - ISC (ISB) [INS] (Increase memory by one, then subtract memory from accumulator (with borrow).) */
        /** - Affects Flags: N V Z C */
        public const byte _ISC_ZERO_PAGE = 0xE7; // 2, 5
        public const byte _ISC_ZERO_PAGE_X = 0xF7; // 2, 6
        public const byte _ISC_ABSOLUTE = 0xEF; // 3, 6
        public const byte _ISC_ABSOLUTE_X = 0xFF; // 3, 7
        public const byte _ISC_ABSOLUTE_Y = 0xFB; // 3, 7
        public const byte _ISC_INDIRECT_X = 0xE3; // 2, 8
        public const byte _ISC_INDIRECT_Y = 0xF3; // 2, 8

        /** - KIL (JAM) [HLT] (Stop program counter (processor lock up).) */
        /** - Affects Flags: none */
        public const byte _KIL_IMPLIED = 0x02; // 1, -
        public const byte _KIL_IMPLIED_ALT = 0x12; // 1, -
        public const byte _KIL_IMPLIED_ALT_2 = 0x22; // 1, -
        public const byte _KIL_IMPLIED_ALT_3 = 0x32; // 1, -
        public const byte _KIL_IMPLIED_ALT_4 = 0x42; // 1, -
        public const byte _KIL_IMPLIED_ALT_5 = 0x52; // 1, -
        public const byte _KIL_IMPLIED_ALT_6 = 0x62; // 1, -
        public const byte _KIL_IMPLIED_ALT_7 = 0x72; // 1, -
        public const byte _KIL_IMPLIED_ALT_8 = 0x92; // 1, -
        public const byte _KIL_IMPLIED_ALT_9 = 0xB2; // 1, -
        public const byte _KIL_IMPLIED_ALT_10 = 0xD2; // 1, -
        public const byte _KIL_IMPLIED_ALT_11 = 0xF2; // 1, -

        /** - LAR (LAE) [LAS] (AND memory with stack pointer, transfer result to accumulator, X register, and stack pointer.) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _LAR_ABSOLUTE_Y = 0xBB; // 3, 4+

        /** - LAX (LAX) [LAX] (Load accumulator and X register with memory.) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _LAX_ZERO_PAGE = 0xA7; // 2, 3
        public const byte _LAX_ZERO_PAGE_Y = 0xB7; // 2, 4
        public const byte _LAX_ABSOLUTE = 0xAF; // 3, 4
        public const byte _LAX_ABSOLUTE_Y = 0xBF; // 3, 4+
        public const byte _LAX_INDIRECT_X = 0xA3; // 2, 6
        public const byte _LAX_INDIRECT_Y = 0xB3; // 2, 5+

        /** - NOP (NOP) [NOP] (No operation.) */
        /** - Affects Flags: none */
        public const byte _NOP_IMPLIED = 0x1A; // 1, 2
        public const byte _NOP_IMPLIED_ALT = 0x3A; // 1, 2
        public const byte _NOP_IMPLIED_ALT_2 = 0x5A; // 1, 2
        public const byte _NOP_IMPLIED_ALT_3 = 0x7A; // 1, 2
        public const byte _NOP_IMPLIED_ALT_4 = 0xDA; // 1, 2
        public const byte _NOP_IMPLIED_ALT_5 = 0xFA; // 1, 2

        /** - RLA (RLA) [RLA] (Rotate one bit left in memory, then AND accumulator with memory.) */
        /** - Affects Flags: N Z C */
        public const byte _RLA_ZERO_PAGE = 0x27; // 2, 5
        public const byte _RLA_ZERO_PAGE_X = 0x37; // 2, 6
        public const byte _RLA_ABSOLUTE = 0x2F; // 3, 6
        public const byte _RLA_ABSOLUTE_X = 0x3F; // 3, 7
        public const byte _RLA_ABSOLUTE_Y = 0x3B; // 3, 7
        public const byte _RLA_INDIRECT_X = 0x23; // 2, 8
        public const byte _RLA_INDIRECT_Y = 0x33; // 2, 8

        /** - RRA (RRA) [RRA] (Rotate one bit right in memory, then add memory to accumulator (with carry).) */
        /** - Affects Flags: N Z C */
        public const byte _RRA_ZERO_PAGE = 0x67; // 2, 5
        public const byte _RRA_ZERO_PAGE_X = 0x77; // 2, 6
        public const byte _RRA_ABSOLUTE = 0x6F; // 3, 6
        public const byte _RRA_ABSOLUTE_X = 0x7F; // 3, 7
        public const byte _RRA_ABSOLUTE_Y = 0x7B; // 3, 7
        public const byte _RRA_INDIRECT_X = 0x63; // 2, 8
        public const byte _RRA_INDIRECT_Y = 0x73; // 2, 8

        /** - SBC (SBC) [SBC] (The same as the legal opcode $E9 (SBC #byte).) */
        /** - Affects Flags: N V Z C */
        public const byte _SBC_IMMEDIATE_ALT = 0xEB; // 2, 2

        /** - SLO (SLO) [ASO] (Shift left one bit in memory, then OR accumulator with memory.) */
        /** - Affects Flags: N Z C */
        public const byte _SLO_ZERO_PAGE = 0x07; // 2, 5
        public const byte _SLO_ZERO_PAGE_X = 0x17; // 2, 6
        public const byte _SLO_ABSOLUTE = 0x0F; // 3, 6
        public const byte _SLO_ABSOLUTE_X = 0x1F; // 3, 7
        public const byte _SLO_ABSOLUTE_Y = 0x1B; // 3, 7
        public const byte _SLO_INDIRECT_X = 0x03; // 2, 8
        public const byte _SLO_INDIRECT_Y = 0x13; // 2, 8

        /** - SRE (SRE) [LSE] (Shift right one bit in memory, then EOR accumulator with memory.) */
        /** - Affects Flags: N Z C */
        public const byte _SRE_ZERO_PAGE = 0x47; // 2, 5
        public const byte _SRE_ZERO_PAGE_X = 0x57; // 2, 6
        public const byte _SRE_ABSOLUTE = 0x4F; // 3, 6
        public const byte _SRE_ABSOLUTE_X = 0x5F; // 3, 7
        public const byte _SRE_ABSOLUTE_Y = 0x5B; // 3, 7
        public const byte _SRE_INDIRECT_X = 0x43; // 2, 8
        public const byte _SRE_INDIRECT_Y = 0x53; // 2, 8

        /** - SXA (SHX) [XAS] (AND X register with the high byte of the target address of the argument + 1. Store the result in memory. M =3D X AND HIGH(arg) + 1 */
        /** - Affects Flags: none */
        public const byte _SXA_ABSOLUTE_Y = 0x9E; // 3, 5

        /** - SYA (SHY) [SAY] (AND Y register with the high byte of the target address of the argument + 1. Store the result in memory. M =3D Y AND HIGH(arg) + 1 */
        /** - Affects Flags: none */
        public const byte _SYA_ABSOLUTE_X = 0x9C; // 3, 5

        /** - SYA (SHY) [SAY] (AND Y register with the high byte of the target address of the argument + 1. Store the result in memory. M =3D Y AND HIGH(arg) + 1 */
        /** - Affects Flags: none */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _TOP_ABSOLUTE = 0x0C; // 3, 4
        public const byte _TOP_ABSOLUTE_X = 0x1C; // 3, 4+
        public const byte _TOP_ABSOLUTE_X_ALT = 0x3C; // 3, 4+
        public const byte _TOP_ABSOLUTE_X_ALT_2 = 0x5C; // 3, 4+
        public const byte _TOP_ABSOLUTE_X_ALT_3 = 0x7C; // 3, 4+
        public const byte _TOP_ABSOLUTE_X_ALT_4 = 0xDC; // 3, 4+
        public const byte _TOP_ABSOLUTE_X_ALT_5 = 0xFC; // 3, 4+

        /** - XAA (ANE) [XAA] (Exact operation unknown. Read the referenced documents for more information and observations.) */
        /** - Affects Flags: ? */
        public const byte _XAA_IMMEDIATE = 0x8B; // 2, 2

        /** - XAS (SHS) [TAS] (AND X register with accumulator and store result in stack pointer, then AND stack pointer with the high byte of the target address of the argument + 1. Store result in memory.
         * S =3D X AND A, M =3D S AND HIGH(arg) + 1
         */
        /** - Affects Flags: none */
        public const byte _XAS_ABSOLUTE_Y = 0x9B; // 3, 5

        #endregion

    }
}
 
 