namespace DadsNESEmulator
{
    partial class FormDebugger
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxMainDebug = new System.Windows.Forms.TextBox();
            this.labelPc = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.textBoxA = new System.Windows.Forms.TextBox();
            this.labelY = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.labelA = new System.Windows.Forms.Label();
            this.textBoxProgramCounter = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxStack = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxZ = new System.Windows.Forms.CheckBox();
            this.checkBoxI = new System.Windows.Forms.CheckBox();
            this.checkBoxD = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxR = new System.Windows.Forms.CheckBox();
            this.checkBoxV = new System.Windows.Forms.CheckBox();
            this.checkBoxN = new System.Windows.Forms.CheckBox();
            this.checkBoxC = new System.Windows.Forms.CheckBox();
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonStep = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxCpuCycleCurrentIncrement = new System.Windows.Forms.TextBox();
            this.textBoxCpuCycles = new System.Windows.Forms.TextBox();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxMainDebug
            // 
            this.textBoxMainDebug.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMainDebug.Location = new System.Drawing.Point(12, 12);
            this.textBoxMainDebug.Multiline = true;
            this.textBoxMainDebug.Name = "textBoxMainDebug";
            this.textBoxMainDebug.ReadOnly = true;
            this.textBoxMainDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxMainDebug.Size = new System.Drawing.Size(526, 426);
            this.textBoxMainDebug.TabIndex = 0;
            // 
            // labelPc
            // 
            this.labelPc.AutoSize = true;
            this.labelPc.Location = new System.Drawing.Point(6, 16);
            this.labelPc.Name = "labelPc";
            this.labelPc.Size = new System.Drawing.Size(24, 13);
            this.labelPc.TabIndex = 1;
            this.labelPc.Text = "PC:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxY);
            this.groupBox1.Controls.Add(this.textBoxX);
            this.groupBox1.Controls.Add(this.textBoxA);
            this.groupBox1.Controls.Add(this.labelY);
            this.groupBox1.Controls.Add(this.labelX);
            this.groupBox1.Controls.Add(this.labelA);
            this.groupBox1.Controls.Add(this.textBoxProgramCounter);
            this.groupBox1.Controls.Add(this.labelPc);
            this.groupBox1.Location = new System.Drawing.Point(544, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 123);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // textBoxY
            // 
            this.textBoxY.Location = new System.Drawing.Point(29, 91);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(20, 20);
            this.textBoxY.TabIndex = 8;
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(29, 65);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(20, 20);
            this.textBoxX.TabIndex = 7;
            // 
            // textBoxA
            // 
            this.textBoxA.Location = new System.Drawing.Point(29, 39);
            this.textBoxA.Name = "textBoxA";
            this.textBoxA.Size = new System.Drawing.Size(20, 20);
            this.textBoxA.TabIndex = 6;
            // 
            // labelY
            // 
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(6, 94);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(17, 13);
            this.labelY.TabIndex = 5;
            this.labelY.Text = "Y:";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(6, 68);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(17, 13);
            this.labelX.TabIndex = 4;
            this.labelX.Text = "X:";
            // 
            // labelA
            // 
            this.labelA.AutoSize = true;
            this.labelA.Location = new System.Drawing.Point(6, 42);
            this.labelA.Name = "labelA";
            this.labelA.Size = new System.Drawing.Size(17, 13);
            this.labelA.TabIndex = 3;
            this.labelA.Text = "A:";
            // 
            // textBoxProgramCounter
            // 
            this.textBoxProgramCounter.Location = new System.Drawing.Point(29, 13);
            this.textBoxProgramCounter.Name = "textBoxProgramCounter";
            this.textBoxProgramCounter.Size = new System.Drawing.Size(50, 20);
            this.textBoxProgramCounter.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxStack);
            this.groupBox2.Location = new System.Drawing.Point(544, 141);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 150);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stack:";
            // 
            // textBoxStack
            // 
            this.textBoxStack.Location = new System.Drawing.Point(9, 19);
            this.textBoxStack.Multiline = true;
            this.textBoxStack.Name = "textBoxStack";
            this.textBoxStack.ReadOnly = true;
            this.textBoxStack.Size = new System.Drawing.Size(135, 125);
            this.textBoxStack.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxZ);
            this.groupBox3.Controls.Add(this.checkBoxI);
            this.groupBox3.Controls.Add(this.checkBoxD);
            this.groupBox3.Controls.Add(this.checkBoxB);
            this.groupBox3.Controls.Add(this.checkBoxR);
            this.groupBox3.Controls.Add(this.checkBoxV);
            this.groupBox3.Controls.Add(this.checkBoxN);
            this.groupBox3.Controls.Add(this.checkBoxC);
            this.groupBox3.Location = new System.Drawing.Point(544, 297);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(150, 68);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "SR / Processor Flags";
            // 
            // checkBoxZ
            // 
            this.checkBoxZ.AutoSize = true;
            this.checkBoxZ.Location = new System.Drawing.Point(78, 42);
            this.checkBoxZ.Name = "checkBoxZ";
            this.checkBoxZ.Size = new System.Drawing.Size(33, 17);
            this.checkBoxZ.TabIndex = 7;
            this.checkBoxZ.Text = "Z";
            this.checkBoxZ.UseVisualStyleBackColor = true;
            // 
            // checkBoxI
            // 
            this.checkBoxI.AutoSize = true;
            this.checkBoxI.Location = new System.Drawing.Point(43, 42);
            this.checkBoxI.Name = "checkBoxI";
            this.checkBoxI.Size = new System.Drawing.Size(29, 17);
            this.checkBoxI.TabIndex = 6;
            this.checkBoxI.Text = "I";
            this.checkBoxI.UseVisualStyleBackColor = true;
            // 
            // checkBoxD
            // 
            this.checkBoxD.AutoSize = true;
            this.checkBoxD.Location = new System.Drawing.Point(9, 42);
            this.checkBoxD.Name = "checkBoxD";
            this.checkBoxD.Size = new System.Drawing.Size(34, 17);
            this.checkBoxD.TabIndex = 5;
            this.checkBoxD.Text = "D";
            this.checkBoxD.UseVisualStyleBackColor = true;
            // 
            // checkBoxB
            // 
            this.checkBoxB.AutoSize = true;
            this.checkBoxB.Location = new System.Drawing.Point(116, 19);
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.Size = new System.Drawing.Size(33, 17);
            this.checkBoxB.TabIndex = 4;
            this.checkBoxB.Text = "B";
            this.checkBoxB.UseVisualStyleBackColor = true;
            // 
            // checkBoxR
            // 
            this.checkBoxR.AutoSize = true;
            this.checkBoxR.Location = new System.Drawing.Point(78, 19);
            this.checkBoxR.Name = "checkBoxR";
            this.checkBoxR.Size = new System.Drawing.Size(34, 17);
            this.checkBoxR.TabIndex = 3;
            this.checkBoxR.Text = "R";
            this.checkBoxR.UseVisualStyleBackColor = true;
            // 
            // checkBoxV
            // 
            this.checkBoxV.AutoSize = true;
            this.checkBoxV.Location = new System.Drawing.Point(43, 19);
            this.checkBoxV.Name = "checkBoxV";
            this.checkBoxV.Size = new System.Drawing.Size(33, 17);
            this.checkBoxV.TabIndex = 2;
            this.checkBoxV.Text = "V";
            this.checkBoxV.UseVisualStyleBackColor = true;
            // 
            // checkBoxN
            // 
            this.checkBoxN.AutoSize = true;
            this.checkBoxN.Location = new System.Drawing.Point(9, 19);
            this.checkBoxN.Name = "checkBoxN";
            this.checkBoxN.Size = new System.Drawing.Size(34, 17);
            this.checkBoxN.TabIndex = 1;
            this.checkBoxN.Text = "N";
            this.checkBoxN.UseVisualStyleBackColor = true;
            // 
            // checkBoxC
            // 
            this.checkBoxC.AutoSize = true;
            this.checkBoxC.Location = new System.Drawing.Point(116, 42);
            this.checkBoxC.Name = "checkBoxC";
            this.checkBoxC.Size = new System.Drawing.Size(33, 17);
            this.checkBoxC.TabIndex = 0;
            this.checkBoxC.Text = "C";
            this.checkBoxC.UseVisualStyleBackColor = true;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(700, 18);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 5;
            this.buttonRun.Text = "&Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(700, 77);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 6;
            this.buttonStep.Text = "&Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxCpuCycleCurrentIncrement);
            this.groupBox4.Controls.Add(this.textBoxCpuCycles);
            this.groupBox4.Location = new System.Drawing.Point(544, 371);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(149, 52);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "CPU Cycles:";
            // 
            // textBoxCpuCycleCurrentIncrement
            // 
            this.textBoxCpuCycleCurrentIncrement.Location = new System.Drawing.Point(123, 19);
            this.textBoxCpuCycleCurrentIncrement.Name = "textBoxCpuCycleCurrentIncrement";
            this.textBoxCpuCycleCurrentIncrement.Size = new System.Drawing.Size(20, 20);
            this.textBoxCpuCycleCurrentIncrement.TabIndex = 11;
            // 
            // textBoxCpuCycles
            // 
            this.textBoxCpuCycles.Location = new System.Drawing.Point(9, 19);
            this.textBoxCpuCycles.Name = "textBoxCpuCycles";
            this.textBoxCpuCycles.Size = new System.Drawing.Size(80, 20);
            this.textBoxCpuCycles.TabIndex = 0;
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(700, 47);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 11;
            this.buttonReset.Text = "R&eset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(700, 106);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 12;
            this.buttonClear.Text = "&Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // FormDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 450);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonStep);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxMainDebug);
            this.MaximizeBox = false;
            this.Name = "FormDebugger";
            this.Text = "Debugger";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMainDebug;
        private System.Windows.Forms.Label labelPc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.TextBox textBoxA;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelA;
        private System.Windows.Forms.TextBox textBoxProgramCounter;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxStack;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxN;
        private System.Windows.Forms.CheckBox checkBoxC;
        private System.Windows.Forms.CheckBox checkBoxV;
        private System.Windows.Forms.CheckBox checkBoxR;
        private System.Windows.Forms.CheckBox checkBoxD;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxZ;
        private System.Windows.Forms.CheckBox checkBoxI;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxCpuCycleCurrentIncrement;
        private System.Windows.Forms.TextBox textBoxCpuCycles;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonClear;
    }
}