namespace UtilityUseDemo
{
    partial class Main
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
            if (disposing && (components != null)) {
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
            tlpConfig = new TableLayoutPanel();
            txtFilepath = new TextBox();
            lblFilepath = new Label();
            lblServerAddress = new Label();
            lblAuthentication = new Label();
            pnlPassword = new Panel();
            btnShowPassword = new Button();
            txtPassword = new TextBox();
            cbAuthentication = new ComboBox();
            txtPort = new TextBox();
            txtUserId = new TextBox();
            txtServerAddress = new TextBox();
            lblPort = new Label();
            lblUserId = new Label();
            lblPassword = new Label();
            btnConnect = new Button();
            lblConnectSuccess = new Label();
            btnImport = new Button();
            btnExport = new Button();
            tlpConfig.SuspendLayout();
            pnlPassword.SuspendLayout();
            SuspendLayout();
            // 
            // tlpConfig
            // 
            tlpConfig.AutoSize = true;
            tlpConfig.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlpConfig.ColumnCount = 4;
            tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpConfig.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpConfig.Controls.Add(txtFilepath, 0, 6);
            tlpConfig.Controls.Add(lblFilepath, 0, 6);
            tlpConfig.Controls.Add(lblServerAddress, 0, 1);
            tlpConfig.Controls.Add(lblAuthentication, 0, 2);
            tlpConfig.Controls.Add(pnlPassword, 1, 4);
            tlpConfig.Controls.Add(cbAuthentication, 1, 2);
            tlpConfig.Controls.Add(txtPort, 3, 1);
            tlpConfig.Controls.Add(txtUserId, 1, 3);
            tlpConfig.Controls.Add(txtServerAddress, 1, 1);
            tlpConfig.Controls.Add(lblPort, 2, 1);
            tlpConfig.Controls.Add(lblUserId, 0, 3);
            tlpConfig.Controls.Add(lblPassword, 0, 4);
            tlpConfig.Controls.Add(btnConnect, 2, 3);
            tlpConfig.Controls.Add(lblConnectSuccess, 2, 4);
            tlpConfig.Controls.Add(btnImport, 2, 6);
            tlpConfig.Controls.Add(btnExport, 3, 6);
            tlpConfig.Dock = DockStyle.Fill;
            tlpConfig.Location = new Point(0, 0);
            tlpConfig.MinimumSize = new Size(640, 310);
            tlpConfig.Name = "tlpConfig";
            tlpConfig.RowCount = 7;
            tlpConfig.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            tlpConfig.RowStyles.Add(new RowStyle());
            tlpConfig.RowStyles.Add(new RowStyle());
            tlpConfig.RowStyles.Add(new RowStyle());
            tlpConfig.RowStyles.Add(new RowStyle());
            tlpConfig.RowStyles.Add(new RowStyle());
            tlpConfig.RowStyles.Add(new RowStyle());
            tlpConfig.Size = new Size(784, 361);
            tlpConfig.TabIndex = 15;
            // 
            // txtFilepath
            // 
            txtFilepath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtFilepath.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            txtFilepath.Location = new Point(277, 305);
            txtFilepath.Margin = new Padding(3, 10, 3, 10);
            txtFilepath.Name = "txtFilepath";
            txtFilepath.Size = new Size(268, 35);
            txtFilepath.TabIndex = 17;
            // 
            // lblFilepath
            // 
            lblFilepath.AutoSize = true;
            lblFilepath.Dock = DockStyle.Fill;
            lblFilepath.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lblFilepath.Location = new Point(3, 284);
            lblFilepath.Name = "lblFilepath";
            lblFilepath.Size = new Size(268, 77);
            lblFilepath.TabIndex = 16;
            lblFilepath.Text = "Filepath";
            lblFilepath.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblServerAddress
            // 
            lblServerAddress.AutoSize = true;
            lblServerAddress.Dock = DockStyle.Fill;
            lblServerAddress.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerAddress.ForeColor = SystemColors.ControlText;
            lblServerAddress.Location = new Point(0, 55);
            lblServerAddress.Margin = new Padding(0);
            lblServerAddress.Name = "lblServerAddress";
            lblServerAddress.Size = new Size(274, 55);
            lblServerAddress.TabIndex = 9;
            lblServerAddress.Text = "ServerAddress";
            lblServerAddress.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAuthentication
            // 
            lblAuthentication.AutoSize = true;
            lblAuthentication.Dock = DockStyle.Fill;
            lblAuthentication.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lblAuthentication.Location = new Point(3, 110);
            lblAuthentication.Name = "lblAuthentication";
            lblAuthentication.Size = new Size(268, 54);
            lblAuthentication.TabIndex = 11;
            lblAuthentication.Text = "Authentication";
            lblAuthentication.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlPassword
            // 
            pnlPassword.Controls.Add(btnShowPassword);
            pnlPassword.Controls.Add(txtPassword);
            pnlPassword.Dock = DockStyle.Fill;
            pnlPassword.Location = new Point(277, 227);
            pnlPassword.Name = "pnlPassword";
            pnlPassword.Size = new Size(268, 54);
            pnlPassword.TabIndex = 5;
            pnlPassword.TabStop = true;
            // 
            // btnShowPassword
            // 
            btnShowPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnShowPassword.BackColor = SystemColors.Window;
            btnShowPassword.FlatAppearance.BorderSize = 0;
            btnShowPassword.FlatStyle = FlatStyle.Flat;
            btnShowPassword.Font = new Font("Microsoft JhengHei UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            btnShowPassword.ForeColor = Color.Black;
            btnShowPassword.Image = Resources.Img.eye_off;
            btnShowPassword.Location = new Point(242, 8);
            btnShowPassword.Name = "btnShowPassword";
            btnShowPassword.Size = new Size(23, 32);
            btnShowPassword.TabIndex = 2;
            btnShowPassword.TabStop = false;
            btnShowPassword.UseVisualStyleBackColor = false;
            btnShowPassword.Visible = false;
            btnShowPassword.Click += BtnShowPassword_Click;
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPassword.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            txtPassword.Location = new Point(0, 7);
            txtPassword.Margin = new Padding(3, 10, 3, 10);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(265, 35);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.TextChanged += TxtPassword_TextChanged;
            // 
            // cbAuthentication
            // 
            cbAuthentication.Dock = DockStyle.Fill;
            cbAuthentication.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            cbAuthentication.FormattingEnabled = true;
            cbAuthentication.Items.AddRange(new object[] { "Windows", "Sql Server", "Sqlite" });
            cbAuthentication.Location = new Point(277, 120);
            cbAuthentication.Margin = new Padding(3, 10, 3, 10);
            cbAuthentication.Name = "cbAuthentication";
            cbAuthentication.Size = new Size(268, 34);
            cbAuthentication.TabIndex = 3;
            cbAuthentication.SelectedIndexChanged += CbAuthentication_SelectedIndexChanged;
            // 
            // txtPort
            // 
            txtPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtPort.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            txtPort.Location = new Point(668, 65);
            txtPort.Margin = new Padding(3, 10, 3, 10);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(90, 35);
            txtPort.TabIndex = 2;
            txtPort.TextChanged += TxtPort_TextChanged;
            // 
            // txtUserId
            // 
            txtUserId.Dock = DockStyle.Fill;
            txtUserId.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            txtUserId.Location = new Point(277, 174);
            txtUserId.Margin = new Padding(3, 10, 3, 10);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(268, 35);
            txtUserId.TabIndex = 4;
            txtUserId.TextChanged += TxtUserId_TextChanged;
            // 
            // txtServerAddress
            // 
            txtServerAddress.Dock = DockStyle.Fill;
            txtServerAddress.Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            txtServerAddress.Location = new Point(277, 65);
            txtServerAddress.Margin = new Padding(3, 10, 3, 10);
            txtServerAddress.Name = "txtServerAddress";
            txtServerAddress.Size = new Size(268, 35);
            txtServerAddress.TabIndex = 1;
            txtServerAddress.TextChanged += TxtServerAddress_TextChanged;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Dock = DockStyle.Fill;
            lblPort.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lblPort.Location = new Point(551, 55);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(111, 55);
            lblPort.TabIndex = 10;
            lblPort.Text = "Port";
            lblPort.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblUserId
            // 
            lblUserId.AutoSize = true;
            lblUserId.Dock = DockStyle.Fill;
            lblUserId.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lblUserId.Location = new Point(3, 164);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(268, 60);
            lblUserId.TabIndex = 12;
            lblUserId.Text = "UserId";
            lblUserId.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Dock = DockStyle.Fill;
            lblPassword.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lblPassword.Location = new Point(3, 224);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(268, 60);
            lblPassword.TabIndex = 13;
            lblPassword.Text = "Password";
            lblPassword.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(61, 139, 205);
            btnConnect.FlatAppearance.BorderColor = Color.Black;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(551, 174);
            btnConnect.Margin = new Padding(3, 10, 3, 10);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(87, 40);
            btnConnect.TabIndex = 6;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += BtnConnect_Click;
            // 
            // lblConnectSuccess
            // 
            lblConnectSuccess.Anchor = AnchorStyles.Left;
            lblConnectSuccess.AutoSize = true;
            tlpConfig.SetColumnSpan(lblConnectSuccess, 2);
            lblConnectSuccess.Font = new Font("Microsoft JhengHei UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            lblConnectSuccess.ForeColor = Color.FromArgb(102, 204, 153);
            lblConnectSuccess.Location = new Point(551, 236);
            lblConnectSuccess.Name = "lblConnectSuccess";
            lblConnectSuccess.Size = new Size(225, 35);
            lblConnectSuccess.TabIndex = 14;
            lblConnectSuccess.Text = "ConnectSuccess";
            lblConnectSuccess.TextAlign = ContentAlignment.MiddleLeft;
            lblConnectSuccess.Visible = false;
            // 
            // btnImport
            // 
            btnImport.Anchor = AnchorStyles.None;
            btnImport.BackColor = Color.FromArgb(102, 204, 153);
            btnImport.FlatAppearance.BorderColor = Color.Black;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnImport.ForeColor = Color.White;
            btnImport.Location = new Point(561, 302);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(90, 40);
            btnImport.TabIndex = 7;
            btnImport.Text = "Import";
            btnImport.UseVisualStyleBackColor = false;
            btnImport.Click += BtnImport_Click;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.None;
            btnExport.BackColor = Color.Orange;
            btnExport.FlatAppearance.BorderColor = Color.Black;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnExport.ForeColor = Color.White;
            btnExport.Location = new Point(679, 302);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(90, 40);
            btnExport.TabIndex = 15;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += BtnExport_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(784, 361);
            Controls.Add(tlpConfig);
            Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            MaximumSize = new Size(800, 400);
            MinimumSize = new Size(800, 400);
            Name = "Main";
            Text = "Main";
            tlpConfig.ResumeLayout(false);
            tlpConfig.PerformLayout();
            pnlPassword.ResumeLayout(false);
            pnlPassword.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tlpConfig;
        private Label lblServerAddress;
        private Label lblConnectSuccess;
        private Label lblAuthentication;
        private Panel pnlPassword;
        private Button btnShowPassword;
        private TextBox txtPassword;
        private ComboBox cbAuthentication;
        private Button btnConnect;
        private TextBox txtPort;
        private TextBox txtUserId;
        private TextBox txtServerAddress;
        private Label lblPort;
        private Label lblUserId;
        private Label lblPassword;
        private Button btnImport;
        private Button btnExport;
        private TextBox txtFilepath;
        private Label lblFilepath;
    }
}