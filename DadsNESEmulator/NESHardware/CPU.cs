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

using System;
using System.Collections;
using System.Text;

namespace DadsNESEmulator.NESHardware
{
    /** @brief  Class that defines the NMOS6502 CPU. */
    public class CPU
    {
        /** - https://wiki.nesdev.com/w/index.php/CPU_interrupts */

        /** - @brief NMI vector address */
        public const ushort _NMI_VECTOR_ADDRESS = 0xFFFA;

        /** - @brief Reset vector address (program counter start address here) */
        public const ushort _RESET_VECTOR_ADDRESS = 0xFFFC;

        /** - @brief IRQ and BRK vector address */
        public const ushort IRQ_BRK_VECTOR_ADDRESS = 0xFFFE;

        /** - @brief Address of bottom of the stack */
        public const ushort STACK_ADDRESS = 0x0100;

        #region Properties

        /** - https://wiki.nesdev.com/w/index.php/CPU_registers */

        /** - @brief Accumulator register - A is byte-wide and along with the arithmetic logic unit (ALU), supports using the status register for carrying, overflow detection, and so on. */
        public byte A
        {
            get;
            protected set;
        }

        /** - @brief X index register is byte-wide and used for several addressing modes. It can be used as a loop counter easily, using INC/DEC and branch instructions. Not being the accumulator,
         * it has limited addressing modes when loading and saving.
         */
        public byte X
        {
            get;
            protected set;
        }

        /** - @brief Y index register is byte-wide and used for several addressing modes. It can be used as a loop counter easily, using INC/DEC and branch instructions. Not being the accumulator,
         * it has limited addressing modes when loading and saving.
         */
        public byte Y
        {
            get;
            protected set;
        }

        /** - @brief Program Counter register - The 2-byte program counter PC supports 65536 direct (unbanked) memory locations, however not all values are sent to the cartridge. It can be
         *    accessed either by allowing CPU's internal fetch logic increment the address bus, an interrupt (NMI, Reset, IRQ/BRQ), and using the RTS/JMP/JSR/Branch instructions.
         */
        public ushort PC
        {
            get;
            protected set;
        }

        /** - @brief Stack Pointer register - S is byte-wide and can be accessed using interrupts, pulls, pushes, and transfers. */
        public byte S
        {
            get;
            protected set;
        }

        /** - @brief Status Register (Processor flag) - P has 6 bits used by the ALU but is byte-wide. PHP, PLP, arithmetic, testing, and branch instructions can access this register. */
        //public byte P
        //{
        //    get;
        //    protected set;
        //}

        /** - @brief Status Register (Processor flag) - P has 6 bits used by the ALU but is byte-wide. PHP, PLP, arithmetic, testing, and branch instructions can access this register. */
        public BitArray P
        {
            get;
            protected set;
        }

        public Memory Mem
        {
            get;
            protected set;
        }

        public byte CPUCycles
        {
            get;
            protected set;
        }

        public uint ClockCount
        {
            get;
            protected set;
        }

        public byte Opcode
        {
            get;
            protected set;
        }

        public ushort AbsoluteAddress
        {
            get;
            protected set;
        }
        public ushort RelativeAddress
        {
            get;
            protected set;
        }

        #endregion

        #region Class methods

        public CPU()
        {

        }

        public void Power(Memory mem)
        {
            /** - https://wiki.nesdev.com/w/index.php/CPU_power_up_state */

            /**
             * P = $34 (IRQ disabled)
             * A, X, Y = 0
             * S = $FD
             * $4017 = $00 (frame irq enabled)
             * $4015 = $00 (all channels disabled)
             * $4000-$400F = $00
             * $4010-$4013 = $00
             * All 15 bits of noise channel LFSR = $0000. The first time the LFSR is clocked from the all-0s state, it will shift in a 1.
             * 2A03G: APU Frame Counter reset. (but 2A03letterless: APU frame counter powers up at a value equivalent to 15)
             * Internal memory ($0000-$07FF) has unreliable startup state. Some machines may have consistent RAM contents at power-on, but others do not.
             * Emulators often implement a consistent RAM startup state (e.g. all $00 or $FF, or a particular pattern), and flash carts like the PowerPak may
             * partially or fully initialize RAM before starting a program, so an NES programmer must be careful not to rely on the startup contents of RAM.
             */
            A = 0x00;
            X = 0x00;
            Y = 0x00;
            //P = 0x34;
            P = new BitArray(new byte[] {0x34});
            S = 0xFD;

            //Mem = new Memory(0xFFFF);
            Mem = mem;

            /* Set PC from 16-bit address 0xFFFC-0xFFFD */
            //PC = Mem.ReadShort(0xFFFC); //_RESET_VECTOR_ADDRESS
            ushort lo = Mem.ReadByte(_RESET_VECTOR_ADDRESS);
            ushort hi = Mem.ReadByte((ushort)(_RESET_VECTOR_ADDRESS + 1));
            PC = (ushort)((hi << 8) | lo);
            //Log.Info($"Cartridge starts at {pc:X4}");

            /* - Nes Test - automated mode (for testing with no video/audio implemented)*/
            PC = Mem.ReadByte(0xC000);

            /* - Nes Test - non-automated mode */
            //PC = Mem.ReadByte(0xC004);

            CPUCycles = 0;
            ClockCount = 0;
            AbsoluteAddress = 0x0000;
            RelativeAddress = 0x0000;
        }

        public void Reset()
        {

            /**
             * A, X, Y were not affected
             * S was decremented by 3 (but nothing was written to the stack)
             * The I (IRQ disable) flag was set to true (status ORed with $04)
             * The internal memory was unchanged
             * APU mode in $4017 was unchanged
             * APU was silenced ($4015 = 0)
             * APU triangle phase is reset to 0 (i.e. outputs a value of 15, the first step of its waveform)
             * APU DPCM output ANDed with 1 (upper 6 bits cleared)
             * 2A03G: APU Frame Counter reset. (but 2A03letterless: APU frame counter retains old value)
             */

            /* Set PC from 16-bit address 0xFFFC-0xFFFD */
            //PC = Mem.ReadShort(0xFFFC);

            /* - Nes Test - automated mode (for testing with no video/audio implemented)*/
            PC = Mem.ReadByte(0xC000);

            /* - Nes Test - non-automated mode */
            //PC = Mem.ReadByte(0xC004);

            CPUCycles = 0;
            ClockCount = 0;
            AbsoluteAddress = 0x0000;
            RelativeAddress = 0x0000;
        }

        public void Step()
        {
            /** Set global for debugging or other uses for now. */
            Opcode = ReadNextByte();

            switch (Opcode)
            {
                case Opcodes._ADC_IMMEDIATE:
                    Immediate();
                    ADC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._ADC_ZERO_PAGE:
                    ZeroPage();
                    ADC();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._ADC_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    ADC();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._ADC_ABSOLUTE:
                    Absolute();
                    ADC();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ADC_ABSOULTE_X:
                    AbsoluteXIndex();
                    ADC();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ADC_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    ADC();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ADC_INDIRECT_X:
                    IndirectXIndex();
                    ADC();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._ADC_INDIRECT_Y:
                    IndirectYIndex();
                    ADC();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._AND_IMMEDIATE:
                    Immediate();
                    AND();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._AND_ZERO_PAGE:
                    ZeroPage();
                    AND();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._AND_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    AND();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._AND_ABSOLUTE:
                    Absolute();
                    AND();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._AND_ABSOULTE_X:
                    AbsoluteXIndex();
                    AND();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._AND_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    AND();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._AND_INDIRECT_X:
                    IndirectXIndex();
                    AND();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._AND_INDIRECT_Y:
                    IndirectYIndex();
                    AND();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._ASL_ACCUMULATOR:
                    Accumulator();
                    ASL();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._ASL_ZERO_PAGE:
                    ZeroPage();
                    ASL();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._ASL_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    ASL();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._ASL_ABSOLUTE:
                    Absolute();
                    ASL();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._ASL_ABSOLUTE_X:
                    AbsoluteXIndex();
                    ASL();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._BIT_ZERO_PAGE:
                    ZeroPage();
                    BIT();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._BIT_ABSOLUTE:
                    Absolute();
                    BIT();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._BPL:
                    BPL();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BMI:
                    BMI();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BVC:
                    BVC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BVS:
                    BVS();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BCC:
                    BCC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BCS:
                    BCS();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BNE:
                    BNE();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BEQ:
                    BEQ();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._BRK:
                    BRK();
                    PC += 2;
                    CPUCycles += 7;
                    break;
                case Opcodes._CMP_IMMEDIATE:
                    Immediate();
                    CMP();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._CMP_ZERO_PAGE:
                    ZeroPage();
                    CMP();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._CMP_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    CMP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._CMP_ABSOLUTE:
                    Absolute();
                    CMP();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._CMP_ABSOULTE_X:
                    AbsoluteXIndex();
                    CMP();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._CMP_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    CMP();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._CMP_INDIRECT_X:
                    IndirectXIndex();
                    CMP();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._CMP_INDIRECT_Y:
                    IndirectYIndex();
                    CMP();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._CPX_IMMEDIATE:
                    Immediate();
                    CPX();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._CPX_ZERO_PAGE:
                    ZeroPage();
                    CPX();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._CPX_ABSOLUTE:
                    Absolute();
                    CPX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._CPY_IMMEDIATE:
                    Immediate();
                    CPY();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._CPY_ZERO_PAGE:
                    ZeroPage();
                    CPY();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._CPY_ABSOLUTE:
                    Absolute();
                    CPY();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._DEC_ZERO_PAGE:
                    ZeroPage();
                    DEC();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._DEC_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    DEC();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._DEC_ABSOLUTE:
                    Absolute();
                    DEC();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._DEC_ABSOULTE_X:
                    AbsoluteXIndex();
                    DEC();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._EOR_IMMEDIATE:
                    Immediate();
                    EOR();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._EOR_ZERO_PAGE:
                    ZeroPage();
                    EOR();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._EOR_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    EOR();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._EOR_ABSOLUTE:
                    Absolute();
                    EOR();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._EOR_ABSOULTE_X:
                    AbsoluteXIndex();
                    EOR();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._EOR_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    EOR();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._EOR_INDIRECT_X:
                    IndirectXIndex();
                    EOR();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._EOR_INDIRECT_Y:
                    IndirectYIndex();
                    EOR();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._CLC:
                    CLC();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._SEC:
                    SEC();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._CLI:
                    CLI();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._SEI:
                    SEI();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._CLV:
                    CLV();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._CLD:
                    CLD();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._SED:
                    SED();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._INC_ZERO_PAGE:
                    ZeroPage();
                    INC();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._INC_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    INC();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._INC_ABSOLUTE:
                    Absolute();
                    INC();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._INC_ABSOULTE_X:
                    AbsoluteXIndex();
                    INC();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._JMP_ABSOLUTE:
                    Absolute();
                    JMP();
                    PC += 3;
                    CPUCycles += 3;
                    break;
                case Opcodes._JMP_INDIRECT:
                    Indirect();
                    JMP();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                case Opcodes._JSR_ABSOLUTE:
                    Absolute();
                    JSR();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._LDA_IMMEDIATE:
                    Immediate();
                    LDA();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._LDA_ZERO_PAGE:
                    ZeroPage();
                    LDA();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._LDA_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    LDA();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDA_ABSOLUTE:
                    Absolute();
                    LDA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDA_ABSOLUTE_X:
                    AbsoluteXIndex();
                    LDA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDA_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    LDA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDA_INDIRECT_X:
                    IndirectXIndex();
                    LDA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._LDA_INDIRECT_Y:
                    IndirectYIndex();
                    LDA();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._LDX_IMMEDIATE:
                    Immediate();
                    LDX();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._LDX_ZERO_PAGE:
                    ZeroPage();
                    LDX();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._LDX_ZERO_PAGE_Y:
                    ZeroPageYIndex();
                    LDX();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDX_ABSOLUTE:
                    Absolute();
                    LDX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDX_ABSOULTE_Y:
                    AbsoluteYIndex();
                    LDX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDY_IMMEDIATE:
                    Immediate();
                    LDY();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._LDY_ZERO_PAGE:
                    ZeroPage();
                    LDY();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._LDY_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    LDY();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDY_ABSOLUTE:
                    Absolute();
                    LDY();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LDY_ABSOLUTE_X:
                    AbsoluteXIndex();
                    LDY();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LSR_ACCUMULATOR:
                    Accumulator();
                    LSR();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._LSR_ZERO_PAGE:
                    ZeroPage();
                    LSR();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._LSR_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    LSR();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._LSR_ABSOLUTE:
                    Absolute();
                    LSR();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._LSR_ABSOLUTE_X:
                    AbsoluteXIndex();
                    LSR();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._NOP:
                    NOP();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._ORA_IMMEDIATE:
                    Immediate();
                    ORA();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._ORA_ZERO_PAGE:
                    ZeroPage();
                    ORA();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._ORA_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    ORA();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._ORA_ABSOLUTE:
                    Absolute();
                    ORA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ORA_ABSOULTE_X:
                    AbsoluteXIndex();
                    ORA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ORA_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    ORA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ORA_INDIRECT_X:
                    IndirectXIndex();
                    ORA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._ORA_INDIRECT_Y:
                    IndirectYIndex();
                    ORA();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._TAX:
                    TAX();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._TXA:
                    TXA();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._DEX:
                    DEX();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._INX:
                    INX();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._TAY:
                    TAY();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._TYA:
                    TYA();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._DEY:
                    DEY();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._INY:
                    INY();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._ROL_ACCUMULATOR:
                    Accumulator();
                    ROL();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._ROL_ZERO_PAGE:
                    ZeroPage();
                    ROL();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._ROL_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    ROL();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._ROL_ABSOLUTE:
                    Absolute();
                    ROL();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._ROL_ABSOLUTE_X:
                    AbsoluteXIndex();
                    ROL();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._ROR_ACCUMULATOR:
                    Accumulator();
                    ROR();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._ROR_ZERO_PAGE:
                    ZeroPage();
                    ROR();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._ROR_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    ROR();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._ROR_ABSOLUTE:
                    Absolute();
                    ROR();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._ROR_ABSOLUTE_X:
                    AbsoluteXIndex();
                    ROR();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._RTI_IMPLIED:
                    Implied();
                    RTI();
                    PC += 1;
                    CPUCycles += 6;
                    break;
                case Opcodes._RTS_IMPLIED:
                    Implied();
                    RTS();
                    PC += 1;
                    CPUCycles += 6;
                    break;
                case Opcodes._SBC_IMMEDIATE:
                    Immediate();
                    SBC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._SBC_ZERO_PAGE:
                    ZeroPage();
                    SBC();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._SBC_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    SBC();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._SBC_ABSOLUTE:
                    Absolute();
                    SBC();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._SBC_ABSOLUTE_X:
                    AbsoluteXIndex();
                    SBC();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._SBC_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    SBC();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._SBC_INDIRECT_X:
                    IndirectXIndex();
                    SBC();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._SBC_INDIRECT_Y:
                    IndirectYIndex();
                    SBC();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._STA_ZERO_PAGE:
                    ZeroPage();
                    STA();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._STA_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    STA();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._STA_ABSOLUTE:
                    Absolute();
                    STA();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._STA_ABSOLUTE_X:
                    AbsoluteXIndex();
                    STA();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                case Opcodes._STA_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    STA();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                case Opcodes._STA_INDIRECT_X:
                    IndirectXIndex();
                    STA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._STA_INDIRECT_Y:
                    IndirectYIndex();
                    STA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._TXS:
                    TXS();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._TSX:
                    TSX();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._PHA:
                    PHA();
                    PC += 1;
                    CPUCycles += 3;
                    break;
                case Opcodes._PLA:
                    PLA();
                    PC += 1;
                    CPUCycles += 4;
                    break;
                case Opcodes._PHP:
                    PHP();
                    PC += 1;
                    CPUCycles += 3;
                    break;
                case Opcodes._PLP:
                    PLP();
                    PC += 1;
                    CPUCycles += 4;
                    break;
                case Opcodes._STX_ZERO_PAGE:
                    ZeroPage();
                    STX();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._STX_ZERO_PAGE_Y:
                    ZeroPageYIndex();
                    STX();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._STX_ABSOLUTE:
                    Absolute();
                    STX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._STY_ZERO_PAGE:
                    ZeroPage();
                    STY();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._STY_ZERO_PAGE_Y:
                    ZeroPageYIndex();
                    STY();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._STY_ABSOLUTE:
                    Absolute();
                    STY();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                /** - Unofficial / Illegal Opcodes */
                case Opcodes._AAC_IMMEDIATE:
                    Immediate();
                    AAC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._AAC_IMMEDIATE_ALT:
                    Immediate();
                    AAC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._AAX_ZERO_PAGE:
                    ZeroPage();
                    AAX();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._AAX_ZERO_PAGE_Y:
                    ZeroPageYIndex();
                    AAX();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._AAX_INDIRECT_X:
                    IndirectXIndex();
                    AAX();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._AAX_ABSOLUTE:
                    Absolute();
                    AAX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._ARR_IMMEDIATE:
                    Immediate();
                    ARR();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._ASR_IMMEDIATE:
                    Immediate();
                    ASR();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._ATX_IMMEDIATE:
                    Immediate();
                    ATX();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._AXA_ABSOLUTE_Y:
                    Absolute();
                    AXA();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                case Opcodes._AXA_INDIRECT_Y:
                    Indirect();
                    AXA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._AXS_IMMEDIATE:
                    Immediate();
                    AXS();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._DCP_ZERO_PAGE:
                    ZeroPage();
                    DCP();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._DCP_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    DCP();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._DCP_ABSOLUTE:
                    Absolute();
                    DCP();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._DCP_ABSOLUTE_X:
                    AbsoluteXIndex();
                    DCP();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._DCP_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    DCP();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._DCP_INDIRECT_X:
                    IndirectXIndex();
                    DCP();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._DCP_INDIRECT_Y:
                    IndirectYIndex();
                    DCP();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._DOP_ZERO_PAGE:
                    ZeroPage();
                    DOP();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._DOP_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    DOP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT:
                    ZeroPageXIndex();
                    DOP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._DOP_ZERO_PAGE_ALT:
                    ZeroPage();
                    DOP();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_2:
                    ZeroPageXIndex();
                    DOP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._DOP_ZERO_PAGE_ALT_2:
                    ZeroPage();
                    DOP();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_3:
                    ZeroPageXIndex();
                    DOP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._DOP_IMMEDIATE:
                    Immediate();
                    DOP();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT:
                    Immediate();
                    DOP();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT_2:
                    Immediate();
                    DOP();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT_3:
                    Immediate();
                    DOP();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_4:
                    ZeroPageXIndex();
                    DOP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT_4:
                    Immediate();
                    DOP();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_5:
                    ZeroPageXIndex();
                    DOP();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._ISC_ZERO_PAGE:
                    ZeroPage();
                    ISC();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._ISC_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    ISC();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._ISC_ABSOLUTE:
                    Absolute();
                    ISC();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._ISC_ABSOLUTE_X:
                    AbsoluteXIndex();
                    ISC();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._ISC_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    ISC();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._ISC_INDIRECT_X:
                    IndirectXIndex();
                    ISC();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._ISC_INDIRECT_Y:
                    IndirectYIndex();
                    ISC();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._KIL_IMPLIED:
                case Opcodes._KIL_IMPLIED_ALT:
                case Opcodes._KIL_IMPLIED_ALT_2:
                case Opcodes._KIL_IMPLIED_ALT_3:
                case Opcodes._KIL_IMPLIED_ALT_4:
                case Opcodes._KIL_IMPLIED_ALT_5:
                case Opcodes._KIL_IMPLIED_ALT_6:
                case Opcodes._KIL_IMPLIED_ALT_7:
                case Opcodes._KIL_IMPLIED_ALT_8:
                case Opcodes._KIL_IMPLIED_ALT_9:
                case Opcodes._KIL_IMPLIED_ALT_10:
                case Opcodes._KIL_IMPLIED_ALT_11:
                    Implied();
                    KIL();
                    PC += 1;
                    break;
                case Opcodes._LAR_ABSOLUTE_Y:
                    Absolute();
                    LAR();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LAX_ZERO_PAGE:
                    ZeroPage();
                    LAX();
                    PC += 2;
                    CPUCycles += 3;
                    break;
                case Opcodes._LAX_ZERO_PAGE_Y:
                    ZeroPageYIndex();
                    LAX();
                    PC += 2;
                    CPUCycles += 4;
                    break;
                case Opcodes._LAX_ABSOLUTE:
                    Absolute();
                    LAX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LAX_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    LAX();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._LAX_INDIRECT_X:
                    IndirectXIndex();
                    LAX();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._LAX_INDIRECT_Y:
                    IndirectYIndex();
                    LAX();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._NOP_IMPLIED:
                case Opcodes._NOP_IMPLIED_ALT:
                case Opcodes._NOP_IMPLIED_ALT_2:
                case Opcodes._NOP_IMPLIED_ALT_3:
                case Opcodes._NOP_IMPLIED_ALT_4:
                case Opcodes._NOP_IMPLIED_ALT_5:
                    Implied();
                    NOP();
                    PC += 1;
                    CPUCycles += 2;
                    break;
                case Opcodes._RLA_ZERO_PAGE:
                    ZeroPage();
                    RLA();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._RLA_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    RLA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._RLA_ABSOLUTE:
                    Absolute();
                    RLA();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._RLA_ABSOLUTE_X:
                    AbsoluteXIndex();
                    RLA();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._RLA_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    RLA();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._RLA_INDIRECT_X:
                    IndirectXIndex();
                    RLA();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._RLA_INDIRECT_Y:
                    IndirectYIndex();
                    RLA();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._RRA_ZERO_PAGE:
                    ZeroPage();
                    RRA();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._RRA_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    RRA();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._RRA_ABSOLUTE:
                    Absolute();
                    RRA();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._RRA_ABSOLUTE_X:
                    AbsoluteXIndex();
                    RRA();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._RRA_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    RRA();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._RRA_INDIRECT_X:
                    IndirectXIndex();
                    RRA();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._RRA_INDIRECT_Y:
                    IndirectYIndex();
                    RRA();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._SBC_IMMEDIATE_ALT:
                    Immediate();
                    SBC();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._SLO_ZERO_PAGE:
                    ZeroPage();
                    SLO();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._SLO_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    SLO();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._SLO_ABSOLUTE:
                    Absolute();
                    SLO();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._SLO_ABSOLUTE_X:
                    AbsoluteXIndex();
                    SLO();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._SLO_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    SLO();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._SLO_INDIRECT_X:
                    IndirectXIndex();
                    SLO();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._SLO_INDIRECT_Y:
                    IndirectYIndex();
                    SLO();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._SRE_ZERO_PAGE:
                    ZeroPage();
                    SRE();
                    PC += 2;
                    CPUCycles += 5;
                    break;
                case Opcodes._SRE_ZERO_PAGE_X:
                    ZeroPageXIndex();
                    SRE();
                    PC += 2;
                    CPUCycles += 6;
                    break;
                case Opcodes._SRE_ABSOLUTE:
                    Absolute();
                    SRE();
                    PC += 3;
                    CPUCycles += 6;
                    break;
                case Opcodes._SRE_ABSOLUTE_X:
                    AbsoluteXIndex();
                    SRE();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._SRE_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    SRE();
                    PC += 3;
                    CPUCycles += 7;
                    break;
                case Opcodes._SRE_INDIRECT_X:
                    IndirectXIndex();
                    SRE();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._SRE_INDIRECT_Y:
                    IndirectYIndex();
                    SRE();
                    PC += 2;
                    CPUCycles += 8;
                    break;
                case Opcodes._SXA_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    SXA();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                case Opcodes._SYA_ABSOLUTE_X:
                    AbsoluteXIndex();
                    SYA();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                case Opcodes._TOP_ABSOLUTE:
                    Absolute();
                    TOP();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._TOP_ABSOLUTE_X:
                case Opcodes._TOP_ABSOLUTE_X_ALT:
                case Opcodes._TOP_ABSOLUTE_X_ALT_2:
                case Opcodes._TOP_ABSOLUTE_X_ALT_3:
                case Opcodes._TOP_ABSOLUTE_X_ALT_4:
                case Opcodes._TOP_ABSOLUTE_X_ALT_5:
                    Absolute();
                    TOP();
                    PC += 3;
                    CPUCycles += 4;
                    break;
                case Opcodes._XAA_IMMEDIATE:
                    Immediate();
                    XAA();
                    PC += 2;
                    CPUCycles += 2;
                    break;
                case Opcodes._XAS_ABSOLUTE_Y:
                    AbsoluteYIndex();
                    XAS();
                    PC += 3;
                    CPUCycles += 5;
                    break;
                default:
                    /** - Should not happen. */
                    break;
            }
        }

        private byte ReadNextByte()
        {
            byte opCode = Mem.ReadByte(PC);
            //PC++;

            return opCode;
        }

        #endregion

        #region Class Methods (Instructions)

        private void ADC()
        {
            
        }

        /**
         * @brief   AND (And Memory With Accumulator) performs a logical AND on the operand and the accumulator and stores the result in the accumulator. 
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void AND()
        {
            byte operand = Mem.ReadByte((ushort)(PC + 1));

            /** - AND values from the operand and the accumulator together */
            operand &= A;

            SetZNStatusRegisterProcessorFlags(operand);

            /** - Store the Operand in the Accumulator Register. */
            A = operand;
        }

        private void ASL()
        {

        }

        private void BIT()
        {

        }

        private void BPL()
        {

        }

        private void BMI()
        {

        }

        private void BVC()
        {

        }

        private void BVS()
        {

        }

        private void BCC()
        {

        }

        private void BCS()
        {

        }

        private void BNE()
        {

        }

        private void BEQ()
        {

        }

        private void BRK()
        {
            // @todo: finish this method
            // pushes the Program Counter Register and processor status register to the stack,
            // sets the Interrupt Flag to temporarily prevent other IRQs from being executed
            P[2] = true;

            // reload the Program Counter from the vector at $FFFE-$FFFF
            //PC = (ushort)(Mem.ReadByte(0xFFFE) | (Mem.ReadByte(0xFFFF) << 8));
            PC = Mem.ReadShort(IRQ_BRK_VECTOR_ADDRESS);
        }

        private void CMP()
        {

        }

        private void CPX()
        {

        }

        private void CPY()
        {

        }

        private void DEC()
        {

        }

        private void EOR()
        {
            byte operand = Mem.ReadByte((ushort)(PC + 1));

            /** - XOR values from the operand and the accumulator together */
            operand ^= A;

            SetZNStatusRegisterProcessorFlags(operand);

            /** - Store the Operand in the Accumulator Register. */
            A = operand;
        }

        private void CLC()
        {
            // CLC (Clear Carry Flag) clears the Carry Flag in the Processor Status Register by setting the 0th bit 0
            P[0] = false;
        }
        private void SEC()
        {
            // SEC (Set Carry Flag) sets the Carry Flag in the Processor Status Register by setting the 0th bit 1.
            P[0] = true;
        }

        private void CLI()
        {
            // CLI (Clear Interrupt Disable Flag) clears the Interrupt Flag in the Processor Status Register by setting the 2nd bit 0.
            P[2] = false;
        }

        private void SEI()
        {
            // SEI (Set Interrupt Disable Flag) sets the Interrupt Flag in the Processor Status Register by setting the 2nd bit 1.
            P[2] = true;
        }

        private void CLV()
        {
            // CLV (Clear Overflow Flag) clears the Overflow Flag in the Processor Status Register by setting the 6th bit 0.
            P[6] = false;
        }

        private void CLD()
        {
            // CLD (Clear Decimal Flag) clears the Decimal Flag in the Processor Status Register by setting the 3rd bit 0.
            P[3] = false;
        }

        private void SED()
        {
            // SED (Set Decimal Flag) set the Decimal Flag in the Processor Status Register by setting the 3rd bit 1
            P[3] = true;
        }

        private void INC()
        {
            // @todo: this method id probably incorrect
            ushort value = Mem.ReadShort((ushort)(PC + 1));
            byte operand = Mem.ReadByte(value);
            operand++;
            Mem.WriteByte(value, operand);

            SetZNStatusRegisterProcessorFlags(operand);

        }

        private void JMP()
        {

        }

        private void JSR()
        {

        }

        private void LDA()
        {
            byte operand = Mem.ReadByte((ushort)(PC + 1));
            
            SetZNStatusRegisterProcessorFlags(operand);

            /** - Stores the Operand in the Accumulator Register. */
            A = operand;
        }

        private void LDX()
        {
            byte operand = Mem.ReadByte((ushort)(PC + 1));

            SetZNStatusRegisterProcessorFlags(operand);

            /** - Stores the Operand in the X Index Register. */
            X = operand;
        }

        private void LDY()
        {
            byte operand = Mem.ReadByte((ushort)(PC + 1));

            SetZNStatusRegisterProcessorFlags(operand);

            /** - Stores the Operand in the Y Index Register. */
            Y = operand;
        }

        private void LSR()
        {

        }

        private void NOP()
        {
            /** - No OPeration */
        }

        private void ORA()
        {
            byte operand = Mem.ReadByte((ushort)(PC + 1));

            /** - OR values from the operand and the accumulator together */
            operand |= A;

            SetZNStatusRegisterProcessorFlags(operand);

            /** - Store the Operand in the Accumulator Register. */
            A = operand;
        }

        private void TAX()
        {
            SetZNStatusRegisterProcessorFlags(A);

            /** - Transfer accumulator to index X. */
            X = A;
        }

        private void TXA()
        {
            SetZNStatusRegisterProcessorFlags(X);

            /** - Transfer index X to accumulator. */
            A = X;
        }

        private void DEX()
        {

        }

        private void INX()
        {
            
        }

        private void TAY()
        {
            SetZNStatusRegisterProcessorFlags(A);

            /** - Transfer accumulator to index Y. */
            Y = A;
        }

        private void TYA()
        {
            SetZNStatusRegisterProcessorFlags(Y);

            /** - Transfer index Y to accumulator. */
            A = Y;
        }

        private void DEY()
        {

        }

        private void INY()
        {

        }

        private void ROL()
        {

        }

        private void ROR()
        {

        }

        private void RTI()
        {

        }

        private void RTS()
        {

        }

        private void SBC()
        {

        }

        private void STA()
        {
            /** - Stores the Accumulator Register into the memory address specified in the operand. */
            Mem.WriteByte(PC, A);
        }

        private void TXS()
        {
            SetZNStatusRegisterProcessorFlags(X);

            /** - Transfer index X to Stack Pointer. */
            S = X;
        }

        private void TSX()
        {
            SetZNStatusRegisterProcessorFlags(S);

            /** - Transfer Stack Pointer to index X. */
            X = S;
        }

        private void PHA()
        {

        }

        private void PLA()
        {

        }

        private void PHP()
        {

        }

        private void PLP()
        {

        }

        private void STX()
        {
            /** - Stores the X Index Register into the memory address specified in the operand. */
            Mem.WriteByte(PC, X);
        }

        private void STY()
        {
            /** - Stores the Y Index Register into the memory address specified in the operand. */
            Mem.WriteByte(PC, Y);
        }

        private void AAC()
        {

        }

        private void AAX()
        {

        }

        private void ARR()
        {

        }

        private void ASR()
        {

        }

        private void ATX()
        {

        }

        private void AXA()
        {

        }

        private void AXS()
        {

        }

        private void DCP()
        {

        }

        private void DOP()
        {

        }

        private void ISC()
        {

        }

        private void KIL()
        {

        }

        private void LAR()
        {

        }

        private void LAX()
        {

        }

        private void RLA()
        {

        }

        private void RRA()
        {

        }

        private void SLO()
        {

        }

        private void SRE()
        {

        }

        private void SXA()
        {

        }
        private void SYA()
        {

        }

        private void TOP()
        {

        }

        private void XAA()
        {

        }

        private void XAS()
        {

        }

        #endregion

        #region Class Methods (Addressing modes)

        /** - Useful info: https://github.com/CodeSourcerer/Emulator/blob/master/NESEmulator/CS6502.cs */

            /**
             * name            abbr    len time formula for N  example
             * implied         impl    1   2    ---            tay
             * immediate       imm     2   2    arg            ora #$f0
             * zero page       dp      2   3    *arg           cmp $56
             * zero page,x     d,x     2   4    *(arg+x & $ff) adc $56,x
             * zero page,y     d,y     2   4    *(arg+y & $ff) ldx $56,y
             * absolute        abs     3   4    *arg           eor $3456
             * absolute,x      a,x     3   4i   *(arg+x)       and $3456,x
             * absolute,y      a,y     3   4i   *(arg+y)       sbc $3456,y
             * indirect,x      (d,x)   2   6    **(arg+x)      lda ($34,x)
             * indirect,y      (d),y   2   5i   *(*arg+y)      sta ($34),y
             * relative        rel     2   2tc  *(PC+arg)      beq loop
             */
            private void ZeroPage()
        {
            ushort absoluteAddress = ReadNextByte();
            absoluteAddress &= 0x00FF;
            AbsoluteAddress = absoluteAddress;
        }

        private void ZeroPageXIndex()
        {
            ushort absoluteAddress = (ushort)(ReadNextByte() + X);
            absoluteAddress &= 0x00FF;
            AbsoluteAddress = absoluteAddress;
        }

        private void ZeroPageYIndex()
        {
            ushort absoluteAddress = (ushort)(ReadNextByte() + Y);
            absoluteAddress &= 0x00FF;
            AbsoluteAddress = absoluteAddress;
        }

        private void Absolute()
        {
            ushort low = ReadNextByte();
            ushort high = ReadNextByte();

            ushort absoluteAddress = (ushort)((high << 8) | low);
            AbsoluteAddress = absoluteAddress;
        }

        private void AbsoluteXIndex()
        {
            ushort low = ReadNextByte();
            ushort high = ReadNextByte();

            ushort absoluteAddress = (ushort)((high << 8) | low);
            absoluteAddress += X;

            if ((absoluteAddress & 0xFF00) != (high << 8))
            {
                /** - Page changed, add additional clock cycle */
                CPUCycles++;
            }

            AbsoluteAddress = absoluteAddress;
        }

        private void AbsoluteYIndex()
        {
            ushort low = ReadNextByte();
            ushort high = ReadNextByte();

            ushort absoluteAddress = (ushort)((high << 8) | low);
            absoluteAddress += Y;

            if ((absoluteAddress & 0xFF00) != (high << 8))
            {
                /** - Page changed, add additional clock cycle */
                CPUCycles++;
            }

            AbsoluteAddress = absoluteAddress;
        }

        private void Indirect()
        {
            ushort pointerLow = ReadNextByte();
            ushort pointerHigh = ReadNextByte();
            ushort absoluteAddress;

            ushort pointer = (ushort)((pointerHigh << 8) | pointerLow);

            ushort lowByte = Mem.ReadByte(pointer);

            /** - Simulate page boundary hardware bug. */
            if (pointerLow == 0x00FF)
            {
                absoluteAddress = (ushort)((Mem.ReadByte((ushort)(pointer & 0xFF00)) << 8) | lowByte);
            }
            else
            {
                /** - Normal behavior */
                absoluteAddress = (ushort)((Mem.ReadByte((ushort)(pointer + 1)) << 8) | lowByte);
            }

            AbsoluteAddress = absoluteAddress;
        }

        private void Implied()
        {
            /*
             * Many instructions do not require access to operands stored in memory.
             * Examples of implied instructions are CLD (Clear Decimal Mode) and NOP (No Operation).
             */

            // Store Accumulator in a temp var???
            // ushort tempAccumulatorValue = A;

        }

        private void Accumulator()
        {
            /*
             * Some instructions operate directly on the contents of the accumulator. The only instructions to
             * use this addressing  mode  are  the  shift  instructions,  ASL (Arithmetic  Shift  Left),
             * LSR (Logical Shift Right), ROL (Rotate Left) and ROR (Rotate Right). 
             */

            // Store value in Accumulator???
            // A = ???
        }

        private void Immediate()
        {
            ushort absoluteAddress = ReadNextByte();
            AbsoluteAddress = absoluteAddress;
        }

        private void Relative()
        {
            ushort relativeAddress = ReadNextByte();
            if ((relativeAddress & 0x80) != 0)
            {
                relativeAddress |= 0xFF00;
            }

            RelativeAddress = relativeAddress;
        }

        private void IndirectXIndex()
        {
            ushort nextByte = ReadNextByte();

            ushort low = Mem.ReadByte((ushort)((ushort)(nextByte + X) & 0x00FF));
            ushort high = Mem.ReadByte((ushort)((ushort)(nextByte + X + 1) & 0x00FF));

            ushort absoluteAddress = (ushort)((high << 8) | low);
            AbsoluteAddress = absoluteAddress;
        }

        private void IndirectYIndex()
        {
            ushort nextByte = ReadNextByte();

            ushort low = Mem.ReadByte((ushort)(nextByte & 0x00FF));
            ushort high = Mem.ReadByte((ushort)((ushort)(nextByte + 1) & 0x00FF));

            ushort absoluteAddress = (ushort)((high << 8) | low);
            absoluteAddress += Y;

            if ((absoluteAddress & 0xFF00) != (high << 8))
            {
                /** - Page changed, add additional clock cycle */
                CPUCycles++;
            }

            AbsoluteAddress = absoluteAddress;
        }

        private void SetStatusRegisterProcessorFlags(byte value)
        {
            /** - @brief Status Register bits. */
            P = new BitArray(8);
            P[7] = (value & (byte)ProcessorFlags.N) != 0;
            P[6] = (value & (byte)ProcessorFlags.V) != 0;
            P[5] = (value & (byte)ProcessorFlags.R) != 0;
            P[4] = (value & (byte)ProcessorFlags.B) != 0;
            P[3] = (value & (byte)ProcessorFlags.D) != 0;
            P[2] = (value & (byte)ProcessorFlags.I) != 0;
            P[1] = (value & (byte)ProcessorFlags.Z) != 0;
            P[0] = (value & (byte)ProcessorFlags.C) != 0;
        }

        private void SetStatusRegisterProcessorFlag(ProcessorFlags processorFlag, byte value)
        {
            switch (processorFlag)
            {
                case ProcessorFlags.N:
                    P[7] = (value & (byte)ProcessorFlags.N) != 0;
                    break;
                case ProcessorFlags.V:
                    P[6] = (value & (byte)ProcessorFlags.V) != 0;
                    break;
                case ProcessorFlags.R:
                    P[5] = (value & (byte)ProcessorFlags.R) != 0;
                    break;
                case ProcessorFlags.B:
                    P[4] = (value & (byte)ProcessorFlags.B) != 0;
                    break;
                case ProcessorFlags.D:
                    P[3] = (value & (byte)ProcessorFlags.D) != 0;
                    break;
                case ProcessorFlags.I:
                    P[2] = (value & (byte)ProcessorFlags.I) != 0;
                    break;
                case ProcessorFlags.Z:
                    P[1] = (value & (byte)ProcessorFlags.Z) != 0;
                    break;
                case ProcessorFlags.C:
                    P[0] = (value & (byte)ProcessorFlags.C) != 0;
                    break;
            }
        }

        /**
         * @brief   This method sets the Negative and Zero status register processor flags.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void SetZNStatusRegisterProcessorFlags(byte value)
        {
            /** - Sets the Negative Flag equal to the 7th bit. */
            SetStatusRegisterProcessorFlag(ProcessorFlags.N, value);

            /** - Sets the Zero Flag if the Operand is $#00, otherwise clears it. */
            P[1] = (value & 0xFF) == 0;
            
        }

        private byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                /* Should not happen. */
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        #endregion

        //public override string ToString()
        //{
        //    StringBuilder stringBuilder = new StringBuilder();

        //    stringBuilder.AppendLine("CPU Information:");
        //    stringBuilder.AppendLine("----------------");
        //    stringBuilder.AppendLine("Registers:");
        //    stringBuilder.AppendLine("Accumulator (A): " + A);
        //    stringBuilder.AppendLine("X-Index (X): " + X);
        //    stringBuilder.AppendLine("Y-Index (Y): " + Y);
        //    stringBuilder.AppendLine("Status Register (Processor Flags) (P): " + ConvertToByte(P));
        //    stringBuilder.AppendLine(" - Negative Flag: " + P[7]);
        //    stringBuilder.AppendLine(" - Overflow Flag: " + P[6]);
        //    stringBuilder.AppendLine(" - Reserved/Ignored (Interrupted) Flag: " + P[5]);
        //    stringBuilder.AppendLine(" - No CPU effect (B) Flag: " + P[4]);
        //    stringBuilder.AppendLine(" - Decimal Flag: " + P[3]);
        //    stringBuilder.AppendLine(" - Interrupt Disable Flag: " + P[2]);
        //    stringBuilder.AppendLine(" - Zero Flag: " + P[1]);
        //    stringBuilder.AppendLine(" - Carry Flag: " + P[0]);
        //    stringBuilder.AppendLine("Stack Pointer (S): " + S);
        //    stringBuilder.AppendLine("Program Counter (PC): " + PC);
        //    //stringBuilder.AppendLine("Opcode: " + opCode);
        //    stringBuilder.AppendLine("");

        //    return stringBuilder.ToString();
        //}

        public string GetCurrentCPUState()
        {
            /** - Match the test ROM log format. */
            /*
            C000  4C F5 C5  A:00 X:00 Y:00 P:24 SP:FD CYC:  0
            C5F5  A2 00     A:00 X:00 Y:00 P:24 SP:FD CYC:  9
            C5F7  86 00     A:00 X:00 Y:00 P:26 SP:FD CYC: 15
            C5F9  86 10     A:00 X:00 Y:00 P:26 SP:FD CYC: 24


            C000  4C F5 C5  JMP $C5F5                       A:00 X:00 Y:00 P:24 SP:FD PPU:  0,  0 CYC:7
            C5F5  A2 00     LDX #$00                        A:00 X:00 Y:00 P:24 SP:FD PPU:  9,  0 CYC:10
            C5F7  86 00     STX $00 = 00                    A:00 X:00 Y:00 P:26 SP:FD PPU: 15,  0 CYC:12
            */

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(AbsoluteAddress.ToString("X4") + "  "
                                     + Mem.ReadByte(PC).ToString("X2") + " "
                                     + Mem.ReadByte((ushort) (PC + 1)).ToString("X2") + " "
                                     + Mem.ReadByte((ushort) (PC + 2)).ToString("X2") + " "
                                     + "A: " + A.ToString("X2") + " "
                                     + "X: " + X.ToString("X2") + " "
                                     + "Y: " + Y.ToString("X2") + " "
                                     + "P: " + ConvertToByte(P).ToString("X2") + " "
                                     + "SP: " + S.ToString("X2") + " "
                                     + "CYC:" + CPUCycles

            );

            Console.WriteLine(stringBuilder.ToString());

            return stringBuilder.ToString();
        }
    }
}

 