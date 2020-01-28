﻿/**
 *  @file           MemoryMap.cs
 *  @brief          Defines the Memory Map (NES RAM/ROM).
 *  
 *  @copyright      2019
 *  @date           11/05/2019
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
using System;
using System.Linq;

namespace DadsNESEmulator.NESHardware
{
    /** @brief  Class for defining the Memory Map (NES RAM/ROM). */
    public class MemoryMap
    {
        /** - Reference:
         * http://www.fceux.com/web/help/fceux.html?NESRAMMappingFindingValues.html
         * https://wiki.nesdev.com/w/index.php/CPU_memory_map
         * https://everything2.com/title/NES%2520memory%2520map
         */

        /*
         *      __________________________________________
         * 0000| System RAM                               |
         *     |__________________________________________|
         * 0800| Mirrors of $0000-$07ff                   |
         *     |__________________________________________|
         * 2000| NES PPU registers                        |
         *     |__________________________________________|
         * 2008| Mirrors of $2000-$2007                   |
         *     |__________________________________________|
         * 4000| 2A03 registers                           |
         *     |__________________________________________|
         * 4018| Area controlled by mapper                |
         *     | Some of the more complex mappers'        |
         *     | registers appear here.                   |
         *     |__________________________________________|
         * 6000| Extra work RAM on cartridge              |
         *     | Some boards power this RAM with a        |
         *     | battery to allow for saving the game.    |
         *     | Other boards may map ROM here.           |
         *     |__________________________________________|
         * 8000| Cartridge program ROM                    |
         *     |__________________________________________|
         */

        /*
         * Memory Map (NES RAM/ROM)
         * 2A03 CPU memory map
         * 2A03 CPU is a 6502-compatible CPU without the decimal mode (CLD and SED do nothing). It has an on-die sound generator,
         * very limited DMA capability, and an input device controller that can be accessed through the 2A03 registers.
         * 6502 CPU Memory Map
         *
         * Address Range                Size in bytes        Notes (Page size = 256bytes)
         * (Hexadecimal)
         *
         * $0000 - $07FF                2048                Game Ram
         *  - $0000 - $00FF              - 256               - Zero Page - Special Zero Page addressing modes give faster memory read/write access
         *  - $0100 - $01FF              - 256               - Stack memory
         *  - $0200 - $07FF              - 1536              - RAM
         *
         * $0800 - $0FFF                2048                Mirror of $0000-$07FF
         *  - $0800 - $08FF              - 256               - Zero Page
         *  - $0900 - $09FF              - 256               - Stack
         *  - $0A00 - $0FFF              - 1024              - RAM
         *
         * $1000 - $17FF                2048 bytes          Mirror of $0000-$07FF
         *  - $1000 - $10FF              - 256               - Zero Page
         *  - $1100 - $11FF              - 256               - Stack
         *  - $1200 - $17FF              - 1024              - RAM
         *
         * $1800 - $1FFF                2048 bytes          Mirror of $0000-$07FF
         *  - $1800 - $18FF              - 256               - Zero Page
         *  - $1900 - $19FF              - 256               - Stack
         *  - $1A00 - $1FFF              - 1024              - RAM
         *
         * $2000 - $2007                8 bytes             Input / Output registers
         *
         * $2008 - $3FFF                8184 bytes          Mirror of $2000-$2007 (multiple times)
         *
         * $4000 - $401F                32 bytes            Input / Output registers
         *
         * $4020 - $5FFF                8160 bytes          Expansion ROM - Used with Nintendo's MMC5 to expand the capabilities of VRAM.
         *
         * $6000 - $7FFF                8192 bytes          SRAM - Save Ram used to save data between game plays.
         *
         * $8000 - $BFFF                16384 bytes         PRG-ROM lower bank - executable code
         *
         * $C000 - $FFFF                16384 bytes         PRG-ROM upper bank - executable code
         *  - $FFFA - $FFFB              - 2 bytes           - Address of Non Maskable Interrupt (NMI) handler routine
         *  - $FFFC - $FFFD              - 2 bytes           - Address of Power on reset handler routine
         *  - $FFFE - $FFFF              - 2 bytes           - Address of Break (BRK instruction) handler routine
         */

        private Memory gameRamMem = new Memory(0x800); // 2048
        private Memory ioRegistersMem = new Memory(0x20); // 8
        private Memory expansionRomMem = new Memory(0x1FE0); // 8160
        private Memory sramMem = new Memory(0x2000); // 8192
        private Memory cartridgeLowerBankMem;
        private Memory cartridgeUpperBankMem;
        private PPU ppu;

        public MemoryMap(PPU ppu)
        {
            ppu = new PPU();
        }

        public void LoadROM(byte[] nesProgram)
        {
            /** - Load the ROM into memory. */
            if (nesProgram.Length > 0x4000)
            {
                cartridgeLowerBankMem = new Memory(0x4000); // 16384
                cartridgeUpperBankMem = new Memory(0x4000); // 16384
                cartridgeLowerBankMem.Write(0x0, new ArraySegment<byte>(nesProgram, 0, 0x4000).ToArray());
                cartridgeUpperBankMem.Write(0x0, new ArraySegment<byte>(nesProgram, 0x4000, 0x4000).ToArray());
            }
            else
            {
                cartridgeLowerBankMem = new Memory(0x4000); // 16384
                cartridgeUpperBankMem = cartridgeLowerBankMem;
                cartridgeLowerBankMem.Write(0x0, nesProgram);
            }
        }

        public byte ReadByte(ushort address)
        {
            byte byteRead;

            if (address < 0x2000)
            {
                /** - mod 0x800 to use first address range if the address is from one of the mirrors. */ 
                byteRead = gameRamMem.Read((ushort) (address % 0x800));
            }
            else if (address < 0x4000)
            {
                /** - @todo: implement PPU, this call is stubbed and returns 0 */
                byteRead = ppu.ReadRegister((ushort) (0x2000 + (address - 0x2000) % 8));
            }
            else if (address < 0x4020)
            {
                byteRead = ioRegistersMem.Read((ushort) (address - 0x4000));
            }
            else if (address < 0x8000)
            {
                byteRead = sramMem.Read((ushort) (address - 0x4020));
            }
            else if (address < 0xC000)
            {
                byteRead = cartridgeLowerBankMem.Read((ushort) (address - 0x8000));
            }
            else
            {
                byteRead = cartridgeUpperBankMem.Read((ushort) (address - 0xC000));
            }

            #region Memory Read Debug
            /* @todo: Delete me. */
            if (address < 0x2000)
            {
                /** - Internal 2K Work (System) RAM (mirrored to 800h-1FFFh) */
                if (address < 0x0100)
                {
                    /** - Zero page */
                    Console.WriteLine("ReadByte (Zero page): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if(address < 0x0200)
                {
                    /** - Stack memory */
                    Console.WriteLine("ReadByte (Stack memory): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));

                }
                else if (address < 0x0800)
                {
                    /** - RAM */
                    Console.WriteLine("ReadByte (RAM): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x0900)
                {
                    /** - Zero page - Mirror of $0000-$00FF */
                    Console.WriteLine("ReadByte (Zero page - Mirror of $0000-$00FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x0A00)
                {
                    /** - Stack memory - Mirror of $0100-$01FF */
                    Console.WriteLine("ReadByte (Stack memory - Mirror of $0100-$01FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x1000)
                {
                    /** - RAM - Mirror of $0200-$07FF */
                    Console.WriteLine("ReadByte (RAM - Mirror of $0200-$07FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x1100)
                {
                    /** - Zero page - Mirror of $0000-$00FF */
                    Console.WriteLine("ReadByte (Zero page - Mirror of $0000-$00FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x1200)
                {
                    /** - Stack memory - Mirror of $0100-$01FF */
                    Console.WriteLine("ReadByte (Stack memory - Mirror of $0100-$01FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x1800)
                {
                    /** - RAM - Mirror of $0200-$07FF */
                    Console.WriteLine("ReadByte (RAM - Mirror of $0200-$07FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x1900)
                {
                    /** - Zero page - Mirror of $0000-$00FF */
                    Console.WriteLine("ReadByte (Zero page - Mirror of $0000-$00FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x1A00)
                {
                    /** - Stack memory - Mirror of $0100-$01FF */
                    Console.WriteLine("ReadByte (Stack memory - Mirror of $0100-$01FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0x2000)
                {
                    /** - RAM - Mirror of $0200-$07FF */
                    Console.WriteLine("ReadByte (RAM - Mirror of $0200-$07FF): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
            }
            else if (address < 0x4000)
            {
                

                /** - Internal PPU Registers (mirrored to 2008h-3FFFh) */
                if (address < 0x2008)
                {
                    /** - Input / Output registers */
                    Console.WriteLine("ReadByte (Internal PPU Registers (mirrored to 2008h-3FFFh)): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else
                {
                    /** - Mirror of $2000 -$2007(multiple times) */
                    Console.WriteLine("ReadByte (Mirror of $2000 -$2007(multiple times)): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
            }
            else if (address < 0x4018)
            {
                
                /** - 2A03 registers (Internal APU Registers) */
                Console.WriteLine("ReadByte (2A03 registers (Internal APU Registers)): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));

            }
            else if (address < 0x6000)
            {
                /** -   4018h-5FFFh   Cartridge Expansion Area almost 8K */
                Console.WriteLine("ReadByte (Cartridge Expansion Area almost 8K): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
            }
            else if (address < 0x8000)
            {
                /** - SRAM */
                Console.WriteLine("ReadByte (SRAM): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
            }
            else if (address < 0xC000)
            {
                /** - PRG-ROM lower bank */
                Console.WriteLine("ReadByte (PRG-ROM lower bank): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
            }
            else // >= 0xFFFF
            {
                /** - PRG-ROM upper bank */

                if (address < 0xFFFA)
                {
                    Console.WriteLine("ReadByte (PRG-ROM upper bank): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0xFFFC)
                {
                    /** - Address of Non Maskable Interrupt (NMI) handler routine */
                    Console.WriteLine("ReadByte (Address of Non Maskable Interrupt (NMI) handler routine): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else if (address < 0xFFFE)
                {
                    /** - Address of Power on reset handler routine */
                    Console.WriteLine("ReadByte (Address of Power on reset handler routine): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
                else
                {
                    /** - Address of Break (BRK instruction) handler routine */
                    Console.WriteLine("ReadByte (Address of Break (BRK instruction) handler routine): address: " + address.ToString("X") + " byteRead: " + byteRead.ToString("X"));
                }
            }
            #endregion

            return byteRead;
        }

        public void WriteByte(ushort address, byte value)
        {
            if (address < 0x2000)
            {
                gameRamMem.Write((ushort) (address % 0x800), value);
            }
            else if (address < 0x4000)
            {
                /** - @todo: implement PPU, this call is stubbed and returns 0 */
                ppu.WriteRegister((ushort) (0x2000 + ((address - 0x2000) % 8)), value);
            }
            else if (address < 0x4020)
            {
                ioRegistersMem.Write((ushort) (address - 0x4000), value);
            }
            else if (address < 0x8000)
            {
                sramMem.Write((ushort) (address - 0x4020), value);
            }
            else if (address < 0xC000)
            {
                cartridgeLowerBankMem.Write((ushort) (address - 0x8000), value);
            }
            else
            {
                cartridgeUpperBankMem.Write((ushort) (address - 0xC000), value);
            }

            #region Memory Write Debug
            /* @todo: Delete me. */
            if (address < 0x2000)
            {
                /** - System RAM */
                if (address < 0x0100)
                {
                    /** - Zero page */
                    Console.WriteLine("WriteByte (Zero page): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x0200)
                {
                    /** - Stack memory */
                    Console.WriteLine("WriteByte (Stack memory): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x0800)
                {
                    /** - RAM */
                    Console.WriteLine("WriteByte (RAM): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x0900)
                {
                    /** - Zero page - Mirror of $0000-$00FF */
                    Console.WriteLine("WriteByte (Zero page - Mirror of $0000-$00FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x0A00)
                {
                    /** - Stack memory - Mirror of $0100-$01FF */
                    Console.WriteLine("WriteByte (Stack memory - Mirror of $0100-$01FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x1000)
                {
                    /** - RAM - Mirror of $0200-$07FF */
                    Console.WriteLine("WriteByte (RAM - Mirror of $0200-$07FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x1100)
                {
                    /** - Zero page - Mirror of $0000-$00FF */
                    Console.WriteLine("WriteByte (Zero page - Mirror of $0000-$00FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x1200)
                {
                    /** - Stack memory - Mirror of $0100-$01FF */
                    Console.WriteLine("WriteByte (Stack memory - Mirror of $0100-$01FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x1800)
                {
                    /** - RAM - Mirror of $0200-$07FF */
                    Console.WriteLine("WriteByte (RAM - Mirror of $0200-$07FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x1900)
                {
                    /** - Zero page - Mirror of $0000-$00FF */
                    Console.WriteLine("WriteByte (Zero page - Mirror of $0000-$00FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x1A00)
                {
                    /** - Stack memory - Mirror of $0100-$01FF */
                    Console.WriteLine("WriteByte (Stack memory - Mirror of $0100-$01FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0x2000)
                {
                    /** - RAM - Mirror of $0200-$07FF */
                    Console.WriteLine("WriteByte (RAM - Mirror of $0200-$07FF): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
            }
            else if (address < 0x4000)
            {
                /** - PPU Registers */
                if (address < 0x2008)
                {
                    /** - Input / Output registers */
                    Console.WriteLine("WriteByte (PPU Registers, Input / Output registers): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else
                {
                    /** - Mirror of $2000 -$2007(multiple times) */
                    Console.WriteLine("WriteByte (PPU Registers, Mirror of $2000 -$2007(multiple times)): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
            }
            else if (address < 0x4020)
            {
                /** - 2A03 registers */
                Console.WriteLine("WriteByte (2A03 registers ): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));

            }
            else if (address < 0x6000)
            {
                /** - Expansion ROM */
                Console.WriteLine("WriteByte (Expansion ROM): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
            }
            else if (address < 0x8000)
            {
                /** - SRAM */
                Console.WriteLine("WriteByte (SRAM): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
            }
            else if (address < 0xC000)
            {
                /** - PRG-ROM lower bank */
                Console.WriteLine("WriteByte (PRG-ROM lower bank): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
            }
            else // >= 0xFFFF
            {
                /** - PRG-ROM upper bank */

                if (address < 0xFFFA)
                {
                    Console.WriteLine("WriteByte (PRG-ROM upper bank): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0xFFFC)
                {
                    /** - Address of Non Maskable Interrupt (NMI) handler routine */
                    Console.WriteLine("WriteByte (Address of Non Maskable Interrupt (NMI) handler routine): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else if (address < 0xFFFE)
                {
                    /** - Address of Power on reset handler routine */
                    Console.WriteLine("WriteByte (Address of Power on reset handler routine): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
                else
                {
                    /** - Address of Break (BRK instruction) handler routine */
                    Console.WriteLine("WriteByte (Address of Break (BRK instruction) handler routine): address: " + address.ToString("X") + " byteWritten: " + value.ToString("X"));
                }
            }
            #endregion
        }

        public ushort ReadShort(ushort address)
        {
            byte lowByte = ReadByte(address);
            byte highByte = ReadByte((ushort)(address + 1));

            return (ushort)(highByte << 8 | lowByte);
        }

        public void WriteShort(ushort address, ushort value)
        {
            byte lowByte = (byte)(value & 0xFF);
            byte highByte = (byte)((value & 0xFF00) >> 8);

            WriteByte(address, highByte);
            WriteByte((ushort)(address + 1), lowByte);
        }
    }
}