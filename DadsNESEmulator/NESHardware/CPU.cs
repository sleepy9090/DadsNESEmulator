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

        public void Power()
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
            
            Mem = new Memory(0xFFFF);

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
                    break;
                case Opcodes._ADC_ZERO_PAGE:
                    break;
                case Opcodes._ADC_ZERO_PAGE_X:
                    break;
                case Opcodes._ADC_ABSOLUTE:
                    break;
                case Opcodes._ADC_ABSOULTE_X:
                    break;
                case Opcodes._ADC_ABSOLUTE_Y:
                    break;
                case Opcodes._ADC_INDIRECT_X:
                    break;
                case Opcodes._ADC_INDIRECT_Y:
                    break;
                case Opcodes._AND_IMMEDIATE:
                    break;
                case Opcodes._AND_ZERO_PAGE:
                    break;
                case Opcodes._AND_ZERO_PAGE_X:
                    break;
                case Opcodes._AND_ABSOLUTE:
                    break;
                case Opcodes._AND_ABSOULTE_X:
                    break;
                case Opcodes._AND_ABSOLUTE_Y:
                    break;
                case Opcodes._AND_INDIRECT_X:
                    break;
                case Opcodes._AND_INDIRECT_Y:
                    break;
                case Opcodes._ASL_ACCUMULATOR:
                    break;
                case Opcodes._ASL_ZERO_PAGE:
                    break;
                case Opcodes._ASL_ZERO_PAGE_X:
                    break;
                case Opcodes._ASL_ABSOLUTE:
                    break;
                case Opcodes._ASL_ABSOULTE_X:
                    break;
                case Opcodes._BIT_ZERO_PAGE:
                    break;
                case Opcodes._BIT_ABSOLUTE:
                    break;
                case Opcodes._BPL:
                    break;
                case Opcodes._BMI:
                    break;
                case Opcodes._BVC:
                    break;
                case Opcodes._BVS:
                    break;
                case Opcodes._BCC:
                    break;
                case Opcodes._BCS:
                    break;
                case Opcodes._BNE:
                    break;
                case Opcodes._BEQ:
                    break;
                case Opcodes._BRK:
                    break;
                case Opcodes._CMP_IMMEDIATE:
                    break;
                case Opcodes._CMP_ZERO_PAGE:
                    break;
                case Opcodes._CMP_ZERO_PAGE_X:
                    break;
                case Opcodes._CMP_ABSOLUTE:
                    break;
                case Opcodes._CMP_ABSOULTE_X:
                    break;
                case Opcodes._CMP_ABSOLUTE_Y:
                    break;
                case Opcodes._CMP_INDIRECT_X:
                    break;
                case Opcodes._CMP_INDIRECT_Y:
                    break;
                case Opcodes._CPX_IMMEDIATE:
                    break;
                case Opcodes._CPX_ZERO_PAGE:
                    break;
                case Opcodes._CPX_ABSOLUTE:
                    break;
                case Opcodes._CPY_IMMEDIATE:
                    break;
                case Opcodes._CPY_ZERO_PAGE:
                    break;
                case Opcodes._CPY_ABSOLUTE:
                    break;
                case Opcodes._DEC_ZERO_PAGE:
                    break;
                case Opcodes._DEC_ZERO_PAGE_X:
                    break;
                case Opcodes._DEC_ABSOLUTE:
                    break;
                case Opcodes._DEC_ABSOULTE_X:
                    break;
                case Opcodes._EOR_IMMEDIATE:
                    break;
                case Opcodes._EOR_ZERO_PAGE:
                    break;
                case Opcodes._EOR_ZERO_PAGE_X:
                    break;
                case Opcodes._EOR_ABSOLUTE:
                    break;
                case Opcodes._EOR_ABSOULTE_X:
                    break;
                case Opcodes._EOR_ABSOLUTE_Y:
                    break;
                case Opcodes._EOR_INDIRECT_X:
                    break;
                case Opcodes._EOR_INDIRECT_Y:
                    break;
                case Opcodes._CLC:
                    break;
                case Opcodes._SEC:
                    break;
                case Opcodes._CLI:
                    break;
                case Opcodes._SEI:
                    break;
                case Opcodes._CLV:
                    break;
                case Opcodes._CLD:
                    break;
                case Opcodes._SED:
                    break;
                case Opcodes._INC_ZERO_PAGE:
                    break;
                case Opcodes._INC_ZERO_PAGE_X:
                    break;
                case Opcodes._INC_ABSOLUTE:
                    break;
                case Opcodes._INC_ABSOULTE_X:
                    break;
                case Opcodes._JMP_ABSOLUTE:
                    break;
                case Opcodes._JMP_INDIRECT:
                    break;
                case Opcodes._JSR_ABSOLUTE:
                    break;
                case Opcodes._LDA_IMMEDIATE:
                    break;
                case Opcodes._LDA_ZERO_PAGE:
                    break;
                case Opcodes._LDA_ZERO_PAGE_X:
                    break;
                case Opcodes._LDA_ABSOLUTE:
                    break;
                case Opcodes._LDA_ABSOULTE_X:
                    break;
                case Opcodes._LDA_ABSOLUTE_Y:
                    break;
                case Opcodes._LDA_INDIRECT_X:
                    break;
                case Opcodes._LDA_INDIRECT_Y:
                    break;
                case Opcodes._LDX_IMMEDIATE:
                    break;
                case Opcodes._LDX_ZERO_PAGE:
                    break;
                case Opcodes._LDX_ZERO_PAGE_Y:
                    break;
                case Opcodes._LDX_ABSOLUTE:
                    break;
                case Opcodes._LDX_ABSOULTE_Y:
                    break;
                case Opcodes._LDY_IMMEDIATE:
                    break;
                case Opcodes._LDY_ZERO_PAGE:
                    break;
                case Opcodes._LDY_ZERO_PAGE_X:
                    break;
                case Opcodes._LDY_ABSOLUTE:
                    break;
                case Opcodes._LDY_ABSOULTE_X:
                    break;
                case Opcodes._LSR_ACCUMULATOR:
                    break;
                case Opcodes._LSR_ZERO_PAGE:
                    break;
                case Opcodes._LSR_ZERO_PAGE_X:
                    break;
                case Opcodes._LSR_ABSOLUTE:
                    break;
                case Opcodes._LSR_ABSOULTE_X:
                    break;
                case Opcodes._NOP:
                    NOP();
                    break;
                case Opcodes._ORA_IMMEDIATE:
                    break;
                case Opcodes._ORA_ZERO_PAGE:
                    break;
                case Opcodes._ORA_ZERO_PAGE_X:
                    break;
                case Opcodes._ORA_ABSOLUTE:
                    break;
                case Opcodes._ORA_ABSOULTE_X:
                    break;
                case Opcodes._ORA_ABSOLUTE_Y:
                    break;
                case Opcodes._ORA_INDIRECT_X:
                    break;
                case Opcodes._ORA_INDIRECT_Y:
                    break;
                case Opcodes._TAX:
                    break;
                case Opcodes._TXA:
                    break;
                case Opcodes._DEX:
                    break;
                case Opcodes._INX:
                    break;
                case Opcodes._TAY:
                    break;
                case Opcodes._TYA:
                    break;
                case Opcodes._DEY:
                    break;
                case Opcodes._INY:
                    break;
                case Opcodes._ROL_ACCUMULATOR:
                    break;
                case Opcodes._ROL_ZERO_PAGE:
                    break;
                case Opcodes._ROL_ZERO_PAGE_X:
                    break;
                case Opcodes._ROL_ABSOLUTE:
                    break;
                case Opcodes._ROL_ABSOULTE_X:
                    break;
                case Opcodes._ROR_ACCUMULATOR:
                    break;
                case Opcodes._ROR_ZERO_PAGE:
                    break;
                case Opcodes._ROR_ZERO_PAGE_X:
                    break;
                case Opcodes._ROR_ABSOLUTE:
                    break;
                case Opcodes._ROR_ABSOULTE_X:
                    break;
                case Opcodes._RTI_IMPLIED:
                    break;
                case Opcodes._RTS_IMPLIED:
                    break;
                case Opcodes._SBC_IMMEDIATE:
                    break;
                case Opcodes._SBC_ZERO_PAGE:
                    break;
                case Opcodes._SBC_ZERO_PAGE_X:
                    break;
                case Opcodes._SBC_ABSOLUTE:
                    break;
                case Opcodes._SBC_ABSOULTE_X:
                    break;
                case Opcodes._SBC_ABSOLUTE_Y:
                    break;
                case Opcodes._SBC_INDIRECT_X:
                    break;
                case Opcodes._SBC_INDIRECT_Y:
                    break;
                case Opcodes._STA_ZERO_PAGE:
                    break;
                case Opcodes._STA_ZERO_PAGE_X:
                    break;
                case Opcodes._STA_ABSOLUTE:
                    break;
                case Opcodes._STA_ABSOULTE_X:
                    break;
                case Opcodes._STA_ABSOLUTE_Y:
                    break;
                case Opcodes._STA_INDIRECT_X:
                    break;
                case Opcodes._STA_INDIRECT_Y:
                    break;
                case Opcodes._TXS:
                    break;
                case Opcodes._TSX:
                    break;
                case Opcodes._PHA:
                    break;
                case Opcodes._PLA:
                    break;
                case Opcodes._PHP:
                    break;
                case Opcodes._PLP:
                    break;
                case Opcodes._STX_ZERO_PAGE:
                    break;
                case Opcodes._STX_ZERO_PAGE_Y:
                    break;
                case Opcodes._STX_ABSOLUTE:
                    break;
                case Opcodes._STY_ZERO_PAGE:
                    break;
                case Opcodes._STY_ZERO_PAGE_Y:
                    break;
                case Opcodes._STY_ABSOLUTE:
                    break;
                case Opcodes._AAC_IMMEDIATE:
                    break;
                case Opcodes._AAC_IMMEDIATE_ALT:
                    break;
                case Opcodes._AAX_ZERO_PAGE:
                    break;
                case Opcodes._AAX_ZERO_PAGE_Y:
                    break;
                case Opcodes._AAX_INDIRECT_X:
                    break;
                case Opcodes._AAX_ABSOLUTE:
                    break;
                case Opcodes._ARR_IMMEDIATE:
                    break;
                case Opcodes._ASR_IMMEDIATE:
                    break;
                case Opcodes._ATX_IMMEDIATE:
                    break;
                case Opcodes._AXA_ABSOLUTE_Y:
                    break;
                case Opcodes._AXA_INDIRECT_Y:
                    break;
                case Opcodes._AXS_IMMEDIATE:
                    break;
                case Opcodes._DCP_ZERO_PAGE:
                    break;
                case Opcodes._DCP_ZERO_PAGE_X:
                    break;
                case Opcodes._DCP_ABSOLUTE:
                    break;
                case Opcodes._DCP_ABSOLUTE_X:
                    break;
                case Opcodes._DCP_ABSOLUTE_Y:
                    break;
                case Opcodes._DCP_INDIRECT_X:
                    break;
                case Opcodes._DCP_INDIRECT_Y:
                    break;
                case Opcodes._DOP_ZERO_PAGE:
                    break;
                case Opcodes._DOP_ZERO_PAGE_X:
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT:
                    break;
                case Opcodes._DOP_ZERO_PAGE_ALT:
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_2:
                    break;
                case Opcodes._DOP_ZERO_PAGE_ALT_2:
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_3:
                    break;
                case Opcodes._DOP_IMMEDIATE:
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT:
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT_2:
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT_3:
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_4:
                    break;
                case Opcodes._DOP_IMMEDIATE_ALT_4:
                    break;
                case Opcodes._DOP_ZERO_PAGE_X_ALT_5:
                    break;
                case Opcodes._ISC_ZERO_PAGE:
                    break;
                case Opcodes._ISC_ZERO_PAGE_X:
                    break;
                case Opcodes._ISC_ABSOLUTE:
                    break;
                case Opcodes._ISC_ABSOLUTE_X:
                    break;
                case Opcodes._ISC_ABSOLUTE_Y:
                    break;
                case Opcodes._ISC_INDIRECT_X:
                    break;
                case Opcodes._ISC_INDIRECT_Y:
                    break;
                case Opcodes._KIL_IMPLIED:
                    break;
                case Opcodes._KIL_IMPLIED_ALT:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_2:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_3:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_4:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_5:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_6:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_7:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_8:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_9:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_10:
                    break;
                case Opcodes._KIL_IMPLIED_ALT_11:
                    break;
                case Opcodes._LAR_ABSOLUTE_Y:
                    break;
                case Opcodes._LAX_ZERO_PAGE:
                    break;
                case Opcodes._LAX_ZERO_PAGE_Y:
                    break;
                case Opcodes._LAX_ABSOLUTE:
                    break;
                case Opcodes._LAX_ABSOLUTE_Y:
                    break;
                case Opcodes._LAX_INDIRECT_X:
                    break;
                case Opcodes._LAX_INDIRECT_Y:
                    break;
                case Opcodes._NOP_IMPLIED:
                    break;
                case Opcodes._NOP_IMPLIED_ALT:
                    break;
                case Opcodes._NOP_IMPLIED_ALT_2:
                    break;
                case Opcodes._NOP_IMPLIED_ALT_3:
                    break;
                case Opcodes._NOP_IMPLIED_ALT_4:
                    break;
                case Opcodes._NOP_IMPLIED_ALT_5:
                    break;
                case Opcodes._RLA_ZERO_PAGE:
                    break;
                case Opcodes._RLA_ZERO_PAGE_X:
                    break;
                case Opcodes._RLA_ABSOLUTE:
                    break;
                case Opcodes._RLA_ABSOLUTE_X:
                    break;
                case Opcodes._RLA_ABSOLUTE_Y:
                    break;
                case Opcodes._RLA_INDIRECT_X:
                    break;
                case Opcodes._RLA_INDIRECT_Y:
                    break;
                case Opcodes._RRA_ZERO_PAGE:
                    break;
                case Opcodes._RRA_ZERO_PAGE_X:
                    break;
                case Opcodes._RRA_ABSOLUTE:
                    break;
                case Opcodes._RRA_ABSOLUTE_X:
                    break;
                case Opcodes._RRA_ABSOLUTE_Y:
                    break;
                case Opcodes._RRA_INDIRECT_X:
                    break;
                case Opcodes._RRA_INDIRECT_Y:
                    break;
                case Opcodes._SBC_IMMEDIATE_ALT:
                    break;
                case Opcodes._SLO_ZERO_PAGE:
                    break;
                case Opcodes._SLO_ZERO_PAGE_X:
                    break;
                case Opcodes._SLO_ABSOLUTE:
                    break;
                case Opcodes._SLO_ABSOLUTE_X:
                    break;
                case Opcodes._SLO_ABSOLUTE_Y:
                    break;
                case Opcodes._SLO_INDIRECT_X:
                    break;
                case Opcodes._SLO_INDIRECT_Y:
                    break;
                case Opcodes._SRE_ZERO_PAGE:
                    break;
                case Opcodes._SRE_ZERO_PAGE_X:
                    break;
                case Opcodes._SRE_ABSOLUTE:
                    break;
                case Opcodes._SRE_ABSOLUTE_X:
                    break;
                case Opcodes._SRE_ABSOLUTE_Y:
                    break;
                case Opcodes._SRE_INDIRECT_X:
                    break;
                case Opcodes._SRE_INDIRECT_Y:
                    break;
                case Opcodes._SXA_ABSOLUTE_Y:
                    break;
                case Opcodes._SYA_ABSOLUTE_X:
                    break;
                case Opcodes._TOP_ABSOLUTE:
                    break;
                case Opcodes._TOP_ABSOLUTE_X:
                    break;
                case Opcodes._TOP_ABSOLUTE_X_ALT:
                    break;
                case Opcodes._TOP_ABSOLUTE_X_ALT_2:
                    break;
                case Opcodes._TOP_ABSOLUTE_X_ALT_3:
                    break;
                case Opcodes._TOP_ABSOLUTE_X_ALT_4:
                    break;
                case Opcodes._TOP_ABSOLUTE_X_ALT_5:
                    break;
                case Opcodes._XAA_IMMEDIATE:
                    break;
                case Opcodes._XAS_ABSOLUTE_Y:
                    break;
                default:
                    /** - Should not happen. */
                    break;
            }
        }

        private byte ReadNextByte()
        {
            byte opCode = Mem.ReadByte(PC);
            PC++;

            return opCode;
        }

        #endregion

        #region Class Methods (Instructions)

        private void ADC()
        {

        }

        private void AND()
        {

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

        }

        private void CLC()
        {

        }
        private void SEC()
        {

        }

        private void CLI()
        {

        }

        private void SEI()
        {

        }

        private void CLV()
        {

        }

        private void CLD()
        {

        }

        private void SED()
        {

        }

        private void INC()
        {

        }

        private void JMP()
        {

        }

        private void JSR()
        {

        }

        private void LDA()
        {

        }

        private void LDX()
        {

        }

        private void LDY()
        {

        }

        private void LSR()
        {

        }

        private void NOP()
        {

        }

        private void ORA()
        {

        }

        private void TAX()
        {

        }

        private void TXA()
        {

        }

        private void DEX()
        {

        }

        private void INX()
        {

        }

        private void TAY()
        {

        }

        private void TYA()
        {

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

        }

        private void TXS()
        {

        }

        private void TSX()
        {

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

        }

        private void STY()
        {

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
                // page changed, add additional clock cycle
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
                // page changed, add additional clock cycle
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
                // page changed, add additional clock cycle
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

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("CPU Information:");
            stringBuilder.AppendLine("----------------");
            stringBuilder.AppendLine("Registers:");
            stringBuilder.AppendLine("Accumulator (A): " + A);
            stringBuilder.AppendLine("X-Index (X): " + X);
            stringBuilder.AppendLine("Y-Index (Y): " + Y);
            stringBuilder.AppendLine("Status Register (Processor Flags) (P): " + ConvertToByte(P));
            stringBuilder.AppendLine(" - Negative Flag: " + P[7]);
            stringBuilder.AppendLine(" - Overflow Flag: " + P[6]);
            stringBuilder.AppendLine(" - Reserved/Ignored (Interrupted) Flag: " + P[5]);
            stringBuilder.AppendLine(" - No CPU effect (B) Flag: " + P[4]);
            stringBuilder.AppendLine(" - Decimal Flag: " + P[3]);
            stringBuilder.AppendLine(" - Interrupt Disable Flag: " + P[2]);
            stringBuilder.AppendLine(" - Zero Flag: " + P[1]);
            stringBuilder.AppendLine(" - Carry Flag: " + P[0]);
            stringBuilder.AppendLine("Stack Pointer (S): " + S);
            stringBuilder.AppendLine("Program Counter (PC): " + PC);
            //stringBuilder.AppendLine("Opcode: " + opCode);
            stringBuilder.AppendLine("");

            return stringBuilder.ToString();
        }
    }
}
 
 