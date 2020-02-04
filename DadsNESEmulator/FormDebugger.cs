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
            
            // Start stepping
            while (true)
            {
                _cpu.Step();

                /** - Debugging is horribly unoptimized and freezes the form. Still sticking with the console for now. */ 
                //textBoxA.Text = _cpu.A.ToString("X");
                //textBoxCpuCycles.Text = _cpu.CPUCycles.ToString();
                //textBoxProgramCounter.Text = _cpu.PC.ToString("X");
                //textBoxX.Text = _cpu.X.ToString("X");
                //textBoxY.Text = _cpu.Y.ToString("X");
                //textBoxStack.Text = _cpu.S.ToString("X");

                //textBoxMainDebug.Text += _cpu.Opcode.ToString("X") + Opcodes.GetOpcodeName(_cpu.Opcode);

                //checkBoxC.Checked = _cpu.P[0];
                //checkBoxZ.Checked = _cpu.P[1];
                //checkBoxI.Checked = _cpu.P[2];
                //checkBoxD.Checked = _cpu.P[3];
                //checkBoxB.Checked = _cpu.P[4];
                //checkBoxR.Checked = _cpu.P[5];
                //checkBoxV.Checked = _cpu.P[6];
                //checkBoxN.Checked = _cpu.P[7];

                
                
                
                //Console.WriteLine(cpu.GetCurrentCPUState());
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
            // 1 step at a time
        }
    }
}
