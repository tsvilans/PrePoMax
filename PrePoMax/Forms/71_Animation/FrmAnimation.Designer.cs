namespace PrePoMax.Forms
{
    partial class FrmAnimation
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lNumberOfAngles = new System.Windows.Forms.Label();
            this.numNumberOfAngles = new System.Windows.Forms.NumericUpDown();
            this.rbHarmonic = new System.Windows.Forms.RadioButton();
            this.numIncrementStep = new System.Windows.Forms.NumericUpDown();
            this.lIncrementStep = new System.Windows.Forms.Label();
            this.lNumberOfFrames = new System.Windows.Forms.Label();
            this.rbTimeIncrements = new System.Windows.Forms.RadioButton();
            this.numNumberOfFrames = new System.Windows.Forms.NumericUpDown();
            this.rbScaleFactor = new System.Windows.Forms.RadioButton();
            this.tbarFrameSelector = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.numFramesPerSecond = new System.Windows.Forms.NumericUpDown();
            this.rbOnce = new System.Windows.Forms.RadioButton();
            this.rbLoop = new System.Windows.Forms.RadioButton();
            this.rbSwing = new System.Windows.Forms.RadioButton();
            this.lCurrent = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnMoreLess = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSaveMovieAs = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayBackward = new System.Windows.Forms.Button();
            this.btnPlayForward = new System.Windows.Forms.Button();
            this.numCurrFrame = new System.Windows.Forms.NumericUpDown();
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.gbColorSpectrumLimits = new System.Windows.Forms.GroupBox();
            this.rbLimitsCurrentFrame = new System.Windows.Forms.RadioButton();
            this.rbLimitsAllFrames = new System.Windows.Forms.RadioButton();
            this.numLastFrame = new System.Windows.Forms.NumericUpDown();
            this.lLast = new System.Windows.Forms.Label();
            this.numFirstFrame = new System.Windows.Forms.NumericUpDown();
            this.lFirst = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.cbSaveAsImages = new System.Windows.Forms.CheckBox();
            this.cbEncoderOptions = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbGraphicsRam = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfAngles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncrementStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarFrameSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFramesPerSecond)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrFrame)).BeginInit();
            this.gbColorSpectrumLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLastFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstFrame)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lNumberOfAngles);
            this.groupBox1.Controls.Add(this.numNumberOfAngles);
            this.groupBox1.Controls.Add(this.rbHarmonic);
            this.groupBox1.Controls.Add(this.numIncrementStep);
            this.groupBox1.Controls.Add(this.lIncrementStep);
            this.groupBox1.Controls.Add(this.lNumberOfFrames);
            this.groupBox1.Controls.Add(this.rbTimeIncrements);
            this.groupBox1.Controls.Add(this.numNumberOfFrames);
            this.groupBox1.Controls.Add(this.rbScaleFactor);
            this.groupBox1.Location = new System.Drawing.Point(12, 133);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Animation Type";
            // 
            // lNumberOfAngles
            // 
            this.lNumberOfAngles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lNumberOfAngles.AutoSize = true;
            this.lNumberOfAngles.Location = new System.Drawing.Point(120, 74);
            this.lNumberOfAngles.Name = "lNumberOfAngles";
            this.lNumberOfAngles.Size = new System.Drawing.Size(102, 15);
            this.lNumberOfAngles.TabIndex = 9;
            this.lNumberOfAngles.Text = "Number of angles";
            // 
            // numNumberOfAngles
            // 
            this.numNumberOfAngles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numNumberOfAngles.Location = new System.Drawing.Point(228, 72);
            this.numNumberOfAngles.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numNumberOfAngles.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numNumberOfAngles.Name = "numNumberOfAngles";
            this.numNumberOfAngles.Size = new System.Drawing.Size(45, 23);
            this.numNumberOfAngles.TabIndex = 8;
            this.numNumberOfAngles.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numNumberOfAngles.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numNumberOfAngles.ValueChanged += new System.EventHandler(this.numNumberOfAngles_ValueChanged);
            // 
            // rbHarmonic
            // 
            this.rbHarmonic.AutoSize = true;
            this.rbHarmonic.Location = new System.Drawing.Point(6, 72);
            this.rbHarmonic.Name = "rbHarmonic";
            this.rbHarmonic.Size = new System.Drawing.Size(78, 19);
            this.rbHarmonic.TabIndex = 7;
            this.rbHarmonic.TabStop = true;
            this.rbHarmonic.Text = "Harmonic";
            this.rbHarmonic.UseVisualStyleBackColor = true;
            this.rbHarmonic.CheckedChanged += new System.EventHandler(this.AnimationType_CheckedChanged);
            // 
            // numIncrementStep
            // 
            this.numIncrementStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numIncrementStep.Location = new System.Drawing.Point(228, 47);
            this.numIncrementStep.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numIncrementStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIncrementStep.Name = "numIncrementStep";
            this.numIncrementStep.Size = new System.Drawing.Size(45, 23);
            this.numIncrementStep.TabIndex = 6;
            this.numIncrementStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numIncrementStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIncrementStep.ValueChanged += new System.EventHandler(this.NumIncrementStep_ValueChanged);
            // 
            // lIncrementStep
            // 
            this.lIncrementStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lIncrementStep.AutoSize = true;
            this.lIncrementStep.Location = new System.Drawing.Point(137, 49);
            this.lIncrementStep.Name = "lIncrementStep";
            this.lIncrementStep.Size = new System.Drawing.Size(86, 15);
            this.lIncrementStep.TabIndex = 5;
            this.lIncrementStep.Text = "Increment step";
            // 
            // lNumberOfFrames
            // 
            this.lNumberOfFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lNumberOfFrames.AutoSize = true;
            this.lNumberOfFrames.Location = new System.Drawing.Point(119, 24);
            this.lNumberOfFrames.Name = "lNumberOfFrames";
            this.lNumberOfFrames.Size = new System.Drawing.Size(104, 15);
            this.lNumberOfFrames.TabIndex = 4;
            this.lNumberOfFrames.Text = "Number of frames";
            // 
            // rbTimeIncrements
            // 
            this.rbTimeIncrements.AutoSize = true;
            this.rbTimeIncrements.Location = new System.Drawing.Point(6, 47);
            this.rbTimeIncrements.Name = "rbTimeIncrements";
            this.rbTimeIncrements.Size = new System.Drawing.Size(113, 19);
            this.rbTimeIncrements.TabIndex = 1;
            this.rbTimeIncrements.TabStop = true;
            this.rbTimeIncrements.Text = "Time increments";
            this.rbTimeIncrements.UseVisualStyleBackColor = true;
            this.rbTimeIncrements.CheckedChanged += new System.EventHandler(this.AnimationType_CheckedChanged);
            // 
            // numNumberOfFrames
            // 
            this.numNumberOfFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numNumberOfFrames.Location = new System.Drawing.Point(228, 22);
            this.numNumberOfFrames.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numNumberOfFrames.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numNumberOfFrames.Name = "numNumberOfFrames";
            this.numNumberOfFrames.Size = new System.Drawing.Size(45, 23);
            this.numNumberOfFrames.TabIndex = 3;
            this.numNumberOfFrames.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numNumberOfFrames.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numNumberOfFrames.ValueChanged += new System.EventHandler(this.numNumberOfFrames_ValueChanged);
            // 
            // rbScaleFactor
            // 
            this.rbScaleFactor.AutoSize = true;
            this.rbScaleFactor.Checked = true;
            this.rbScaleFactor.Location = new System.Drawing.Point(6, 22);
            this.rbScaleFactor.Name = "rbScaleFactor";
            this.rbScaleFactor.Size = new System.Drawing.Size(86, 19);
            this.rbScaleFactor.TabIndex = 0;
            this.rbScaleFactor.TabStop = true;
            this.rbScaleFactor.Text = "Scale factor";
            this.rbScaleFactor.UseVisualStyleBackColor = true;
            this.rbScaleFactor.CheckedChanged += new System.EventHandler(this.AnimationType_CheckedChanged);
            // 
            // tbarFrameSelector
            // 
            this.tbarFrameSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbarFrameSelector.AutoSize = false;
            this.tbarFrameSelector.Location = new System.Drawing.Point(6, 49);
            this.tbarFrameSelector.Maximum = 15;
            this.tbarFrameSelector.Minimum = 1;
            this.tbarFrameSelector.Name = "tbarFrameSelector";
            this.tbarFrameSelector.Size = new System.Drawing.Size(269, 23);
            this.tbarFrameSelector.TabIndex = 5;
            this.tbarFrameSelector.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbarFrameSelector.Value = 8;
            this.tbarFrameSelector.ValueChanged += new System.EventHandler(this.tbarFrameSelector_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(198, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "FPS";
            // 
            // numFramesPerSecond
            // 
            this.numFramesPerSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numFramesPerSecond.Location = new System.Drawing.Point(230, 22);
            this.numFramesPerSecond.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numFramesPerSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFramesPerSecond.Name = "numFramesPerSecond";
            this.numFramesPerSecond.Size = new System.Drawing.Size(45, 23);
            this.numFramesPerSecond.TabIndex = 9;
            this.numFramesPerSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numFramesPerSecond.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numFramesPerSecond.ValueChanged += new System.EventHandler(this.numFramesPerSecond_ValueChanged);
            // 
            // rbOnce
            // 
            this.rbOnce.AutoSize = true;
            this.rbOnce.Location = new System.Drawing.Point(6, 22);
            this.rbOnce.Name = "rbOnce";
            this.rbOnce.Size = new System.Drawing.Size(53, 19);
            this.rbOnce.TabIndex = 11;
            this.rbOnce.Text = "Once";
            this.rbOnce.UseVisualStyleBackColor = true;
            this.rbOnce.CheckedChanged += new System.EventHandler(this.rbAnimationStyle_CheckedChanged);
            // 
            // rbLoop
            // 
            this.rbLoop.AutoSize = true;
            this.rbLoop.Location = new System.Drawing.Point(68, 22);
            this.rbLoop.Name = "rbLoop";
            this.rbLoop.Size = new System.Drawing.Size(52, 19);
            this.rbLoop.TabIndex = 12;
            this.rbLoop.Text = "Loop";
            this.rbLoop.UseVisualStyleBackColor = true;
            this.rbLoop.CheckedChanged += new System.EventHandler(this.rbAnimationStyle_CheckedChanged);
            // 
            // rbSwing
            // 
            this.rbSwing.AutoSize = true;
            this.rbSwing.Checked = true;
            this.rbSwing.Location = new System.Drawing.Point(126, 22);
            this.rbSwing.Name = "rbSwing";
            this.rbSwing.Size = new System.Drawing.Size(57, 19);
            this.rbSwing.TabIndex = 13;
            this.rbSwing.TabStop = true;
            this.rbSwing.Text = "Swing";
            this.rbSwing.UseVisualStyleBackColor = true;
            this.rbSwing.CheckedChanged += new System.EventHandler(this.rbAnimationStyle_CheckedChanged);
            // 
            // lCurrent
            // 
            this.lCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lCurrent.AutoSize = true;
            this.lCurrent.Location = new System.Drawing.Point(92, 19);
            this.lCurrent.Name = "lCurrent";
            this.lCurrent.Size = new System.Drawing.Size(47, 15);
            this.lCurrent.TabIndex = 14;
            this.lCurrent.Text = "Current";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rbOnce);
            this.groupBox2.Controls.Add(this.numFramesPerSecond);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.rbSwing);
            this.groupBox2.Controls.Add(this.rbLoop);
            this.groupBox2.Location = new System.Drawing.Point(11, 240);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(281, 51);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Animation Style";
            // 
            // btnMoreLess
            // 
            this.btnMoreLess.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnMoreLess.Location = new System.Drawing.Point(9, 22);
            this.btnMoreLess.Name = "btnMoreLess";
            this.btnMoreLess.Size = new System.Drawing.Size(52, 24);
            this.btnMoreLess.TabIndex = 17;
            this.btnMoreLess.Text = "Less";
            this.btnMoreLess.UseVisualStyleBackColor = true;
            this.btnMoreLess.Click += new System.EventHandler(this.btnMoreLess_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnSaveMovieAs);
            this.groupBox3.Controls.Add(this.btnStop);
            this.groupBox3.Controls.Add(this.btnMoreLess);
            this.groupBox3.Controls.Add(this.btnPlayBackward);
            this.groupBox3.Controls.Add(this.btnPlayForward);
            this.groupBox3.Controls.Add(this.tbarFrameSelector);
            this.groupBox3.Location = new System.Drawing.Point(12, -2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(281, 77);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            // 
            // btnSaveMovieAs
            // 
            this.btnSaveMovieAs.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnSaveMovieAs.Location = new System.Drawing.Point(221, 22);
            this.btnSaveMovieAs.Name = "btnSaveMovieAs";
            this.btnSaveMovieAs.Size = new System.Drawing.Size(52, 24);
            this.btnSaveMovieAs.TabIndex = 19;
            this.btnSaveMovieAs.Text = "Save as";
            this.btnSaveMovieAs.UseVisualStyleBackColor = true;
            this.btnSaveMovieAs.Click += new System.EventHandler(this.btnSaveMovieAs_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::PrePoMax.Properties.Resources.Stop;
            this.btnStop.Location = new System.Drawing.Point(128, 22);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(24, 24);
            this.btnStop.TabIndex = 6;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlayBackward
            // 
            this.btnPlayBackward.Image = global::PrePoMax.Properties.Resources.Animate_backwards;
            this.btnPlayBackward.Location = new System.Drawing.Point(100, 22);
            this.btnPlayBackward.Name = "btnPlayBackward";
            this.btnPlayBackward.Size = new System.Drawing.Size(24, 24);
            this.btnPlayBackward.TabIndex = 4;
            this.btnPlayBackward.UseVisualStyleBackColor = true;
            this.btnPlayBackward.Click += new System.EventHandler(this.btnPlayBackward_Click);
            // 
            // btnPlayForward
            // 
            this.btnPlayForward.Image = global::PrePoMax.Properties.Resources.Animate;
            this.btnPlayForward.Location = new System.Drawing.Point(158, 22);
            this.btnPlayForward.Name = "btnPlayForward";
            this.btnPlayForward.Size = new System.Drawing.Size(24, 24);
            this.btnPlayForward.TabIndex = 7;
            this.btnPlayForward.UseVisualStyleBackColor = true;
            this.btnPlayForward.Click += new System.EventHandler(this.btnPlayForward_Click);
            // 
            // numCurrFrame
            // 
            this.numCurrFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numCurrFrame.Location = new System.Drawing.Point(145, 17);
            this.numCurrFrame.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numCurrFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCurrFrame.Name = "numCurrFrame";
            this.numCurrFrame.Size = new System.Drawing.Size(45, 23);
            this.numCurrFrame.TabIndex = 18;
            this.numCurrFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCurrFrame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCurrFrame.ValueChanged += new System.EventHandler(this.numCurrFrame_ValueChanged);
            // 
            // timerAnimation
            // 
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // gbColorSpectrumLimits
            // 
            this.gbColorSpectrumLimits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbColorSpectrumLimits.Controls.Add(this.rbLimitsCurrentFrame);
            this.gbColorSpectrumLimits.Controls.Add(this.rbLimitsAllFrames);
            this.gbColorSpectrumLimits.Location = new System.Drawing.Point(12, 297);
            this.gbColorSpectrumLimits.Name = "gbColorSpectrumLimits";
            this.gbColorSpectrumLimits.Size = new System.Drawing.Size(281, 51);
            this.gbColorSpectrumLimits.TabIndex = 19;
            this.gbColorSpectrumLimits.TabStop = false;
            this.gbColorSpectrumLimits.Text = "Automatic Color Spectrum Limits";
            // 
            // rbLimitsCurrentFrame
            // 
            this.rbLimitsCurrentFrame.AutoSize = true;
            this.rbLimitsCurrentFrame.Location = new System.Drawing.Point(6, 22);
            this.rbLimitsCurrentFrame.Name = "rbLimitsCurrentFrame";
            this.rbLimitsCurrentFrame.Size = new System.Drawing.Size(119, 19);
            this.rbLimitsCurrentFrame.TabIndex = 11;
            this.rbLimitsCurrentFrame.Text = "Use current frame";
            this.rbLimitsCurrentFrame.UseVisualStyleBackColor = true;
            this.rbLimitsCurrentFrame.CheckedChanged += new System.EventHandler(this.rbLimitChanged_CheckedChanged);
            // 
            // rbLimitsAllFrames
            // 
            this.rbLimitsAllFrames.AutoSize = true;
            this.rbLimitsAllFrames.Checked = true;
            this.rbLimitsAllFrames.Location = new System.Drawing.Point(176, 22);
            this.rbLimitsAllFrames.Name = "rbLimitsAllFrames";
            this.rbLimitsAllFrames.Size = new System.Drawing.Size(98, 19);
            this.rbLimitsAllFrames.TabIndex = 12;
            this.rbLimitsAllFrames.TabStop = true;
            this.rbLimitsAllFrames.Text = "Use all frames";
            this.rbLimitsAllFrames.UseVisualStyleBackColor = true;
            this.rbLimitsAllFrames.CheckedChanged += new System.EventHandler(this.rbLimitChanged_CheckedChanged);
            // 
            // numLastFrame
            // 
            this.numLastFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numLastFrame.Location = new System.Drawing.Point(228, 17);
            this.numLastFrame.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numLastFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLastFrame.Name = "numLastFrame";
            this.numLastFrame.Size = new System.Drawing.Size(45, 23);
            this.numLastFrame.TabIndex = 23;
            this.numLastFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numLastFrame.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numLastFrame.ValueChanged += new System.EventHandler(this.numLastFrame_ValueChanged);
            // 
            // lLast
            // 
            this.lLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lLast.AutoSize = true;
            this.lLast.Location = new System.Drawing.Point(197, 19);
            this.lLast.Name = "lLast";
            this.lLast.Size = new System.Drawing.Size(28, 15);
            this.lLast.TabIndex = 22;
            this.lLast.Text = "Last";
            // 
            // numFirstFrame
            // 
            this.numFirstFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numFirstFrame.Location = new System.Drawing.Point(41, 17);
            this.numFirstFrame.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numFirstFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFirstFrame.Name = "numFirstFrame";
            this.numFirstFrame.Size = new System.Drawing.Size(45, 23);
            this.numFirstFrame.TabIndex = 21;
            this.numFirstFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numFirstFrame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFirstFrame.ValueChanged += new System.EventHandler(this.numFirstFrame_ValueChanged);
            // 
            // lFirst
            // 
            this.lFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lFirst.AutoSize = true;
            this.lFirst.Location = new System.Drawing.Point(6, 19);
            this.lFirst.Name = "lFirst";
            this.lFirst.Size = new System.Drawing.Size(29, 15);
            this.lFirst.TabIndex = 20;
            this.lFirst.Text = "First";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.lFirst);
            this.groupBox5.Controls.Add(this.numLastFrame);
            this.groupBox5.Controls.Add(this.lCurrent);
            this.groupBox5.Controls.Add(this.numCurrFrame);
            this.groupBox5.Controls.Add(this.numFirstFrame);
            this.groupBox5.Controls.Add(this.lLast);
            this.groupBox5.Location = new System.Drawing.Point(12, 81);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(281, 46);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Frames";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnClose);
            this.groupBox6.Controls.Add(this.cbSaveAsImages);
            this.groupBox6.Controls.Add(this.cbEncoderOptions);
            this.groupBox6.Location = new System.Drawing.Point(11, 408);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(281, 73);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Movie Options";
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnClose.Location = new System.Drawing.Point(-100, -100);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(52, 24);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "CloseH";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cbSaveAsImages
            // 
            this.cbSaveAsImages.AutoSize = true;
            this.cbSaveAsImages.Location = new System.Drawing.Point(6, 47);
            this.cbSaveAsImages.Name = "cbSaveAsImages";
            this.cbSaveAsImages.Size = new System.Drawing.Size(105, 19);
            this.cbSaveAsImages.TabIndex = 1;
            this.cbSaveAsImages.Text = "Save as images";
            this.cbSaveAsImages.UseVisualStyleBackColor = true;
            this.cbSaveAsImages.CheckedChanged += new System.EventHandler(this.cbSaveAsImages_CheckedChanged);
            // 
            // cbEncoderOptions
            // 
            this.cbEncoderOptions.AutoSize = true;
            this.cbEncoderOptions.Location = new System.Drawing.Point(6, 22);
            this.cbEncoderOptions.Name = "cbEncoderOptions";
            this.cbEncoderOptions.Size = new System.Drawing.Size(201, 19);
            this.cbEncoderOptions.TabIndex = 0;
            this.cbEncoderOptions.Text = "Show video compression options";
            this.cbEncoderOptions.UseVisualStyleBackColor = true;
            this.cbEncoderOptions.CheckedChanged += new System.EventHandler(this.cbEncoderOptions_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbGraphicsRam);
            this.groupBox4.Location = new System.Drawing.Point(11, 354);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(281, 48);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Acceleration";
            // 
            // cbGraphicsRam
            // 
            this.cbGraphicsRam.AutoSize = true;
            this.cbGraphicsRam.Location = new System.Drawing.Point(6, 22);
            this.cbGraphicsRam.Name = "cbGraphicsRam";
            this.cbGraphicsRam.Size = new System.Drawing.Size(258, 19);
            this.cbGraphicsRam.TabIndex = 0;
            this.cbGraphicsRam.Text = "Use graphics card RAM (for smaller meshes)";
            this.cbGraphicsRam.UseVisualStyleBackColor = true;
            this.cbGraphicsRam.CheckedChanged += new System.EventHandler(this.cbGraphicsRam_CheckedChanged);
            // 
            // FrmAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(304, 486);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.gbColorSpectrumLimits);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAnimation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Animation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAnimation_FormClosing);
            this.Shown += new System.EventHandler(this.FrmAnimation_Shown);
            this.VisibleChanged += new System.EventHandler(this.FrmAnimation_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfAngles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIncrementStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumberOfFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarFrameSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFramesPerSecond)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCurrFrame)).EndInit();
            this.gbColorSpectrumLimits.ResumeLayout(false);
            this.gbColorSpectrumLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLastFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFirstFrame)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lNumberOfFrames;
        private System.Windows.Forms.RadioButton rbTimeIncrements;
        private System.Windows.Forms.NumericUpDown numNumberOfFrames;
        private System.Windows.Forms.RadioButton rbScaleFactor;
        private System.Windows.Forms.Button btnPlayBackward;
        private System.Windows.Forms.TrackBar tbarFrameSelector;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlayForward;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numFramesPerSecond;
        private System.Windows.Forms.RadioButton rbOnce;
        private System.Windows.Forms.RadioButton rbLoop;
        private System.Windows.Forms.RadioButton rbSwing;
        private System.Windows.Forms.Label lCurrent;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnMoreLess;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Timer timerAnimation;
        private System.Windows.Forms.NumericUpDown numCurrFrame;
        private System.Windows.Forms.GroupBox gbColorSpectrumLimits;
        private System.Windows.Forms.RadioButton rbLimitsCurrentFrame;
        private System.Windows.Forms.RadioButton rbLimitsAllFrames;
        private System.Windows.Forms.Button btnSaveMovieAs;
        private System.Windows.Forms.NumericUpDown numFirstFrame;
        private System.Windows.Forms.Label lFirst;
        private System.Windows.Forms.NumericUpDown numLastFrame;
        private System.Windows.Forms.Label lLast;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbEncoderOptions;
        private System.Windows.Forms.NumericUpDown numIncrementStep;
        private System.Windows.Forms.Label lIncrementStep;
        private System.Windows.Forms.CheckBox cbSaveAsImages;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbGraphicsRam;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lNumberOfAngles;
        private System.Windows.Forms.NumericUpDown numNumberOfAngles;
        private System.Windows.Forms.RadioButton rbHarmonic;
    }
}