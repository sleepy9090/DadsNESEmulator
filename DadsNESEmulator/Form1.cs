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

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = @"Open file...";
            openFileDialog.Filter = @"nds files (*.nes)|*.nes|All files (*.*)|*.*";
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

            /** - Remove header if present */
            if (cartridge.isiNESFormat)
            {
                //nesProgramBytesToLoad = new ArraySegment<byte>(nesProgramBytes, 0x0010, nesProgramBytes.Length - 0x0010).ToArray();
                nesProgramBytesToLoad = new ArraySegment<byte>(nesProgramBytes, 0x0010, 0x4000).ToArray();
            }
            else
            {
                nesProgramBytesToLoad = nesProgramBytes;
            }
            
            

            PPU ppu = new PPU();
            MemoryMap memoryMap = new MemoryMap(ppu);
            //memoryMap.LoadROM(nesProgramBytes);
            memoryMap.LoadROM(nesProgramBytesToLoad);

            CPU cpu = new CPU();
            cpu.Power(memoryMap);

            // Start stepping
            while (true)
            {
                cpu.Step();
                //Console.WriteLine(cpu.GetCurrentCPUState());
            }

            //Console.WriteLine(cpu.GetTestResults());
        }
    }
}
