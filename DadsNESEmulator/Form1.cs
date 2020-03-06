using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DadsNESEmulator.NESHardware;

namespace DadsNESEmulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static CPU _cpu
        {
            get;
            protected set;
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = @"Open file...";
            openFileDialog.Filter = @"nes files (*.nes)|*.nes|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;

                LoadROMFile(path);
            }
        }

        private void LoadROMFile(string path)
        {
            /** - Parse out ROM header */
            Cartridge cartridge = new Cartridge(path);
            Console.WriteLine(cartridge.GetCartridgeInfo());

            byte[] nesProgramBytes = File.ReadAllBytes(path);
            byte[] nesProgramBytesToLoad;

            if (cartridge.MapperByte != 0x0)
            {
                Console.WriteLine("Unsupported mapper: " + cartridge.MapperByte);
                Console.WriteLine("Only NROM is supported at this time.");
            }

            /** - Remove header if present */
            if (cartridge.isiNESFormat)
            {
                // Copy from end of header + 0x4000
                nesProgramBytesToLoad = new ArraySegment<byte>(nesProgramBytes, 0x0010, nesProgramBytes.Length - 0x0010).ToArray();
            }
            else
            {
                // Copy 0x4000
                nesProgramBytesToLoad = new ArraySegment<byte>(nesProgramBytes, 0x0000, nesProgramBytes.Length).ToArray();
            }

            PPUMemoryMap ppuMemoryMap = new PPUMemoryMap();
            PPU ppu = new PPU(ppuMemoryMap);
            MemoryMap memoryMap = new MemoryMap(ppu);
            //memoryMap.LoadROM(nesProgramBytes);
            memoryMap.LoadROM(nesProgramBytesToLoad, cartridge.PRGROMSize, cartridge.PRGROMSizeLSB, cartridge.CHRROMSize, cartridge.CHRROMSizeLSB);

            _cpu = new CPU();
            _cpu.Power(memoryMap);

            /** - Launch debugger */
            FormDebugger formDebugger = new FormDebugger( _cpu);
            formDebugger.ShowDialog();

            // Start stepping (moved into debugger form, once the emu has video support we'll come back here)
            //while (true)
            //{
            //    _cpu.Step();
            //    //Console.WriteLine(cpu.GetCurrentCPUState());
            //}

            //Console.WriteLine(cpu.GetTestResults());
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Figure this out later */
            //if (null != _cpu)
            //{
            //    FormDebugger formDebugger = new FormDebugger();
            //    formDebugger._cpu = _cpu;
            //    formDebugger.ShowDialog();
            //}
            //else
            //{
            //    FormDebugger formDebugger = new FormDebugger();
            //    formDebugger._cpu = new CPU();
            //    formDebugger.ShowDialog();
            //}
            
        }
    }
}
