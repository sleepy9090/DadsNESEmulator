﻿/**
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
        /** - + add 1 cycle if page boundary crossed */ // size, cycles
        public const byte _ADC_IMMEDIATE = 0x69; // 2, 2
        public const byte _ADC_ZERO_PAGE = 0x65; // 2, 3
        public const byte _ADC_ZERO_PAGE_X = 0x75; // 2, 4
        public const byte _ADC_ABSOLUTE = 0x6D; // 3, 4
        public const byte _ADC_ABSOLUTE_X = 0x7D; // 3, 4+
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
        public const byte _AND_ABSOLUTE_X = 0x3D; // 3, 4+
        public const byte _AND_ABSOLUTE_Y = 0x39; // 3, 4+
        public const byte _AND_INDIRECT_X = 0x21; // 2, 6
        public const byte _AND_INDIRECT_Y = 0x31; // 2, 5+

        /** - ASL (Arithmetic Shift Left) */
        /** - Affects Flags: N Z C */
        public const byte _ASL_ACCUMULATOR = 0x0A; // 1, 2
        public const byte _ASL_ZERO_PAGE = 0x06; // 2, 5
        public const byte _ASL_ZERO_PAGE_X = 0x16; // 2, 6
        public const byte _ASL_ABSOLUTE = 0x0E; // 3, 6
        public const byte _ASL_ABSOLUTE_X = 0x1E; // 3, 7

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
        public const byte _BRK = 0x00; // 2, 7

        /** - CMP (CoMPare accumulator) */
        /** - Affects Flags: N Z C */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _CMP_IMMEDIATE = 0xC9; // 2, 2
        public const byte _CMP_ZERO_PAGE = 0xC5; // 2, 3
        public const byte _CMP_ZERO_PAGE_X = 0xD5; // 2, 4
        public const byte _CMP_ABSOLUTE = 0xCD; // 3, 4
        public const byte _CMP_ABSOLUTE_X = 0xDD; // 3, 4+
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
        public const byte _DEC_ABSOLUTE_X = 0xDE; // 3, 7

        /** - EOR (bitwise Exclusive OR) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _EOR_IMMEDIATE = 0x49; // 2, 2
        public const byte _EOR_ZERO_PAGE = 0x45; // 2, 3
        public const byte _EOR_ZERO_PAGE_X = 0x55; // 2, 4
        public const byte _EOR_ABSOLUTE = 0x4D; // 3, 4
        public const byte _EOR_ABSOLUTE_X = 0x5D; // 3, 4+
        public const byte _EOR_ABSOLUTE_Y = 0x59; // 3, 4+
        public const byte _EOR_INDIRECT_X = 0x41; // 2, 6
        public const byte _EOR_INDIRECT_Y = 0x51; // 2, 5+

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
        public const byte _INC_ABSOLUTE_X = 0xFE; // 3, 7

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
        public const byte _LDA_ABSOLUTE_X = 0xBD; // 3, 4+
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
        public const byte _LDX_ABSOLUTE_Y = 0xBE; // 3, 4+

        /** - LDY (LoaD Y register) */
        /** - Affects Flags: N Z */
        /** - + add 1 cycle if page boundary crossed */
        public const byte _LDY_IMMEDIATE = 0xA0; // 2, 2
        public const byte _LDY_ZERO_PAGE = 0xA4; // 2, 3
        public const byte _LDY_ZERO_PAGE_X = 0xB4; // 2, 4
        public const byte _LDY_ABSOLUTE = 0xAC; // 3, 4
        public const byte _LDY_ABSOLUTE_X = 0xBC; // 3, 4+

        /** - LSR (Logical Shift Right) */
        /** - Affects Flags: N Z C */
        public const byte _LSR_ACCUMULATOR = 0x4A; // 1, 2
        public const byte _LSR_ZERO_PAGE = 0x46; // 2, 5
        public const byte _LSR_ZERO_PAGE_X = 0x56; // 2, 6
        public const byte _LSR_ABSOLUTE = 0x4E; // 3, 6
        public const byte _LSR_ABSOLUTE_X = 0x5E; // 3, 7

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
        public const byte _ORA_ABSOLUTE_X = 0x1D; // 3, 4+
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
        public const byte _ROL_ABSOLUTE_X = 0x3E; // 3, 7

        /** - ROR (ROtate Right) */
        /** - Affects Flags: N Z C */
        public const byte _ROR_ACCUMULATOR = 0x6A; // 1, 2
        public const byte _ROR_ZERO_PAGE = 0x66; // 2, 5
        public const byte _ROR_ZERO_PAGE_X = 0x76; // 2, 6
        public const byte _ROR_ABSOLUTE = 0x6E; // 3, 6
        public const byte _ROR_ABSOLUTE_X = 0x7E; // 3, 7

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
        public const byte _SBC_ABSOLUTE_X = 0xFD; // 3, 4+
        public const byte _SBC_ABSOLUTE_Y = 0xF9; // 3, 4+
        public const byte _SBC_INDIRECT_X = 0xE1; // 2, 6
        public const byte _SBC_INDIRECT_Y = 0xF1; // 2, 5+

        /** - STA (STore Accumulator) */
        /** - Affects Flags: none */
        public const byte _STA_ZERO_PAGE = 0x85; // 2, 3
        public const byte _STA_ZERO_PAGE_X = 0x95; // 2, 4
        public const byte _STA_ABSOLUTE = 0x8D; // 3, 4
        public const byte _STA_ABSOLUTE_X = 0x9D; // 3, 5
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
        public const byte _PHA = 0x48; // 1, 3, (PusH Accumulator) 
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
        public const byte _STY_ZERO_PAGE_X = 0x94; // 2, 4
        public const byte _STY_ABSOLUTE = 0x8C; // 3, 4

        #endregion

        #region Unofficial / Illegal Opcodes

        /** - Unofficial / Illegal Opcodes */

        /** - AAC (ANC) [ANC] (AND byte with accumulator. If result is negative then carry is set.) */
        /** - Affects Flags: N Z C */
        public const byte _AAC_IMMEDIATE = 0x0B; // 2, 2
        public const byte _AAC_IMMEDIATE_ALT = 0x2B; // 2, 2

        /** - AAX (SAX) [AXS] (AND X register with accumulator and store result in memory.) */
        /** - Affects Flags: None */
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
        /** - Affects Flags: N Z C */
        public const byte _ASR_IMMEDIATE = 0x4B; // 2, 2

        /** - ATX (LXA) [OAL] (AND byte with accumulator, then transfer accumulator to X register.) */
        /** - Affects Flags: N Z */
        public const byte _ATX_IMMEDIATE = 0xAB; // 2, 2

        /** - AXA (SHA) [AXA] (AND X register with accumulator then AND result with 7 and store in memory.) */
        /** - Affects Flags: none */
        public const byte _AXA_ABSOLUTE_Y = 0x9F; // 3, 5
        public const byte _AXA_INDIRECT_Y = 0x93; // 2, 6

        /** - AXS (SBX) [SAX] (AND X register with accumulator and store result in X register, then subtract byte from X register (without borrow).) */
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

        /** - SXA (SHX) [XAS] (AND X register with the high byte of the target address of the argument + 1. Store the result in memory. M =3D X AND HIGH(arg) + 1) */
        /** - Affects Flags: none */
        public const byte _SXA_ABSOLUTE_Y = 0x9E; // 3, 5

        /** - SYA (SHY) [SAY] (AND Y register with the high byte of the target address of the argument + 1. Store the result in memory. M =3D Y AND HIGH(arg) + 1) */
        /** - Affects Flags: none */
        public const byte _SYA_ABSOLUTE_X = 0x9C; // 3, 5

        /** - TOP (NOP) [SKW](No operation (tripple NOP). The argument has no significance.) */
        /** - Affects Flags: none */
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

        /**
         * @brief   This method returns the name of an opcode from the byte.
         *
         * @param   opcode = The opcode as byte.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        public static string GetOpcodeName(byte opcode)
        {
            string opcodeName;
            switch (opcode)
            {
                case _ADC_IMMEDIATE:
                    opcodeName = "ADC Imm";
                    break;
                case _ADC_ZERO_PAGE:
                    opcodeName = "ADC ZP";
                    break;
                case _ADC_ZERO_PAGE_X:
                    opcodeName = "ADC ZP,X";
                    break;
                case _ADC_ABSOLUTE:
                    opcodeName = "ADC Abs";
                    break;
                case _ADC_ABSOLUTE_X:
                    opcodeName = "ADC Abs,X";
                    break;
                case _ADC_ABSOLUTE_Y:
                    opcodeName = "ADC Abs,Y";
                    break;
                case _ADC_INDIRECT_X:
                    opcodeName = "ADC Ind,X";
                    break;
                case _ADC_INDIRECT_Y:
                    opcodeName = "ADC Ind,Y";
                    break;
                case _AND_IMMEDIATE:
                    opcodeName = "AND Imm";
                    break;
                case _AND_ZERO_PAGE:
                    opcodeName = "AND ZP";
                    break;
                case _AND_ZERO_PAGE_X:
                    opcodeName = "AND ZP,X";
                    break;
                case _AND_ABSOLUTE:
                    opcodeName = "AND Abs";
                    break;
                case _AND_ABSOLUTE_X:
                    opcodeName = "AND Abs,X";
                    break;
                case _AND_ABSOLUTE_Y:
                    opcodeName = "AND Abs,Y";
                    break;
                case _AND_INDIRECT_X:
                    opcodeName = "AND Ind,X";
                    break;
                case _AND_INDIRECT_Y:
                    opcodeName = "AND Ind,Y";
                    break;
                case _ASL_ACCUMULATOR:
                    opcodeName = "ASL Acc";
                    break;
                case _ASL_ZERO_PAGE:
                    opcodeName = "ASL ZP";
                    break;
                case _ASL_ZERO_PAGE_X:
                    opcodeName = "ASL ZP,X";
                    break;
                case _ASL_ABSOLUTE:
                    opcodeName = "ASL Abs";
                    break;
                case _ASL_ABSOLUTE_X:
                    opcodeName = "ASL Abs,X";
                    break;
                case _BIT_ZERO_PAGE:
                    opcodeName = "BIT ZP";
                    break;
                case _BIT_ABSOLUTE:
                    opcodeName = "BIT Abs";
                    break;
                case _BPL:
                    opcodeName = "BPL Rel";
                    break;
                case _BMI:
                    opcodeName = "BMI Rel";
                    break;
                case _BVC:
                    opcodeName = "BVC Rel";
                    break;
                case _BVS:
                    opcodeName = "BVS Rel";
                    break;
                case _BCC:
                    opcodeName = "BCC Rel";
                    break;
                case _BCS:
                    opcodeName = "BCS Rel";
                    break;
                case _BNE:
                    opcodeName = "BNE Rel";
                    break;
                case _BEQ:
                    opcodeName = "BEQ Rel";
                    break;
                case _BRK:
                    opcodeName = "BRK Imp";
                    break;
                case _CMP_IMMEDIATE:
                    opcodeName = "CMP Imm";
                    break;
                case _CMP_ZERO_PAGE:
                    opcodeName = "CMP ZP";
                    break;
                case _CMP_ZERO_PAGE_X:
                    opcodeName = "CMP ZP,X";
                    break;
                case _CMP_ABSOLUTE:
                    opcodeName = "CMP Abs";
                    break;
                case _CMP_ABSOLUTE_X:
                    opcodeName = "CMP Abs,X";
                    break;
                case _CMP_ABSOLUTE_Y:
                    opcodeName = "CMP Abs,Y";
                    break;
                case _CMP_INDIRECT_X:
                    opcodeName = "CMP Ind,X";
                    break;
                case _CMP_INDIRECT_Y:
                    opcodeName = "CMP Ind,Y";
                    break;
                case _CPX_IMMEDIATE:
                    opcodeName = "CPX Imm";
                    break;
                case _CPX_ZERO_PAGE:
                    opcodeName = "CPX ZP";
                    break;
                case _CPX_ABSOLUTE:
                    opcodeName = "CPX Abs";
                    break;
                case _CPY_IMMEDIATE:
                    opcodeName = "CPY Imm";
                    break;
                case _CPY_ZERO_PAGE:
                    opcodeName = "CPY ZP";
                    break;
                case _CPY_ABSOLUTE:
                    opcodeName = "CPY Abs";
                    break;
                case _DEC_ZERO_PAGE:
                    opcodeName = "DEC ZP";
                    break;
                case _DEC_ZERO_PAGE_X:
                    opcodeName = "DEC ZP,X";
                    break;
                case _DEC_ABSOLUTE:
                    opcodeName = "DEC Abs";
                    break;
                case _DEC_ABSOLUTE_X:
                    opcodeName = "DEC Abs,X";
                    break;
                case _EOR_IMMEDIATE:
                    opcodeName = "EOR Imm";
                    break;
                case _EOR_ZERO_PAGE:
                    opcodeName = "EOR ZP";
                    break;
                case _EOR_ZERO_PAGE_X:
                    opcodeName = "EOR ZP,X";
                    break;
                case _EOR_ABSOLUTE:
                    opcodeName = "EOR Abs";
                    break;
                case _EOR_ABSOLUTE_X:
                    opcodeName = "EOR Abs,X";
                    break;
                case _EOR_ABSOLUTE_Y:
                    opcodeName = "EOR Abs,Y";
                    break;
                case _EOR_INDIRECT_X:
                    opcodeName = "EOR Ind,X";
                    break;
                case _EOR_INDIRECT_Y:
                    opcodeName = "EOR Ind,Y";
                    break;
                case _CLC:
                    opcodeName = "CLC Imp";
                    break;
                case _SEC:
                    opcodeName = "SEC Imp";
                    break;
                case _CLI:
                    opcodeName = "CLI Imp";
                    break;
                case _SEI:
                    opcodeName = "SEI Imp";
                    break;
                case _CLV:
                    opcodeName = "CLV Imp";
                    break;
                case _CLD:
                    opcodeName = "CLD Imp";
                    break;
                case _SED:
                    opcodeName = "SED Imp";
                    break;
                case _INC_ZERO_PAGE:
                    opcodeName = "INC ZP";
                    break;
                case _INC_ZERO_PAGE_X:
                    opcodeName = "INC ZP,X";
                    break;
                case _INC_ABSOLUTE:
                    opcodeName = "INC Abs";
                    break;
                case _INC_ABSOLUTE_X:
                    opcodeName = "INC Abs,X";
                    break;
                case _JMP_ABSOLUTE:
                    opcodeName = "JMP Abs";
                    break;
                case _JMP_INDIRECT:
                    opcodeName = "JMP Ind";
                    break;
                case _JSR_ABSOLUTE:
                    opcodeName = "JSR Abs";
                    break;
                case _LDA_IMMEDIATE:
                    opcodeName = "LDA Imm";
                    break;
                case _LDA_ZERO_PAGE:
                    opcodeName = "LDA ZP";
                    break;
                case _LDA_ZERO_PAGE_X:
                    opcodeName = "LDA ZP,X";
                    break;
                case _LDA_ABSOLUTE:
                    opcodeName = "LDA Abs";
                    break;
                case _LDA_ABSOLUTE_X:
                    opcodeName = "LDA Abs,X";
                    break;
                case _LDA_ABSOLUTE_Y:
                    opcodeName = "LDA Abs,Y";
                    break;
                case _LDA_INDIRECT_X:
                    opcodeName = "LDA Ind,X";
                    break;
                case _LDA_INDIRECT_Y:
                    opcodeName = "LDA Ind,Y";
                    break;
                case _LDX_IMMEDIATE:
                    opcodeName = "LDX Imm";
                    break;
                case _LDX_ZERO_PAGE:
                    opcodeName = "LDX ZP";
                    break;
                case _LDX_ZERO_PAGE_Y:
                    opcodeName = "LDX ZP,Y";
                    break;
                case _LDX_ABSOLUTE:
                    opcodeName = "LDX Abs";
                    break;
                case _LDX_ABSOLUTE_Y:
                    opcodeName = "LDX Abs,Y";
                    break;
                case _LDY_IMMEDIATE:
                    opcodeName = "LDY Imm";
                    break;
                case _LDY_ZERO_PAGE:
                    opcodeName = "LDY ZP";
                    break;
                case _LDY_ZERO_PAGE_X:
                    opcodeName = "LDY ZP,X";
                    break;
                case _LDY_ABSOLUTE:
                    opcodeName = "LDY Abs";
                    break;
                case _LDY_ABSOLUTE_X:
                    opcodeName = "LDY Abs,X";
                    break;
                case _LSR_ACCUMULATOR:
                    opcodeName = "LSR Acc";
                    break;
                case _LSR_ZERO_PAGE:
                    opcodeName = "LSR ZP";
                    break;
                case _LSR_ZERO_PAGE_X:
                    opcodeName = "LSR ZP,X";
                    break;
                case _LSR_ABSOLUTE:
                    opcodeName = "LSR Abs";
                    break;
                case _LSR_ABSOLUTE_X:
                    opcodeName = "LSR Abs,X";
                    break;
                case _NOP:
                    opcodeName = "NOP Imp";
                    break;
                case _ORA_IMMEDIATE:
                    opcodeName = "ORA Imm";
                    break;
                case _ORA_ZERO_PAGE:
                    opcodeName = "ORA ZP";
                    break;
                case _ORA_ZERO_PAGE_X:
                    opcodeName = "ORA ZP,X";
                    break;
                case _ORA_ABSOLUTE:
                    opcodeName = "ORA Abs";
                    break;
                case _ORA_ABSOLUTE_X:
                    opcodeName = "ORA Abs,X";
                    break;
                case _ORA_ABSOLUTE_Y:
                    opcodeName = "ORA Abs,Y";
                    break;
                case _ORA_INDIRECT_X:
                    opcodeName = "ORA Ind,X";
                    break;
                case _ORA_INDIRECT_Y:
                    opcodeName = "ORA Ind,Y";
                    break;
                case _TAX:
                    opcodeName = "TAX Imp";
                    break;
                case _TXA:
                    opcodeName = "TXA Imp";
                    break;
                case _DEX:
                    opcodeName = "DEX Imp";
                    break;
                case _INX:
                    opcodeName = "INX Imp";
                    break;
                case _TAY:
                    opcodeName = "TAY Imp";
                    break;
                case _TYA:
                    opcodeName = "TYA Imp";
                    break;
                case _DEY:
                    opcodeName = "DEY Imp";
                    break;
                case _INY:
                    opcodeName = "INY Imp";
                    break;
                case _ROL_ACCUMULATOR:
                    opcodeName = "ROL Acc";
                    break;
                case _ROL_ZERO_PAGE:
                    opcodeName = "ROL ZP";
                    break;
                case _ROL_ZERO_PAGE_X:
                    opcodeName = "ROL ZP,X";
                    break;
                case _ROL_ABSOLUTE:
                    opcodeName = "ROL Abs";
                    break;
                case _ROL_ABSOLUTE_X:
                    opcodeName = "ROL Abs,X";
                    break;
                case _ROR_ACCUMULATOR:
                    opcodeName = "ROR Acc";
                    break;
                case _ROR_ZERO_PAGE:
                    opcodeName = "ROR ZP";
                    break;
                case _ROR_ZERO_PAGE_X:
                    opcodeName = "ROR ZP,X";
                    break;
                case _ROR_ABSOLUTE:
                    opcodeName = "ROR Abs";
                    break;
                case _ROR_ABSOLUTE_X:
                    opcodeName = "ROR Abs,X";
                    break;
                case _RTI_IMPLIED:
                    opcodeName = "RTI Imp";
                    break;
                case _RTS_IMPLIED:
                    opcodeName = "RTS Imp";
                    break;
                case _SBC_IMMEDIATE:
                    opcodeName = "SBC Imm";
                    break;
                case _SBC_ZERO_PAGE:
                    opcodeName = "SBC ZP";
                    break;
                case _SBC_ZERO_PAGE_X:
                    opcodeName = "SBC ZP,X";
                    break;
                case _SBC_ABSOLUTE:
                    opcodeName = "SBC Abs";
                    break;
                case _SBC_ABSOLUTE_X:
                    opcodeName = "SBC Abs,X";
                    break;
                case _SBC_ABSOLUTE_Y:
                    opcodeName = "SBC Abs,Y";
                    break;
                case _SBC_INDIRECT_X:
                    opcodeName = "SBC Ind,X";
                    break;
                case _SBC_INDIRECT_Y:
                    opcodeName = "SBC Ind,Y";
                    break;
                case _STA_ZERO_PAGE:
                    opcodeName = "STA ZP";
                    break;
                case _STA_ZERO_PAGE_X:
                    opcodeName = "STA ZP,X";
                    break;
                case _STA_ABSOLUTE:
                    opcodeName = "STA Abs";
                    break;
                case _STA_ABSOLUTE_X:
                    opcodeName = "STA Abs,X";
                    break;
                case _STA_ABSOLUTE_Y:
                    opcodeName = "STA Abs,Y";
                    break;
                case _STA_INDIRECT_X:
                    opcodeName = "STA Ind,X";
                    break;
                case _STA_INDIRECT_Y:
                    opcodeName = "STA Ind,Y";
                    break;
                case _TXS:
                    opcodeName = "TXS Imp";
                    break;
                case _TSX:
                    opcodeName = "TSX Imp";
                    break;
                case _PHA:
                    opcodeName = "PHA Imp";
                    break;
                case _PLA:
                    opcodeName = "PLA Imp";
                    break;
                case _PHP:
                    opcodeName = "PHP Imp";
                    break;
                case _PLP:
                    opcodeName = "PLP Imp";
                    break;
                case _STX_ZERO_PAGE:
                    opcodeName = "STX ZP";
                    break;
                case _STX_ZERO_PAGE_Y:
                    opcodeName = "STX ZP,Y";
                    break;
                case _STX_ABSOLUTE:
                    opcodeName = "STX Abs";
                    break;
                case _STY_ZERO_PAGE:
                    opcodeName = "STY ZP";
                    break;
                case _STY_ZERO_PAGE_X:
                    opcodeName = "STY ZP,Y";
                    break;
                case _STY_ABSOLUTE:
                    opcodeName = "STY Abs";
                    break;
                case _AAC_IMMEDIATE:
                case _AAC_IMMEDIATE_ALT:
                    opcodeName = "AAC Imm (ill)";
                    break;
                case _AAX_ZERO_PAGE:
                    opcodeName = "AAX ZP (ill)";
                    break;
                case _AAX_ZERO_PAGE_Y:
                    opcodeName = "AAX ZP,Y (ill)";
                    break;
                case _AAX_INDIRECT_X:
                    opcodeName = "AAX Ind,X (ill)";
                    break;
                case _AAX_ABSOLUTE:
                    opcodeName = "AAX Abs (ill)";
                    break;
                case _ARR_IMMEDIATE:
                    opcodeName = "AAR Imm (ill)";
                    break;
                case _ASR_IMMEDIATE:
                    opcodeName = "ASR Imm (ill)";
                    break;
                case _ATX_IMMEDIATE:
                    opcodeName = "ATX Imm (ill)";
                    break;
                case _AXA_ABSOLUTE_Y:
                    opcodeName = "AXA Abs,Y (ill)";
                    break;
                case _AXA_INDIRECT_Y:
                    opcodeName = "AXA Ind,Y (ill)";
                    break;
                case _AXS_IMMEDIATE:
                    opcodeName = "AXS Imm (ill)";
                    break;
                case _DCP_ZERO_PAGE:
                    opcodeName = "DCP ZP (ill)";
                    break;
                case _DCP_ZERO_PAGE_X:
                    opcodeName = "DCP ZP,X (ill)";
                    break;
                case _DCP_ABSOLUTE:
                    opcodeName = "DCP Abs (ill)";
                    break;
                case _DCP_ABSOLUTE_X:
                    opcodeName = "DCP Abs,X (ill)";
                    break;
                case _DCP_ABSOLUTE_Y:
                    opcodeName = "DCP Abs,Y (ill)";
                    break;
                case _DCP_INDIRECT_X:
                    opcodeName = "DCP Ind,X (ill)";
                    break;
                case _DCP_INDIRECT_Y:
                    opcodeName = "DCP Ind,Y (ill)";
                    break;
                case _DOP_ZERO_PAGE:
                case _DOP_ZERO_PAGE_ALT:
                case _DOP_ZERO_PAGE_ALT_2:
                    opcodeName = "DOP ZP (ill)";
                    break;
                case _DOP_ZERO_PAGE_X:
                case _DOP_ZERO_PAGE_X_ALT:
                case _DOP_ZERO_PAGE_X_ALT_2:
                case _DOP_ZERO_PAGE_X_ALT_3:
                case _DOP_ZERO_PAGE_X_ALT_4:
                case _DOP_ZERO_PAGE_X_ALT_5:
                    opcodeName = "DOP ZP,X (ill)";
                    break;
                case _DOP_IMMEDIATE:
                case _DOP_IMMEDIATE_ALT:
                case _DOP_IMMEDIATE_ALT_2:
                case _DOP_IMMEDIATE_ALT_3:
                case _DOP_IMMEDIATE_ALT_4:
                    opcodeName = "DOP Imm (ill)";
                    break;
                case _ISC_ZERO_PAGE:
                    opcodeName = "ISC ZP (ill)";
                    break;
                case _ISC_ZERO_PAGE_X:
                    opcodeName = "ISC ZP,X (ill)";
                    break;
                case _ISC_ABSOLUTE:
                    opcodeName = "ISC Abs (ill)";
                    break;
                case _ISC_ABSOLUTE_X:
                    opcodeName = "ISC Abs,X (ill)";
                    break;
                case _ISC_ABSOLUTE_Y:
                    opcodeName = "ISC Abs,Y (ill)";
                    break;
                case _ISC_INDIRECT_X:
                    opcodeName = "ISC Ind,X (ill)";
                    break;
                case _ISC_INDIRECT_Y:
                    opcodeName = "ISC Ind,Y (ill)";
                    break;
                case _KIL_IMPLIED:
                case _KIL_IMPLIED_ALT:
                case _KIL_IMPLIED_ALT_2:
                case _KIL_IMPLIED_ALT_3:
                case _KIL_IMPLIED_ALT_4:
                case _KIL_IMPLIED_ALT_5:
                case _KIL_IMPLIED_ALT_6:
                case _KIL_IMPLIED_ALT_7:
                case _KIL_IMPLIED_ALT_8:
                case _KIL_IMPLIED_ALT_9:
                case _KIL_IMPLIED_ALT_10:
                case _KIL_IMPLIED_ALT_11:
                    opcodeName = "KIL Imp (ill)";
                    break;
                case _LAR_ABSOLUTE_Y:
                    opcodeName = "LAR Abs,Y (ill)";
                    break;
                case _LAX_ZERO_PAGE:
                    opcodeName = "LAX ZP (ill)";
                    break;
                case _LAX_ZERO_PAGE_Y:
                    opcodeName = "LAX ZP,X (ill)";
                    break;
                case _LAX_ABSOLUTE:
                    opcodeName = "LAX Abs (ill)";
                    break;
                case _LAX_ABSOLUTE_Y:
                    opcodeName = "LAX Abs,Y (ill)";
                    break;
                case _LAX_INDIRECT_X:
                    opcodeName = "LAX Ind,X (ill)";
                    break;
                case _LAX_INDIRECT_Y:
                    opcodeName = "LAX Ind,Y (ill)";
                    break;
                case _NOP_IMPLIED:
                case _NOP_IMPLIED_ALT:
                case _NOP_IMPLIED_ALT_2:
                case _NOP_IMPLIED_ALT_3:
                case _NOP_IMPLIED_ALT_4:
                case _NOP_IMPLIED_ALT_5:
                    opcodeName = "NOP Imp (ill)";
                    break;
                case _RLA_ZERO_PAGE:
                    opcodeName = "RLA ZP (ill)";
                    break;
                case _RLA_ZERO_PAGE_X:
                    opcodeName = "RLA ZP,X (ill)";
                    break;
                case _RLA_ABSOLUTE:
                    opcodeName = "RLA Abs (ill)";
                    break;
                case _RLA_ABSOLUTE_X:
                    opcodeName = "RLA Abs,X (ill)";
                    break;
                case _RLA_ABSOLUTE_Y:
                    opcodeName = "RLA Abs,Y (ill)";
                    break;
                case _RLA_INDIRECT_X:
                    opcodeName = "RLA Ind,X (ill)";
                    break;
                case _RLA_INDIRECT_Y:
                    opcodeName = "RLA Ind,Y (ill)";
                    break;
                case _RRA_ZERO_PAGE:
                    opcodeName = "RRA ZP (ill)";
                    break;
                case _RRA_ZERO_PAGE_X:
                    opcodeName = "RRA ZP,X (ill)";
                    break;
                case _RRA_ABSOLUTE:
                    opcodeName = "RRA Abs (ill)";
                    break;
                case _RRA_ABSOLUTE_X:
                    opcodeName = "RRA Abs,X (ill)";
                    break;
                case _RRA_ABSOLUTE_Y:
                    opcodeName = "RRA Abs,Y (ill)";
                    break;
                case _RRA_INDIRECT_X:
                    opcodeName = "RRA Ind,X (ill)";
                    break;
                case _RRA_INDIRECT_Y:
                    opcodeName = "RRA Ind,Y (ill)";
                    break;
                case _SBC_IMMEDIATE_ALT:
                    opcodeName = "SBC Imm (ill)";
                    break;
                case _SLO_ZERO_PAGE:
                    opcodeName = "SLO ZP (ill)";
                    break;
                case _SLO_ZERO_PAGE_X:
                    opcodeName = "SLO ZP,X (ill)";
                    break;
                case _SLO_ABSOLUTE:
                    opcodeName = "SLO Abs (ill)";
                    break;
                case _SLO_ABSOLUTE_X:
                    opcodeName = "SLO Abs,X (ill)";
                    break;
                case _SLO_ABSOLUTE_Y:
                    opcodeName = "SLO Abs,Y (ill)";
                    break;
                case _SLO_INDIRECT_X:
                    opcodeName = "SLO Ind,X (ill)";
                    break;
                case _SLO_INDIRECT_Y:
                    opcodeName = "SLO Ind,Y (ill)";
                    break;
                case _SRE_ZERO_PAGE:
                    opcodeName = "SRE ZP (ill)";
                    break;
                case _SRE_ZERO_PAGE_X:
                    opcodeName = "SRE ZP,X (ill)";
                    break;
                case _SRE_ABSOLUTE:
                    opcodeName = "SRE Abs (ill)";
                    break;
                case _SRE_ABSOLUTE_X:
                    opcodeName = "SRE Abs,X (ill)";
                    break;
                case _SRE_ABSOLUTE_Y:
                    opcodeName = "SRE Abs,Y (ill)";
                    break;
                case _SRE_INDIRECT_X:
                    opcodeName = "SRE Ind,X (ill)";
                    break;
                case _SRE_INDIRECT_Y:
                    opcodeName = "SRE Ind,Y (ill)";
                    break;
                case _SXA_ABSOLUTE_Y:
                    opcodeName = "SXA Abs,Y (ill)";
                    break;
                case _SYA_ABSOLUTE_X:
                    opcodeName = "SYA Abs,X (ill)";
                    break;
                case _TOP_ABSOLUTE:
                case _TOP_ABSOLUTE_X:
                case _TOP_ABSOLUTE_X_ALT:
                case _TOP_ABSOLUTE_X_ALT_2:
                case _TOP_ABSOLUTE_X_ALT_3:
                case _TOP_ABSOLUTE_X_ALT_4:
                case _TOP_ABSOLUTE_X_ALT_5:
                    opcodeName = "TOP ABS (ill)";
                    break;
                case _XAA_IMMEDIATE:
                    opcodeName = "XAA Imm (ill)";
                    break;
                case _XAS_ABSOLUTE_Y:
                    opcodeName = "XAS ABS,Y (ill)";
                    break;
                default:
                    opcodeName = "???";
                    break;
            }

            return opcodeName;
        }
    }
}
 
 