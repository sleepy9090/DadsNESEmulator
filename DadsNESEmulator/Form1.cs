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

            /** - @todo: Check for ROM header? Use ArraySegment instead of byte array? */
            //ArraySegment<byte> nesProgram = new ArraySegment<byte>(nesProgramBytes, 0x0010, nesProgramBytes.Length - 0x0010);
            //ArraySegment<byte> nesProgram = new ArraySegment<byte>(nesProgramBytes, 0x00, nesProgramBytes.Length);
            
            PPU ppu = new PPU();
            MemoryMap memoryMap = new MemoryMap(ppu);
            memoryMap.LoadROM(nesProgramBytes);
            
            CPU cpu = new CPU();
            cpu.Power(memoryMap);

            // try some steps see what happens
            int i = 0;
            while (i < 10)
            {
                cpu.Step();
                Console.WriteLine(cpu.GetCurrentCPUState());
                i++;
            }
        }
    }
}
