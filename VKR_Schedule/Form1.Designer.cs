namespace VKR_Schedule
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.formsPlot1 = new ScottPlot.FormsPlot();
            this.popSize = new System.Windows.Forms.NumericUpDown();
            this.genNum = new System.Windows.Forms.NumericUpDown();
            this.mutProb = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.popSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mutProb)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 469);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 77);
            this.button1.TabIndex = 0;
            this.button1.Text = "Старт";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // formsPlot1
            // 
            this.formsPlot1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.formsPlot1.Location = new System.Drawing.Point(164, 11);
            this.formsPlot1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(907, 534);
            this.formsPlot1.TabIndex = 0;
            // 
            // popSize
            // 
            this.popSize.Location = new System.Drawing.Point(12, 36);
            this.popSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.popSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.popSize.Name = "popSize";
            this.popSize.Size = new System.Drawing.Size(132, 27);
            this.popSize.TabIndex = 1;
            this.popSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // genNum
            // 
            this.genNum.Location = new System.Drawing.Point(12, 89);
            this.genNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.genNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.genNum.Name = "genNum";
            this.genNum.Size = new System.Drawing.Size(132, 27);
            this.genNum.TabIndex = 2;
            this.genNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mutProb
            // 
            this.mutProb.Location = new System.Drawing.Point(12, 142);
            this.mutProb.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mutProb.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mutProb.Name = "mutProb";
            this.mutProb.Size = new System.Drawing.Size(132, 27);
            this.mutProb.TabIndex = 3;
            this.mutProb.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Размер популяции:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Количество поколений:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Вероятность мутации:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 385);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 78);
            this.button2.TabIndex = 8;
            this.button2.Text = "Сравнить расписания";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 175);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(144, 24);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Учёт заражений";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 558);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mutProb);
            this.Controls.Add(this.genNum);
            this.Controls.Add(this.popSize);
            this.Controls.Add(this.formsPlot1);
            this.Controls.Add(this.button1);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Form1";
            this.Text = "Расписание";
            ((System.ComponentModel.ISupportInitialize)(this.popSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mutProb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private ScottPlot.FormsPlot formsPlot1;
        private NumericUpDown popSize;
        private NumericUpDown genNum;
        private NumericUpDown mutProb;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button button2;
        private CheckBox checkBox1;
    }
}