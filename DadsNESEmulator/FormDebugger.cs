/**
 *  @file           FormDebugger.cs
 *  @brief          The debugger form.
 *  
 *  @copyright      2020
 *  @date           03/05/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
using System;
using System.Windows.Forms;
using DadsNESEmulator.NESHardware;

namespace DadsNESEmulator
{
    public partial class FormDebugger : Form
    {
        private CPU _cpu;
        private Timer timer;

        public FormDebugger()
        {
            InitializeComponent();
        }

        public FormDebugger(CPU cpu) : this()
        {
            _cpu = cpu;
            timer = new Timer();
        }

        public void RunDebug(object sender, System.EventArgs e)
        {
            uint currentCycles = _cpu.Step();

            ShowDebug(currentCycles);

            while (currentCycles > 0)
            {
                currentCycles--;
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
            timer.Interval = 500;
            timer.Tick += new EventHandler(RunDebug);
            timer.Start();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _cpu.Reset();
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
            textBoxMainDebug.AppendText(_cpu.PreviousPC.ToString("X4") + "  " + _cpu.TempByte1.PadRight(2) + "  " + _cpu.TempByte2.PadRight(2) + "  " + _cpu.TempByte3.PadRight(2) + "  ");
            textBoxMainDebug.AppendText(_cpu.OpcodeName.PadRight(17));
            textBoxMainDebug.AppendText(@"A:" + _cpu.PreviousA.ToString("X2") + @" ");
            textBoxMainDebug.AppendText(@"X:" + _cpu.PreviousX.ToString("X2") + @" ");
            textBoxMainDebug.AppendText(@"Y:" + _cpu.PreviousY.ToString("X2") + @" ");
            textBoxMainDebug.AppendText(@"P:" + _cpu.PreviousP.ToString("X2") + @" ");
            textBoxMainDebug.AppendText(@"SP:" + _cpu.PreviousS.ToString("X2") + @" ");
            textBoxMainDebug.AppendText(@"CYC:" + _cpu.PreviousCpuCycles + Environment.NewLine);
            //textBoxMainDebug.AppendText ("SL:" + _cpu.PreviousSL.ToString("X2")); /* @todo implement sl when PPU completed */
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }
    }
}
