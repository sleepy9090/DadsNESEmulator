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
using DadsNESEmulator.Types;

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

        public MemoryMap Mem
        {
            get;
            protected set;
        }

        public uint CPUCycles
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

        public AddressModes CurrentAddressMode
        {
            get;
            protected set;
        }

        public byte ImmediateByte
        {
            get;
            protected set;
        }

        /** - Triggered by PPU when NMI is fired */
        public bool Nmi
        {
            get;
            set;
        }

        #endregion

        #region Class methods

        public CPU()
        {

        }

        public void Power(MemoryMap mem)
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
            //P = new BitArray(new byte[] {0x34});
            P = new BitArray(new byte[] { 0x24 });
            S = 0xFD;

            /** - Power up cycles */
            CPUCycles = 7;
            ClockCount = 0;
            AbsoluteAddress = 0x0000;
            RelativeAddress = 0x0000;
            Mem = mem;

            /* - Nes Test - automated mode (for testing with no video/audio implemented) for nestest.nes test ROM */
            PC = 0xC000;
            /* - Nes Test - non-automated mode for nestest.nes test ROM */
            //PC = 0xC004;

            /** Correct start point (0xFFFC-0xFFFD) (for NROM), and all_instrs.nes test ROM */
            //PC = Mem.ReadShort(_RESET_VECTOR_ADDRESS);

            /** Correct start point (0xFFFC-0xFFFD) (for NROM), and all_instrs.nes test ROM (alt) */
            //ushort lo = Mem.ReadByte(_RESET_VECTOR_ADDRESS);
            //ushort hi = Mem.ReadByte(_RESET_VECTOR_ADDRESS + 1);
            //PC = (ushort)((hi << 8) | lo);

            Console.WriteLine("Now you're playing with POWER: Program Counter: 0x" + PC.ToString("X"));
            Console.WriteLine("");

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

            // ?
            P = new BitArray(new byte[] { 0x24 });

            /* - Nes Test - automated mode (for testing with no video/audio implemented) for nestest.nes test ROM */
            PC = Mem.ReadByte(0xC000);

            /* - Nes Test - non-automated mode for nestest.nes test ROM */
            //PC = Mem.ReadByte(0xC004);

            /* Set PC from 16-bit address 0xFFFC-0xFFFD */
            //PC = Mem.ReadShort(_RESET_VECTOR_ADDRESS);

            /** - Reset cycles? */
            CPUCycles = 7;
            ClockCount = 0;
            AbsoluteAddress = 0x0000;
            RelativeAddress = 0x0000;
        }

        public void Step()
        {
            /* @todo: NMI on PPU side */
            if (Nmi)
            {
                NMI();
            }
            else
            {

                Console.Write(PC.ToString("X") + "  ");

                byte oldA = A;
                byte oldX = X;
                byte oldY = Y;

                byte oldP = ConvertToByte(P);
                byte oldS = S;
                uint oldCPUCycles = CPUCycles;


                /** Set global for debugging or other uses for now. */
                Opcode = ReadNextByte();

                switch (Opcode)
                {
                    case Opcodes._ADC_IMMEDIATE:
                        Immediate();
                        ADC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ADC_ZERO_PAGE:
                        ZeroPage();
                        ADC();
                        CPUCycles += 3;
                        break;
                    case Opcodes._ADC_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        ADC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ADC_ABSOLUTE:
                        Absolute();
                        ADC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ADC_ABSOLUTE_X:
                        AbsoluteXIndex();
                        ADC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ADC_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        ADC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ADC_INDIRECT_X:
                        IndirectXIndex();
                        ADC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ADC_INDIRECT_Y:
                        IndirectYIndex();
                        ADC();
                        CPUCycles += 5;
                        break;
                    case Opcodes._AND_IMMEDIATE:
                        Immediate();
                        AND();
                        CPUCycles += 2;
                        break;
                    case Opcodes._AND_ZERO_PAGE:
                        ZeroPage();
                        AND();
                        CPUCycles += 3;
                        break;
                    case Opcodes._AND_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        AND();
                        CPUCycles += 4;
                        break;
                    case Opcodes._AND_ABSOLUTE:
                        Absolute();
                        AND();
                        CPUCycles += 4;
                        break;
                    case Opcodes._AND_ABSOLUTE_X:
                        AbsoluteXIndex();
                        AND();
                        CPUCycles += 4;
                        break;
                    case Opcodes._AND_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        AND();
                        CPUCycles += 4;
                        break;
                    case Opcodes._AND_INDIRECT_X:
                        IndirectXIndex();
                        AND();
                        CPUCycles += 6;
                        break;
                    case Opcodes._AND_INDIRECT_Y:
                        IndirectYIndex();
                        AND();
                        CPUCycles += 5;
                        break;
                    case Opcodes._ASL_ACCUMULATOR:
                        Accumulator();
                        ASL();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ASL_ZERO_PAGE:
                        ZeroPage();
                        ASL();
                        CPUCycles += 5;
                        break;
                    case Opcodes._ASL_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        ASL();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ASL_ABSOLUTE:
                        Absolute();
                        ASL();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ASL_ABSOLUTE_X:
                        AbsoluteXIndex();
                        ASL();
                        CPUCycles += 7;
                        break;
                    case Opcodes._BIT_ZERO_PAGE:
                        ZeroPage();
                        BIT();
                        CPUCycles += 3;
                        break;
                    case Opcodes._BIT_ABSOLUTE:
                        Absolute();
                        BIT();
                        CPUCycles += 4;
                        break;
                    case Opcodes._BPL:
                        Relative();
                        BPL();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BMI:
                        Relative();
                        BMI();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BVC:
                        Relative();
                        BVC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BVS:
                        Relative();
                        BVS();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BCC:
                        Relative();
                        BCC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BCS:
                        Relative();
                        BCS();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BNE:
                        Relative();
                        BNE();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BEQ:
                        Relative();
                        BEQ();
                        CPUCycles += 2;
                        break;
                    case Opcodes._BRK:
                        Implied();
                        BRK();
                        CPUCycles += 7;
                        break;
                    case Opcodes._CMP_IMMEDIATE:
                        Immediate();
                        CMP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._CMP_ZERO_PAGE:
                        ZeroPage();
                        CMP();
                        CPUCycles += 3;
                        break;
                    case Opcodes._CMP_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        CMP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._CMP_ABSOLUTE:
                        Absolute();
                        CMP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._CMP_ABSOLUTE_X:
                        AbsoluteXIndex();
                        CMP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._CMP_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        CMP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._CMP_INDIRECT_X:
                        IndirectXIndex();
                        CMP();
                        CPUCycles += 6;
                        break;
                    case Opcodes._CMP_INDIRECT_Y:
                        IndirectYIndex();
                        CMP();
                        CPUCycles += 5;
                        break;
                    case Opcodes._CPX_IMMEDIATE:
                        Immediate();
                        CPX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._CPX_ZERO_PAGE:
                        ZeroPage();
                        CPX();
                        CPUCycles += 3;
                        break;
                    case Opcodes._CPX_ABSOLUTE:
                        Absolute();
                        CPX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._CPY_IMMEDIATE:
                        Immediate();
                        CPY();
                        CPUCycles += 2;
                        break;
                    case Opcodes._CPY_ZERO_PAGE:
                        ZeroPage();
                        CPY();
                        CPUCycles += 3;
                        break;
                    case Opcodes._CPY_ABSOLUTE:
                        Absolute();
                        CPY();
                        CPUCycles += 4;
                        break;
                    case Opcodes._DEC_ZERO_PAGE:
                        ZeroPage();
                        DEC();
                        CPUCycles += 5;
                        break;
                    case Opcodes._DEC_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        DEC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._DEC_ABSOLUTE:
                        Absolute();
                        DEC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._DEC_ABSOLUTE_X:
                        AbsoluteXIndex();
                        DEC();
                        CPUCycles += 7;
                        break;
                    case Opcodes._EOR_IMMEDIATE:
                        Immediate();
                        EOR();
                        CPUCycles += 2;
                        break;
                    case Opcodes._EOR_ZERO_PAGE:
                        ZeroPage();
                        EOR();
                        CPUCycles += 3;
                        break;
                    case Opcodes._EOR_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        EOR();
                        CPUCycles += 4;
                        break;
                    case Opcodes._EOR_ABSOLUTE:
                        Absolute();
                        EOR();
                        CPUCycles += 4;
                        break;
                    case Opcodes._EOR_ABSOLUTE_X:
                        AbsoluteXIndex();
                        EOR();
                        CPUCycles += 4;
                        break;
                    case Opcodes._EOR_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        EOR();
                        CPUCycles += 4;
                        break;
                    case Opcodes._EOR_INDIRECT_X:
                        IndirectXIndex();
                        EOR();
                        CPUCycles += 6;
                        break;
                    case Opcodes._EOR_INDIRECT_Y:
                        IndirectYIndex();
                        EOR();
                        CPUCycles += 5;
                        break;
                    case Opcodes._CLC:
                        Implied();
                        CLC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._SEC:
                        Implied();
                        SEC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._CLI:
                        Implied();
                        CLI();
                        CPUCycles += 2;
                        break;
                    case Opcodes._SEI:
                        Implied();
                        SEI();
                        CPUCycles += 2;
                        break;
                    case Opcodes._CLV:
                        Implied();
                        CLV();
                        CPUCycles += 2;
                        break;
                    case Opcodes._CLD:
                        Implied();
                        CLD();
                        CPUCycles += 2;
                        break;
                    case Opcodes._SED:
                        Implied();
                        SED();
                        CPUCycles += 2;
                        break;
                    case Opcodes._INC_ZERO_PAGE:
                        ZeroPage();
                        INC();
                        CPUCycles += 5;
                        break;
                    case Opcodes._INC_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        INC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._INC_ABSOLUTE:
                        Absolute();
                        INC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._INC_ABSOLUTE_X:
                        AbsoluteXIndex();
                        INC();
                        CPUCycles += 7;
                        break;
                    case Opcodes._JMP_ABSOLUTE:
                        Absolute();
                        JMP();
                        CPUCycles += 3;
                        break;
                    case Opcodes._JMP_INDIRECT:
                        Indirect();
                        JMP();
                        CPUCycles += 5;
                        break;
                    case Opcodes._JSR_ABSOLUTE:
                        Absolute();
                        JSR();
                        CPUCycles += 6;
                        break;
                    case Opcodes._LDA_IMMEDIATE:
                        Immediate();
                        LDA();
                        CPUCycles += 2;
                        break;
                    case Opcodes._LDA_ZERO_PAGE:
                        ZeroPage();
                        LDA();
                        CPUCycles += 3;
                        break;
                    case Opcodes._LDA_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        LDA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDA_ABSOLUTE:
                        Absolute();
                        LDA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDA_ABSOLUTE_X:
                        AbsoluteXIndex();
                        LDA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDA_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        LDA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDA_INDIRECT_X:
                        IndirectXIndex();
                        LDA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._LDA_INDIRECT_Y:
                        IndirectYIndex();
                        LDA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._LDX_IMMEDIATE:
                        Immediate();
                        LDX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._LDX_ZERO_PAGE:
                        ZeroPage();
                        LDX();
                        CPUCycles += 3;
                        break;
                    case Opcodes._LDX_ZERO_PAGE_Y:
                        ZeroPageYIndex();
                        LDX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDX_ABSOLUTE:
                        Absolute();
                        LDX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDX_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        LDX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDY_IMMEDIATE:
                        Immediate();
                        LDY();
                        CPUCycles += 2;
                        break;
                    case Opcodes._LDY_ZERO_PAGE:
                        ZeroPage();
                        LDY();
                        CPUCycles += 3;
                        break;
                    case Opcodes._LDY_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        LDY();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDY_ABSOLUTE:
                        Absolute();
                        LDY();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LDY_ABSOLUTE_X:
                        AbsoluteXIndex();
                        LDY();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LSR_ACCUMULATOR:
                        Accumulator();
                        LSR();
                        CPUCycles += 2;
                        break;
                    case Opcodes._LSR_ZERO_PAGE:
                        ZeroPage();
                        LSR();
                        CPUCycles += 5;
                        break;
                    case Opcodes._LSR_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        LSR();
                        CPUCycles += 6;
                        break;
                    case Opcodes._LSR_ABSOLUTE:
                        Absolute();
                        LSR();
                        CPUCycles += 6;
                        break;
                    case Opcodes._LSR_ABSOLUTE_X:
                        AbsoluteXIndex();
                        LSR();
                        CPUCycles += 7;
                        break;
                    case Opcodes._NOP:
                        Implied();
                        NOP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ORA_IMMEDIATE:
                        Immediate();
                        ORA();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ORA_ZERO_PAGE:
                        ZeroPage();
                        ORA();
                        CPUCycles += 3;
                        break;
                    case Opcodes._ORA_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        ORA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ORA_ABSOLUTE:
                        Absolute();
                        ORA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ORA_ABSOLUTE_X:
                        AbsoluteXIndex();
                        ORA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ORA_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        ORA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ORA_INDIRECT_X:
                        IndirectXIndex();
                        ORA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ORA_INDIRECT_Y:
                        IndirectYIndex();
                        ORA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._TAX:
                        Implied();
                        TAX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._TXA:
                        Implied();
                        TXA();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DEX:
                        Implied();
                        DEX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._INX:
                        Implied();
                        INX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._TAY:
                        Implied();
                        TAY();
                        CPUCycles += 2;
                        break;
                    case Opcodes._TYA:
                        Implied();
                        TYA();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DEY:
                        Implied();
                        DEY();
                        CPUCycles += 2;
                        break;
                    case Opcodes._INY:
                        Implied();
                        INY();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ROL_ACCUMULATOR:
                        Accumulator();
                        ROL();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ROL_ZERO_PAGE:
                        ZeroPage();
                        ROL();
                        CPUCycles += 5;
                        break;
                    case Opcodes._ROL_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        ROL();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ROL_ABSOLUTE:
                        Absolute();
                        ROL();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ROL_ABSOLUTE_X:
                        AbsoluteXIndex();
                        ROL();
                        CPUCycles += 7;
                        break;
                    case Opcodes._ROR_ACCUMULATOR:
                        Accumulator();
                        ROR();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ROR_ZERO_PAGE:
                        ZeroPage();
                        ROR();
                        CPUCycles += 5;
                        break;
                    case Opcodes._ROR_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        ROR();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ROR_ABSOLUTE:
                        Absolute();
                        ROR();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ROR_ABSOLUTE_X:
                        AbsoluteXIndex();
                        ROR();
                        CPUCycles += 7;
                        break;
                    case Opcodes._RTI_IMPLIED:
                        Implied();
                        RTI();
                        CPUCycles += 6;
                        break;
                    case Opcodes._RTS_IMPLIED:
                        Implied();
                        RTS();
                        CPUCycles += 6;
                        break;
                    case Opcodes._SBC_IMMEDIATE:
                        Immediate();
                        SBC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._SBC_ZERO_PAGE:
                        ZeroPage();
                        SBC();
                        CPUCycles += 3;
                        break;
                    case Opcodes._SBC_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        SBC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._SBC_ABSOLUTE:
                        Absolute();
                        SBC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._SBC_ABSOLUTE_X:
                        AbsoluteXIndex();
                        SBC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._SBC_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        SBC();
                        CPUCycles += 4;
                        break;
                    case Opcodes._SBC_INDIRECT_X:
                        IndirectXIndex();
                        SBC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._SBC_INDIRECT_Y:
                        IndirectYIndex();
                        SBC();
                        CPUCycles += 5;
                        break;
                    case Opcodes._STA_ZERO_PAGE:
                        ZeroPage();
                        STA();
                        CPUCycles += 3;
                        break;
                    case Opcodes._STA_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        STA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._STA_ABSOLUTE:
                        Absolute();
                        STA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._STA_ABSOLUTE_X:
                        AbsoluteXIndex();
                        STA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._STA_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        STA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._STA_INDIRECT_X:
                        IndirectXIndex();
                        STA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._STA_INDIRECT_Y:
                        IndirectYIndex();
                        STA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._TXS:
                        Implied();
                        TXS();
                        CPUCycles += 2;
                        break;
                    case Opcodes._TSX:
                        Implied();
                        TSX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._PHA:
                        Implied();
                        PHA();
                        CPUCycles += 3;
                        break;
                    case Opcodes._PLA:
                        Implied();
                        PLA();
                        CPUCycles += 4;
                        break;
                    case Opcodes._PHP:
                        Implied();
                        PHP();
                        CPUCycles += 3;
                        break;
                    case Opcodes._PLP:
                        Implied();
                        PLP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._STX_ZERO_PAGE:
                        ZeroPage();
                        STX();
                        CPUCycles += 3;
                        break;
                    case Opcodes._STX_ZERO_PAGE_Y:
                        ZeroPageYIndex();
                        STX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._STX_ABSOLUTE:
                        Absolute();
                        STX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._STY_ZERO_PAGE:
                        ZeroPage();
                        STY();
                        CPUCycles += 3;
                        break;
                    case Opcodes._STY_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        STY();
                        CPUCycles += 4;
                        break;
                    case Opcodes._STY_ABSOLUTE:
                        Absolute();
                        STY();
                        CPUCycles += 4;
                        break;
                    /** - Unofficial / Illegal Opcodes */
                    case Opcodes._AAC_IMMEDIATE:
                        Immediate();
                        AAC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._AAC_IMMEDIATE_ALT:
                        Immediate();
                        AAC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._AAX_ZERO_PAGE:
                        ZeroPage();
                        AAX();
                        CPUCycles += 3;
                        break;
                    case Opcodes._AAX_ZERO_PAGE_Y:
                        ZeroPageYIndex();
                        AAX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._AAX_INDIRECT_X:
                        IndirectXIndex();
                        AAX();
                        CPUCycles += 6;
                        break;
                    case Opcodes._AAX_ABSOLUTE:
                        Absolute();
                        AAX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ARR_IMMEDIATE:
                        Immediate();
                        ARR();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ASR_IMMEDIATE:
                        Immediate();
                        ASR();
                        CPUCycles += 2;
                        break;
                    case Opcodes._ATX_IMMEDIATE:
                        Immediate();
                        ATX();
                        CPUCycles += 2;
                        break;
                    case Opcodes._AXA_ABSOLUTE_Y:
                        Absolute();
                        AXA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._AXA_INDIRECT_Y:
                        Indirect();
                        AXA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._AXS_IMMEDIATE:
                        Immediate();
                        AXS();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DCP_ZERO_PAGE:
                        ZeroPage();
                        DCP();
                        CPUCycles += 5;
                        break;
                    case Opcodes._DCP_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        DCP();
                        CPUCycles += 6;
                        break;
                    case Opcodes._DCP_ABSOLUTE:
                        Absolute();
                        DCP();
                        CPUCycles += 6;
                        break;
                    case Opcodes._DCP_ABSOLUTE_X:
                        AbsoluteXIndex();
                        DCP();
                        CPUCycles += 7;
                        break;
                    case Opcodes._DCP_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        DCP();
                        CPUCycles += 7;
                        break;
                    case Opcodes._DCP_INDIRECT_X:
                        IndirectXIndex();
                        DCP();
                        CPUCycles += 8;
                        break;
                    case Opcodes._DCP_INDIRECT_Y:
                        IndirectYIndex();
                        DCP();
                        CPUCycles += 8;
                        break;
                    case Opcodes._DOP_ZERO_PAGE:
                        ZeroPage();
                        DOP();
                        CPUCycles += 3;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        DOP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_X_ALT:
                        ZeroPageXIndex();
                        DOP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_ALT:
                        ZeroPage();
                        DOP();
                        CPUCycles += 3;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_X_ALT_2:
                        ZeroPageXIndex();
                        DOP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_ALT_2:
                        ZeroPage();
                        DOP();
                        CPUCycles += 3;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_X_ALT_3:
                        ZeroPageXIndex();
                        DOP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._DOP_IMMEDIATE:
                        Immediate();
                        DOP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DOP_IMMEDIATE_ALT:
                        Immediate();
                        DOP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DOP_IMMEDIATE_ALT_2:
                        Immediate();
                        DOP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DOP_IMMEDIATE_ALT_3:
                        Immediate();
                        DOP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_X_ALT_4:
                        ZeroPageXIndex();
                        DOP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._DOP_IMMEDIATE_ALT_4:
                        Immediate();
                        DOP();
                        CPUCycles += 2;
                        break;
                    case Opcodes._DOP_ZERO_PAGE_X_ALT_5:
                        ZeroPageXIndex();
                        DOP();
                        CPUCycles += 4;
                        break;
                    case Opcodes._ISC_ZERO_PAGE:
                        ZeroPage();
                        ISC();
                        CPUCycles += 5;
                        break;
                    case Opcodes._ISC_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        ISC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ISC_ABSOLUTE:
                        Absolute();
                        ISC();
                        CPUCycles += 6;
                        break;
                    case Opcodes._ISC_ABSOLUTE_X:
                        AbsoluteXIndex();
                        ISC();
                        CPUCycles += 7;
                        break;
                    case Opcodes._ISC_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        ISC();
                        CPUCycles += 7;
                        break;
                    case Opcodes._ISC_INDIRECT_X:
                        IndirectXIndex();
                        ISC();
                        CPUCycles += 8;
                        break;
                    case Opcodes._ISC_INDIRECT_Y:
                        IndirectYIndex();
                        ISC();
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
                        break;
                    case Opcodes._LAR_ABSOLUTE_Y:
                        Absolute();
                        LAR();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LAX_ZERO_PAGE:
                        ZeroPage();
                        LAX();
                        CPUCycles += 3;
                        break;
                    case Opcodes._LAX_ZERO_PAGE_Y:
                        ZeroPageYIndex();
                        LAX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LAX_ABSOLUTE:
                        Absolute();
                        LAX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LAX_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        LAX();
                        CPUCycles += 4;
                        break;
                    case Opcodes._LAX_INDIRECT_X:
                        IndirectXIndex();
                        LAX();
                        CPUCycles += 6;
                        break;
                    case Opcodes._LAX_INDIRECT_Y:
                        IndirectYIndex();
                        LAX();
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
                        CPUCycles += 2;
                        break;
                    case Opcodes._RLA_ZERO_PAGE:
                        ZeroPage();
                        RLA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._RLA_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        RLA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._RLA_ABSOLUTE:
                        Absolute();
                        RLA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._RLA_ABSOLUTE_X:
                        AbsoluteXIndex();
                        RLA();
                        CPUCycles += 7;
                        break;
                    case Opcodes._RLA_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        RLA();
                        CPUCycles += 7;
                        break;
                    case Opcodes._RLA_INDIRECT_X:
                        IndirectXIndex();
                        RLA();
                        CPUCycles += 8;
                        break;
                    case Opcodes._RLA_INDIRECT_Y:
                        IndirectYIndex();
                        RLA();
                        CPUCycles += 8;
                        break;
                    case Opcodes._RRA_ZERO_PAGE:
                        ZeroPage();
                        RRA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._RRA_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        RRA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._RRA_ABSOLUTE:
                        Absolute();
                        RRA();
                        CPUCycles += 6;
                        break;
                    case Opcodes._RRA_ABSOLUTE_X:
                        AbsoluteXIndex();
                        RRA();
                        CPUCycles += 7;
                        break;
                    case Opcodes._RRA_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        RRA();
                        CPUCycles += 7;
                        break;
                    case Opcodes._RRA_INDIRECT_X:
                        IndirectXIndex();
                        RRA();
                        CPUCycles += 8;
                        break;
                    case Opcodes._RRA_INDIRECT_Y:
                        IndirectYIndex();
                        RRA();
                        CPUCycles += 8;
                        break;
                    case Opcodes._SBC_IMMEDIATE_ALT:
                        Immediate();
                        SBC();
                        CPUCycles += 2;
                        break;
                    case Opcodes._SLO_ZERO_PAGE:
                        ZeroPage();
                        SLO();
                        CPUCycles += 5;
                        break;
                    case Opcodes._SLO_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        SLO();
                        CPUCycles += 6;
                        break;
                    case Opcodes._SLO_ABSOLUTE:
                        Absolute();
                        SLO();
                        CPUCycles += 6;
                        break;
                    case Opcodes._SLO_ABSOLUTE_X:
                        AbsoluteXIndex();
                        SLO();
                        CPUCycles += 7;
                        break;
                    case Opcodes._SLO_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        SLO();
                        CPUCycles += 7;
                        break;
                    case Opcodes._SLO_INDIRECT_X:
                        IndirectXIndex();
                        SLO();
                        CPUCycles += 8;
                        break;
                    case Opcodes._SLO_INDIRECT_Y:
                        IndirectYIndex();
                        SLO();
                        CPUCycles += 8;
                        break;
                    case Opcodes._SRE_ZERO_PAGE:
                        ZeroPage();
                        SRE();
                        CPUCycles += 5;
                        break;
                    case Opcodes._SRE_ZERO_PAGE_X:
                        ZeroPageXIndex();
                        SRE();
                        CPUCycles += 6;
                        break;
                    case Opcodes._SRE_ABSOLUTE:
                        Absolute();
                        SRE();
                        CPUCycles += 6;
                        break;
                    case Opcodes._SRE_ABSOLUTE_X:
                        AbsoluteXIndex();
                        SRE();
                        CPUCycles += 7;
                        break;
                    case Opcodes._SRE_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        SRE();
                        CPUCycles += 7;
                        break;
                    case Opcodes._SRE_INDIRECT_X:
                        IndirectXIndex();
                        SRE();
                        CPUCycles += 8;
                        break;
                    case Opcodes._SRE_INDIRECT_Y:
                        IndirectYIndex();
                        SRE();
                        CPUCycles += 8;
                        break;
                    case Opcodes._SXA_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        SXA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._SYA_ABSOLUTE_X:
                        AbsoluteXIndex();
                        SYA();
                        CPUCycles += 5;
                        break;
                    case Opcodes._TOP_ABSOLUTE:
                        Absolute();
                        TOP();
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
                        CPUCycles += 4;
                        break;
                    case Opcodes._XAA_IMMEDIATE:
                        Immediate();
                        XAA();
                        CPUCycles += 2;
                        break;
                    case Opcodes._XAS_ABSOLUTE_Y:
                        AbsoluteYIndex();
                        XAS();
                        CPUCycles += 5;
                        break;
                    default:
                        /** - Should not happen. */
                        break;
                }

                string opcodeName = Opcodes.GetOpcodeName(Opcode);

                /* Church up the log. */
                if (CurrentAddressMode == AddressModes.Immediate || CurrentAddressMode == AddressModes.ZeroPage ||
                    CurrentAddressMode == AddressModes.ZeroPageX
                    || CurrentAddressMode == AddressModes.ZeroPageY || CurrentAddressMode == AddressModes.Relative ||
                    CurrentAddressMode == AddressModes.Indirect
                    || CurrentAddressMode == AddressModes.IndirectX || CurrentAddressMode == AddressModes.IndirectY)
                {
                    Console.Write("    ");
                }
                else if (CurrentAddressMode == AddressModes.Implied || CurrentAddressMode == AddressModes.Accumulator)
                {
                    Console.Write("        ");
                }

                Console.Write(opcodeName.PadRight(30));
                // values before the opcode was processed so the log is easier to read
                Console.Write("A:" + oldA.ToString("X2") + " ");
                Console.Write("X:" + oldX.ToString("X2") + " ");
                Console.Write("Y:" + oldY.ToString("X2") + " ");
                Console.Write("P:" + oldP.ToString("X2") +
                              " "); // flags are either not being processed correctly or not logged at the right time according to logs.
                Console.Write("SP:" + oldS.ToString("X2") + " ");
                Console.WriteLine("CYC:" + oldCPUCycles + " ");
                //Console.WriteLine("SL:" + "scanline"); /* @todo implement sl when PPU completed */
            }
        }

        private byte ReadNextByte()
        {
            byte byteRead = Mem.ReadByte(PC);

            /** - Used for NES Tests from instr_test-v5.zip (all_instrs.nes, official_only.nes, rom_singles/*.*)
             * http://blargg.8bitalley.com/nes-tests/instr_test-v5.zip
             * https://wiki.nesdev.com/w/index.php/Emulator_tests
             *
             * Not sure if it is correct or fully useful without ppu and other implemented.
             *
             */
            //byte debugByte = Mem.ReadByte(0x6000);
            //if (debugByte == 0x80)
            //{
            //    Console.WriteLine("Test Running.");
            //}
            //else if (debugByte == 0x81)
            //{
            //    Console.WriteLine("Test needs reset pressed.");
            //}
            //else
            //{
            //    Console.WriteLine("Result code: " + debugByte.ToString("X"));
            //}

            
            Console.Write(byteRead.ToString("X2").PadRight(4));
            
            

            PC++;

            return byteRead;
        }

        #endregion

        #region Class Methods (Instructions/Opcodes)

        /** - References:
         * http://www.obelisk.me.uk/6502/reference.html
         * https://github.com/DanTup/DaNES/blob/master/DaNES.Emulation/Cpu.cs
         *
         */

            /**
             * @brief   ADC - Add with Carry - This instruction adds the contents of a memory location to the accumulator together with the carry bit.
             *                                 If overflow occurs the carry bit is set, this enables multiple byte addition to be performed.
             *
             * @return  N/A
             *
             * @author  Shawn M. Crawford
             *
             * @note    A,Z,C,N = A+M+C
             * 
             */
            private void ADC()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            /** - If P[0](Carry) is true, carryflag = 1 else carryflag = 0 */
            byte carryFlag = P[0] ? (byte)1 : (byte)0;

            /** - Add the accumulator, byte read, and carryFlag */
            int result = A + byteRead + carryFlag;

            byte oldA = A;

            /** - Set the Accumulator (0xFF keeps the value in the last 8 bits, not needed because this is a byte, but kept for reference) */
            A = (byte)(result & 0xFF);

            /** - Set the zero and negative flags */
            SetZNStatusRegisterProcessorFlags(A);

            /** - Set the carry flag (Set if overflow in bit 7) */
            P[0] = result > 0xFF;

            /** - Set the overflow flag - Set if overflow in bit 7 */
            P[6] = (((oldA ^ byteRead) & 0x80) == 0 && ((oldA ^ A) & 0x80) != 0);
        }

        /**
         * @brief   AND - Logical AND - A logical AND is performed, bit by bit, on the accumulator contents using the contents of a byte of memory.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    A,Z,N = A&M
         * 
         */
        private void AND()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            /** - AND values from the operand and the accumulator together, store the result in the Accumulator Register. */
            A &= byteRead;

            /** - Set the zero and negative flags */
            SetZNStatusRegisterProcessorFlags(A);

        }

        /**
         * @brief   ASL - Arithmetic Shift Left - This operation shifts all the bits of the accumulator or memory contents one bit left. Bit 0 is
         *                                        set to 0 and bit 7 is placed in the carry flag. The effect of this operation is to multiply the
         *                                        memory contents by 2 (ignoring 2's complement considerations), setting the carry if the result
         *                                        will not fit in 8 bits.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    A,Z,C,N = M*2 or M,Z,C,N = M*2
         * 
         */
        private void ASL()
        {
            /* Example
             ASL (BEFORE): 10110011 Carry Flag: X
             ASL         : Carry Flag: X<---10110011<---0
             ASL (AFTER) : 01100110 Carry Flag: 1
             */

            /** - Read the next byte or use accumulator depending on address mode. */
            byte byteRead = CurrentAddressMode == AddressModes.Accumulator ? A : Mem.ReadByte(AbsoluteAddress);

            /** - Set the carry flag.  byte read & 10000000 != 0 */
            P[0] = (byteRead & 0x80) != 0;

            /** - Shift left one bit. */
            byteRead <<= 1;

            /** - Write byte to memory or accumulator depending on address mode. */
            if (CurrentAddressMode == AddressModes.Accumulator)
            {
                /** - Set accumulator to shifted byte. */
                A = byteRead;
            }
            else
            {
                /** - Set memory to shifted byte. */
                Mem.WriteByte(AbsoluteAddress, byteRead);
            }

            /** - Set the zero flag and negative flag. */
            SetZNStatusRegisterProcessorFlags(byteRead);
        }

        /**
         * @brief   BIT - Bit Test - This instructions is used to test if one or more bits are set in a target memory location. The mask pattern in
         *                           A is ANDed with the value in memory to set or clear the zero flag, but the result is not kept. Bits 7 and 6 of
         *                           the value from memory are copied into the N and V flags.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    A & M, N = M7, V = M6
         * 
         */
        private void BIT()
        {
            /** - Read the next byte. */
            byte byteRead = Mem.ReadByte(AbsoluteAddress);
            byte temp = (byte)(A & byteRead);

            /** - Sets the Zero Flag if the value is 0x00, otherwise clears it. */
            P[1] = ((temp & 0xFF) == 0);

            /** - Set Negative flag to bit 7 of the memory value. */
            SetStatusRegisterProcessorFlag(ProcessorFlags.N, byteRead);

            /** - Set Overflow flag to bit 6 of the memory value. */
            SetStatusRegisterProcessorFlag(ProcessorFlags.V, byteRead);
            
        }

        private void BPL()
        {
            //Negative Flag is 0/false
            if (P[7] == false)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BMI()
        {
            //Negative Flag is 1/true
            if (P[7] == true)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BVC()
        {
            //Overflow Flag is 0/false
            if (P[6] == false)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BVS()
        {
            //Overflow Flag is 1/true
            if (P[6] == true)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BCC()
        {
            // Carry Flag is clear, 0/false
            if (P[0] == false)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BCS()
        {
            // Carry Flag is set, 1/true
            if (P[0] == true)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BNE()
        {
            // Zero Flag is 0/false
            if (P[1] == false)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BEQ()
        {
            // Zero Flag is 1/true
            if (P[1] == true)
            {
                /** - Branch was taken, add additional clock cycle. */
                CPUCycles++;
                AbsoluteAddress = (ushort)(PC + RelativeAddress);

                if ((AbsoluteAddress & 0xFF00) != (PC & 0xFF00))
                {
                    /** - Crossed page boundary, add additional clock cycle */
                    CPUCycles++;
                }

                PC = AbsoluteAddress;
            }
        }

        private void BRK()
        {
            // Push the Program Counter to the stack,
            Push16(PC);

            // Set the Interrupt Flag to temporarily prevent other IRQs from being executed
            P[2] = true;

            byte statusRegister = ConvertToByte(P);
            //Push(new byte[] {(byte) PC, statusRegister});
            
            // Push the status register to the stack
            Push(statusRegister);

            /** - Reload PC Counter with irq brk vector address */
            PC = Mem.ReadShort(IRQ_BRK_VECTOR_ADDRESS);
        }

        /**
         * @brief   CMP - Compare Memory and Accumulator
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    A - M
         * 
         */
        private void CMP()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
            
            byte tempByte = (byte)(A - byteRead);

            /** - Set the Carry flag. */
            P[0] = A >= byteRead;

            /** - Set the Zero flag and Negative flag. */
            SetZNStatusRegisterProcessorFlags(tempByte);
        }

        private void CPX()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            byte tempByte = (byte)(X - byteRead);

            /** - Set the Carry flag. */
            P[0] = X >= byteRead;

            /** - Set the Zero flag and Negative flag. */
            SetZNStatusRegisterProcessorFlags(tempByte);
        }

        private void CPY()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            byte tempByte = (byte)(Y - byteRead);

            /** - Set the Carry flag. */
            P[0] = Y >= byteRead;

            /** - Set the Zero flag and Negative flag. */
            SetZNStatusRegisterProcessorFlags(tempByte);
        }

        private void DEC()
        {
            /** - Read the next byte. */
            byte value = Mem.ReadByte(AbsoluteAddress);
            value--;

            /** - AbsoluteAddress is set in the ZeroPage, ZeroPageX, Absolute, AbsoluteX addressing. */
            Mem.WriteByte(AbsoluteAddress, value);
            
            SetZNStatusRegisterProcessorFlags(value);
        }

        private void EOR()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            /** - XOR values from the operand and the accumulator together */
            byteRead ^= A;

            SetZNStatusRegisterProcessorFlags(byteRead);

            /** - Store the Operand in the Accumulator Register. */
            A = byteRead;
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
            byte value = Mem.ReadByte(AbsoluteAddress);
            value++;

            /** - AbsoluteAddress is set in the ZeroPage, ZeroPageX, Absolute, AbsoluteX addressing. */
            Mem.WriteByte(AbsoluteAddress, value);

            SetZNStatusRegisterProcessorFlags(value);
        }

        private void JMP()
        {
            PC = AbsoluteAddress;
        }

        private void JSR()
        {
            Push((ushort)(PC - 1));
            PC = AbsoluteAddress;
        }

        private void LDA()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            SetZNStatusRegisterProcessorFlags(byteRead);

            /** - Stores the Operand in the Accumulator Register. */
            A = byteRead;
        }

        private void LDX()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            SetZNStatusRegisterProcessorFlags(byteRead);

            /** - Stores the Operand in the X Index Register. */
            X = byteRead;
        }

        private void LDY()
        {
            /** - Read the next byte. */
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            SetZNStatusRegisterProcessorFlags(byteRead);

            /** - Stores the Operand in the Y Index Register. */
            Y = byteRead;
        }

        /**
         * @brief   LSR - Logical Shift Right - Each of the bits in A or M is shift one place to the right. The bit that was in bit 0 is shifted
         *                                      into the carry flag. Bit 7 is set to zero.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    A,C,Z,N = A/2 or M,C,Z,N = M/2
         * 
         */
        private void LSR()
        {
            /* Example
             LSR (BEFORE): 10110011 Carry Flag: X
             LSR         : 0--->10110011--->Carry Flag: X
             LSR (AFTER) : 01011001 Carry Flag: 1
             */

            /** - Read the next byte or use accumulator depending on address mode. */
            byte byteRead = CurrentAddressMode == AddressModes.Accumulator ? A : Mem.ReadByte(AbsoluteAddress);

            /** - Set the carry flag.  byte read & 00000001 != 0 */
            P[0] = (byteRead & 0x1) != 0;

            /** - Shift right one bit. */
            byteRead >>= 1;

            /** - Write byte to memory or accumulator depending on address mode. */
            if (CurrentAddressMode == AddressModes.Accumulator)
            {
                /** - Set accumulator to shifted byte. */
                A = byteRead;
            }
            else
            {
                /** - Set memory to shifted byte. */
                Mem.WriteByte(AbsoluteAddress, byteRead);
            }

            /** - Set the zero flag and negative flag. */
            SetZNStatusRegisterProcessorFlags(byteRead);
        }

        /**
         * @brief   NOP - No Operation - The NOP instruction causes no changes to the processor other than the normal incrementing of the program
         *                               counter to the next instruction.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void NOP()
        {
            /** - No OPeration */
        }

        private void ORA()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            /** - OR values from the byte read and the accumulator together */
            byteRead |= A;

            SetZNStatusRegisterProcessorFlags(byteRead);

            /** - Store the byte read in the Accumulator Register. */
            A = byteRead;
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
            X--;
            SetZNStatusRegisterProcessorFlags(X);
        }

        private void INX()
        {
            X++;
            SetZNStatusRegisterProcessorFlags(X);
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
            Y--;
            SetZNStatusRegisterProcessorFlags(Y);
        }

        private void INY()
        {
            Y++;
            SetZNStatusRegisterProcessorFlags(Y);
        }

        /**
         * @brief   ROL - Rotate Left - Move each of the bits in either A or M one place to the left. Bit 0 is filled with the current value
         *                              of the carry flag whilst the old bit 7 becomes the new carry flag value.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void ROL()
        {
            /* Example
             ROL (BEFORE): 10110011 Carry Flag: X
             ROL         : Carry Flag: X<---10110011<---Carry Flag: X
             ROL (AFTER) : 0110011X Carry Flag: 1
             */

            /** - Read the next byte or use accumulator depending on address mode. */
            byte byteRead = CurrentAddressMode == AddressModes.Accumulator ? A : Mem.ReadByte(AbsoluteAddress);

            bool tempCarry = P[0];

            /** - Set the carry flag.  byte read & 10000000 != 0 */
            P[0] = (byteRead & 0x80) != 0;

            /** - Left shift byte read 1 bit, OR with 10000000 00000000 depending on carry flag */
            byte byteRotated = (byte)((byteRead << 1) | (tempCarry ? 0x1 : 0x0));

            /** - Write byte to memory or accumulator depending on address mode. */
            if (CurrentAddressMode == AddressModes.Accumulator)
            {
                /** - Set accumulator to rotated byte. */
                A = byteRotated;
                
            }
            else
            {
                /** - Set memory to rotated byte. */
                Mem.WriteByte(AbsoluteAddress, byteRotated);
            }

            /** - Set the zero flag and negative flag. */
            SetZNStatusRegisterProcessorFlags(byteRotated);
        }

        /**
         * @brief   ROR - Rotate Right - Move each of the bits in either A or M one place to the right. Bit 7 is filled with the current value
         *                               of the carry flag whilst the old bit 0 becomes the new carry flag value.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void ROR()
        {
            /* Example
             ROR (BEFORE): 10110011 Carry Flag: X
             ROR         : Carry Flag: X--->10110011--->Carry Flag: X
             ROR (AFTER) : X1011001 Carry Flag: 1
             */

            /** - Read the next byte or use accumulator depending on address mode. */
            byte byteRead = CurrentAddressMode == AddressModes.Accumulator ? A : Mem.ReadByte(AbsoluteAddress);

            bool tempCarry = P[0];

            /** - Set the carry flag.  byte read & 00000001 != 0 */
            P[0] = (byteRead & 0x1) != 0;
            
            /** - Right shift byte read 1 bit, OR with 10000000  if carry flag true, else 00000000 for false carry flag */
            byte byteRotated = (byte)((byteRead >> 1) | (tempCarry ? 0x80 : 0x0));

            /** - Write byte to memory or accumulator depending on address mode. */
            if (CurrentAddressMode == AddressModes.Accumulator)
            {
                /** - Set accumulator to rotated byte. */
                A = byteRotated;
            }
            else
            {
                /** - Set memory to rotated byte. */
                Mem.WriteByte(AbsoluteAddress, byteRotated);
            }

            /** - Set the zero flag and negative flag. */
            SetZNStatusRegisterProcessorFlags(byteRotated);
        }

        private void RTI()
        {
            SetStatusRegisterProcessorFlags(Pop());
            PC = Pop16();
            //PC = ConvertFromBytes(Pop(), Pop());
        }

        private void RTS()
        {
            PC = (ushort)(Pop16() + 1);
            //PC = (ushort)(ConvertFromBytes(Pop(), Pop()) + 1);
        }

        /**
         * @brief   SBC - Subtract Memory from Accumulator with Borrow
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    A - M - ~C -> A 
         * 
         */
        private void SBC()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);

            /** - If P[0](Carry) is true, carryflag = 1 else carryflag = 0 */
            byte carryFlag = P[0] ? (byte)1 : (byte)0;

            // ug ~carryFlag = -2, so using 1 - carryFlag instead
            /** - Add the accumulator, byte read, and carryFlag */
            int result = A - byteRead - (1 - carryFlag);

            byte oldA = A;

            /** - Set the Accumulator (0xFF keeps the value in the last 8 bits, not needed because this is a byte, but kept for reference) */
            A = (byte)(result & 0xFF);

            /** - Set the zero and negative flags */
            SetZNStatusRegisterProcessorFlags(A);

            /** - Clear the carry flag (Clear if overflow in bit 7) */
            P[0] = result >= 0;

            /** - Set the overflow flag - Set if overflow in bit 7 */
            P[6] = (((oldA ^ byteRead) & 0x80) != 0 && ((oldA ^ A) & 0x80) != 0);
        }

        /**
         * @brief   STA - STore Accumulator
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void STA()
        {
            /** - Stores the Accumulator Register into memory. */
            Mem.WriteByte(AbsoluteAddress, A);
        }

        /**
         * @brief   TXS - Transfer X to Stack Pointer
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void TXS()
        {
            /** - Transfer index X to Stack Pointer. */
            S = X;
        }

        /**
         * @brief   TXS - Transfer Stack Pointer to X
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void TSX()
        {
            SetZNStatusRegisterProcessorFlags(S);

            /** - Transfer Stack Pointer to index X. */
            X = S;
        }

        /**
         * @brief   PHA - Push Accumulator
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void PHA()
        {
            /** - Push the accumulator on the stack. */
            Push(A);
        }

        /**
         * @brief   PLA - Pull Accumulator
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void PLA()
        {
            A = Pop();
            SetZNStatusRegisterProcessorFlags(A);
        }

        /**
         * @brief   PHP - Push Processor Status
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void PHP()
        {
            byte statusRegister = ConvertToByte(P);
            Push((byte)(statusRegister | 16));
            //Push((byte)(GetStatus() | 16));
        }

        /**
         * @brief   PLP - Pull Processor Status
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void PLP()
        {
            // either of these should work
            P = new BitArray(new byte[] { PopProcessorStatus() });

            //byte statusRegister = ConvertToByte(P);
            //SetStatusRegisterProcessorFlags(statusRegister);
        }

        /**
         * @brief   STX - Store X Register
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void STX()
        {
            /** - Stores the X Index Register into memory. */
            Mem.WriteByte(AbsoluteAddress, X);
        }

        /**
         * @brief   STY - Store Y Register
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    N/A
         * 
         */
        private void STY()
        {
            /** - Stores the Y Index Register into memory. */
            Mem.WriteByte(AbsoluteAddress, Y);
        }

        #endregion

        #region Illegal Opcodes

        /**
         * @brief   AAC - AND byte with accumulator. If result is negative then carry is set.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    Status flags: N,Z,C
         * 
         */
        private void AAC()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
        }

        /**
         * @brief   AAX - AND X register with accumulator and store result in memory.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    Status flags: N,Z
         * 
         */
        private void AAX()
        {

        }

        /**
         * @brief   ARR - AND byte with accumulator, then rotate one bit right in accu-mulator and check bit 5 and 6:
         *          If both bits are 1: set C, clear V.
         *          If both bits are 0: clear C and V.
         *          If only bit 5 is 1: set V, clear C.
         *          If only bit 6 is 1: set C and V.
         *
         * @return  N/A
         *
         * @author  Shawn M. Crawford
         *
         * @note    Status flags: N,V,Z,C
         * 
         */
        private void ARR()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
        }

        private void ASR()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
        }

        private void ATX()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
        }

        private void AXA()
        {

        }

        private void AXS()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
        }

        private void DCP()
        {

        }

        private void DOP()
        {
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
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
            byte byteRead = CurrentAddressMode == AddressModes.Immediate ? ImmediateByte : Mem.ReadByte(AbsoluteAddress);
        }

        private void XAS()
        {

        }

        #endregion

        #region Class Methods (Addressing modes)
        

        private void Accumulator()
        {
            CurrentAddressMode = AddressModes.Accumulator;

            /*
             * Some instructions operate directly on the contents of the accumulator. The only instructions to
             * use this addressing  mode  are  the  shift  instructions,  ASL (Arithmetic  Shift  Left),
             * LSR (Logical Shift Right), ROL (Rotate Left) and ROR (Rotate Right). 
             */

            /** - Used for ASL, LSR, ROL, and ROR. Logic handled in those methods, ignore here. */

        }

        private void Immediate()
        {
            CurrentAddressMode = AddressModes.Immediate;
            ImmediateByte = ReadNextByte();
        }

        private void Implied()
        {
            CurrentAddressMode = AddressModes.Implied;

            /*
             * For many 6502 instructions the source and destination of the information to be manipulated is implied directly by the
             * function of the instruction itself and no further operand needs to be specified. Operations like 'Clear Carry Flag'
             * (CLC), CLD (Clear Decimal Mode), NOP (No Operation) and 'Return from Subroutine' (RTS) are implicit.
             */
        }

        private void ZeroPage()
        {
            CurrentAddressMode = AddressModes.ZeroPage;

            ushort absoluteAddress = ReadNextByte();
            absoluteAddress &= 0x00FF;
            AbsoluteAddress = absoluteAddress;
        }

        private void ZeroPageXIndex()
        {
            CurrentAddressMode = AddressModes.ZeroPageX;

            ushort absoluteAddress = (ushort)(ReadNextByte() + X);
            absoluteAddress &= 0x00FF;
            AbsoluteAddress = absoluteAddress;
        }

        private void ZeroPageYIndex()
        {
            CurrentAddressMode = AddressModes.ZeroPageY;

            ushort absoluteAddress = (ushort)(ReadNextByte() + Y);
            absoluteAddress &= 0x00FF;
            AbsoluteAddress = absoluteAddress;
        }

        private void Absolute()
        {
            CurrentAddressMode = AddressModes.Absolute;

            ushort low = ReadNextByte();
            ushort high = ReadNextByte();
            
            ushort absoluteAddress = (ushort)((high << 8) | low);
            AbsoluteAddress = absoluteAddress;
        }

        private void AbsoluteXIndex()
        {
            CurrentAddressMode = AddressModes.AbsoluteX;

            ushort low = ReadNextByte();
            ushort high = ReadNextByte();

            ushort absoluteAddress = (ushort)((high << 8) | low);
            absoluteAddress += X;

            if ((absoluteAddress & 0xFF00) != (high << 8))
            {
                /** - Crossed page boundary, add additional clock cycle */
                CPUCycles++;
            }

            AbsoluteAddress = absoluteAddress;
        }

        private void AbsoluteYIndex()
        {
            CurrentAddressMode = AddressModes.AbsoluteY;

            ushort low = ReadNextByte();
            ushort high = ReadNextByte();

            ushort absoluteAddress = (ushort)((high << 8) | low);
            absoluteAddress += Y;

            if ((absoluteAddress & 0xFF00) != (high << 8))
            {
                /** - Crossed page boundary, add additional clock cycle */
                CPUCycles++;
            }

            AbsoluteAddress = absoluteAddress;
        }

        private void Indirect()
        {
            CurrentAddressMode = AddressModes.Indirect;

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

        private void Relative()
        {
            CurrentAddressMode = AddressModes.Relative;

            ushort relativeAddress = ReadNextByte();
            if ((relativeAddress & 0x80) != 0)
            {
                relativeAddress |= 0xFF00;
            }

            RelativeAddress = relativeAddress;
        }

        private void IndirectXIndex()
        {
            CurrentAddressMode = AddressModes.IndirectX;

            ushort nextByte = ReadNextByte();

            ushort low = Mem.ReadByte((ushort)((ushort)(nextByte + X) & 0x00FF));
            ushort high = Mem.ReadByte((ushort)((ushort)(nextByte + X + 1) & 0x00FF));

            ushort absoluteAddress = (ushort)((high << 8) | low);
            AbsoluteAddress = absoluteAddress;
        }

        private void IndirectYIndex()
        {
            CurrentAddressMode = AddressModes.IndirectY;

            ushort nextByte = ReadNextByte();

            ushort low = Mem.ReadByte((ushort)(nextByte & 0x00FF));
            ushort high = Mem.ReadByte((ushort)((ushort)(nextByte + 1) & 0x00FF));

            ushort absoluteAddress = (ushort)((high << 8) | low);
            absoluteAddress += Y;

            if ((absoluteAddress & 0xFF00) != (high << 8))
            {
                /** - Crossed page boundary, add additional clock cycle */
                CPUCycles++;
            }

            AbsoluteAddress = absoluteAddress;
        }

        #endregion


        private void SetStatusRegisterProcessorFlags(byte value)
        {
            /** - @brief Status Register bits. */
            P = new BitArray(8);
            P[7] = (value & (byte)ProcessorFlags.N) != 0; /* Negative */
            P[6] = (value & (byte)ProcessorFlags.V) != 0; /* Overflow */
            //P[5] = (value & (byte)ProcessorFlags.R) != 0; /* Reserved */
            P[5] = true; /* bit 5 is always set to one */
            P[4] = (value & (byte)ProcessorFlags.B) != 0; /* No CPU effect - bit 4 is 1 if from an instruction (PHP or BRK) or 0 if from an interrupt line being pulled low (/IRQ or /NMI) */
            P[3] = (value & (byte)ProcessorFlags.D) != 0; /* Decimal - Unused in the NES 2A03 since there is no decimal mode. */
            //P[3] = false; /* bit 3 is always set to 0, leaving it in for the instruction test SED */
            P[2] = (value & (byte)ProcessorFlags.I) != 0; /* Interrupt Disable */
            P[1] = (value & (byte)ProcessorFlags.Z) != 0; /* Zero */
            P[0] = (value & (byte)ProcessorFlags.C) != 0; /* Carry */
        }

        private void NMI()
        {
            Nmi = false;

            Push16(PC);
            byte statusRegister = ConvertToByte(P);
            Push(statusRegister);

            PC = Mem.ReadShort(_NMI_VECTOR_ADDRESS);
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
                    P[5] = true; //(value & (byte)ProcessorFlags.R) != 0;
                    break;
                case ProcessorFlags.B:
                    /** - No CPU effect. */
                    P[4] = (value & (byte)ProcessorFlags.B) != 0;
                    break;
                case ProcessorFlags.D:
                    P[3] = (value & (byte)ProcessorFlags.D) != 0; /* bit 3 is always set to 0, leaving it in for the instruction test SED */
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
         * @brief   This method sets the Negative and Zero status register processor flags usign the passed in value.
         *
         * @param   value = The value to test to set the flags.
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

            /** - Sets the Zero Flag if the value is 0x00, otherwise clears it. */
            P[1] = ((value & 0xFF) == 0);
            
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

        byte[] ConvertToBytes(ushort value)
        {
            return new[] { (byte)(value >> 8), (byte)value };
        }

        private void Push(ushort value)
        {
            Push(ConvertToBytes(value));
        }

        private ushort ConvertFromBytes(byte b1, byte b2)
        {
            return (ushort)((b1 | b2) << 8);
        }

        private void Push(byte[] value)
        {
            foreach (byte b in value)
            {
                Push(b);
            }

        }

        private void Push(byte value)
        {
            Mem.WriteByte((ushort)(STACK_ADDRESS + S), value);
            S--;
        }

        private void Push16(ushort value)
        {
            Mem.WriteShort((ushort)(STACK_ADDRESS + (S - 1)), value);
            S -= 2;
        }

        private byte Pop()
        {
            S++;
            byte value = Mem.ReadByte((ushort)(STACK_ADDRESS + S));
            return value;
        }

        private ushort Pop16()
        {
            S += 2;
            //ushort value = Mem.ReadShort((ushort) (STACK_ADDRESS + (++S - 1)));
            ushort value = Mem.ReadShort((ushort)(STACK_ADDRESS + (S - 1)));
            return value;
        }

        private byte PopProcessorStatus()
        {
            /** - Ensure that break flag is not set and get the processor status off the stack. */
            byte value = (byte)(Pop() & ~(1 << 4));
            return value;
        }

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

            stringBuilder.AppendLine("PC: 0x" + PC.ToString("X4") + "  "
                                     + "Opcode: 0x" + Opcode.ToString("X2") + " "
                                     + "A: 0x" + A.ToString("X2") + " "
                                     + "X: 0x" + X.ToString("X2") + " "
                                     + "Y: 0x" + Y.ToString("X2") + " "
                                     + "P: 0x" + ConvertToByte(P).ToString("X2") + " "
                                     + "SP: 0x" + S.ToString("X2") + " "
                                     + "CYC:" + CPUCycles + " "
                                     + "Relative address: 0x" + RelativeAddress.ToString("X4") + " "
                                     + "Absolute address: 0x" + AbsoluteAddress.ToString("X4") + " "
                                     + "Current address mode: [" + CurrentAddressMode + "] "
            );

            

            return stringBuilder.ToString();
        }

        public string GetTestResults()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("TEST RESULTS: ");
            for (ushort i = 0x6000; i < 0x8000; i++)
            {
                stringBuilder.Append(Mem.ReadByte(i).ToString("X") + " ");
            }

            return stringBuilder.ToString();
        }
    }
}

 