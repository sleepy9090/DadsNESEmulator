/**
 *  @file           RAM.cs
 *  @brief          Defines the RAM.
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
    public class Memory
    {
        /** - Reference: http://www.fceux.com/web/help/fceux.html?NESRAMMappingFindingValues.html
         * https://wiki.nesdev.com/w/index.php/CPU_memory_map
         */

        /*
         * Memory Map (NES RAM/ROM)
         * 2A03 CPU memory map
         * 2A03 CPU is a 6502-compatible CPU without the decimal mode (CLD and SED do nothing). It has an on-die sound generator, very limited DMA capability, and an input device controller that can be accessed through the 2A03 registers.
         * 6502 CPU Memory Map
         * Address Range                Size in bytes        Notes (Page size = 256bytes)
         * (Hexadecimal)
         * $0000 - $07FF                2048                Game Ram
         * ($0000 - $00FF)                256                Zero Page - Special Zero Page addressing modes give faster memory read/write access
         * ($0100 - $01FF)                256                Stack memory
         * ($0200 - $07FF)                1536                RAM
         * $0800 - $0FFF                2048                Mirror of $0000-$07FF
         * ($0800 - $08FF)                256                 Zero Page
         * ($0900 - $09FF)        256                Stack
         * ($0A00 - $0FFF)                1024                Ram
         * $1000 - $17FF                2048 bytes        Mirror of $0000-$07FF
         * ($1000 - $10FF)                256                Zero Page
         * $1100 - $11FF                256                Stack
         * $1200 - $17FF                1024                RAM
         * $1800 - $1FFF                2048 bytes        Mirror of $0000-$07FF
         * ($1800 - $18FF)                256                Zero Page
         * ($1900 - $19FF)                256                Stack
         * ($1A00 - $1FFF)        1024                RAM
         * $2000 - $2007                8 bytes                Input / Output registers
         * $2008 - $3FFF                8184 bytes        Mirror of $2000-$2007 (mulitple times)
         * $4000 - $401F                32 bytes        Input / Output registers
         * $4020 - $5FFF                8160 bytes        Expansion ROM - Used with Nintendo's MMC5 to expand the capabilities of VRAM.
         * $6000 - $7FFF                8192 bytes        SRAM - Save Ram used to save data between game plays.
         * $8000 - $BFFF                16384 bytes        PRG-ROM lower bank - executable code
         * $C000 - $FFFF                16384 bytes        PRG-ROM upper bank - executable code
         * $FFFA - $FFFB        2 bytes                Address of Non Maskable Interrupt (NMI) handler routine
         * $FFFC - $FFFD        2 bytes                Address of Power on reset handler routine
         * $FFFE - $FFFF                2 bytes                Address of Break (BRK instruction) handler routine
         */

        private byte[] _memory { get; set; }

        public Memory(int size)
        {
            _memory = new byte[size];
        }

        public void LoadROM()
        {

        }

        public byte Read(ushort address)
        {
            return 0x00;
        }

        public byte Write(ushort address, byte value)
        {
            return 0x00;
        }
    }
}