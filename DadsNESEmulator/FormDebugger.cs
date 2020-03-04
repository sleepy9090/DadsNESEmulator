using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DadsNESEmulator.NESHardware;
using DadsNESEmulator.Types;

namespace DadsNESEmulator
{
    public partial class FormDebugger : Form
    {
        private CPU _cpu;

        public FormDebugger()
        {
            InitializeComponent();
            //StartDebugging();
        }

        public FormDebugger(CPU cpu) : this()
        {
            _cpu = cpu;
            //StartDebugging();
        }

        public void StartDebugging()
        {
            // @todo: Add a timer to handle stepping until PPU is figured out, otherwise this is way too fast

            // Start stepping
            while (true)
            {
                uint currentCycles = _cpu.Step();

                ShowDebug(currentCycles);

                while (currentCycles > 0)
                {
                    currentCycles--;
                }
            }
        }
        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxMainDebug.Clear();
            textBoxA.Clear();
            textBoxCpuCycleCurrentIncrement.Clear();
            textBoxCpuCycles.Clear();
            textBoxProgramCounter.Clear();
            textBoxStack.Clear();
            textBoxX.Clear();
            textBoxY.Clear();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            StartDebugging();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _cpu.Reset();
            StartDebugging();
        }

        private void buttonStep_Click(object sender, EventArgs e)
        {
            uint currentCycles = _cpu.Step();

            ShowDebug(currentCycles);

        }

        private void ShowDebug(uint currentCycles)
        {
            textBoxA.Text = _cpu.A.ToString("X");
            textBoxCpuCycles.Text = _cpu.CPUCycles.ToString();
            textBoxCpuCycleCurrentIncrement.Text = currentCycles.ToString("X");
            textBoxProgramCounter.Text = _cpu.PC.ToString("X");
            textBoxX.Text = _cpu.X.ToString("X");
            textBoxY.Text = _cpu.Y.ToString("X");
            textBoxStack.Text = _cpu.S.ToString("X");

            checkBoxC.Checked = _cpu.P[0];
            checkBoxZ.Checked = _cpu.P[1];
            checkBoxI.Checked = _cpu.P[2];
            checkBoxD.Checked = _cpu.P[3];
            checkBoxB.Checked = _cpu.P[4];
            checkBoxR.Checked = _cpu.P[5];
            checkBoxV.Checked = _cpu.P[6];
            checkBoxN.Checked = _cpu.P[7];

            // previous
            textBoxMainDebug.Text += (_cpu.PreviousPC.ToString("X4") + "  " + _cpu.TempByte1.PadRight(2) + "  " + _cpu.TempByte2.PadRight(2) + "  " + _cpu.TempByte3.PadRight(2) + "  ");
            textBoxMainDebug.Text += _cpu.OpcodeName.PadRight(17);
            textBoxMainDebug.Text += @"A:" + _cpu.PreviousA.ToString("X2") + @" ";
            textBoxMainDebug.Text += @"X:" + _cpu.PreviousX.ToString("X2") + @" ";
            textBoxMainDebug.Text += @"Y:" + _cpu.PreviousY.ToString("X2") + @" ";
            textBoxMainDebug.Text += @"P:" + _cpu.PreviousP.ToString("X2") + @" ";
            textBoxMainDebug.Text += @"SP:" + _cpu.PreviousS.ToString("X2") + @" ";
            textBoxMainDebug.Text += @"CYC:" + _cpu.PreviousCpuCycles + Environment.NewLine;
            //Console.WriteLine("SL:" + _cpu.PreviousSL.ToString("X2")); /* @todo implement sl when PPU completed */
        }
    }
}
