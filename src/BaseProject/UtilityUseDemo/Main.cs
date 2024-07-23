using DAO.Services;
using DAO.StaticUtil;
using DAO.StaticUtil.Enums;
using System.Text;
using UtilityUseDemo.Models;
using UtilityUseDemo.Utils;

namespace UtilityUseDemo
{
    public partial class Main : Form
    {
        #region �غc��
        private readonly IDatabaseDAO _databaseDAO;
        /// <summary>
        /// ��ܬO�_�s�����\
        /// </summary>
        public bool ConnectTask { get; private set; }
        public Main(IDatabaseDAO databaseDAO)
        {
            _databaseDAO = databaseDAO;
            InitializeComponent();
            // �]�w���s��l�˦�
            ButtonStyleHelper.UpdateButtonStyle(btnExport, new ButtonOrange(), lblConnectSuccess);
            // ���U EnabledChanged �ƥ�
            btnExport.EnabledChanged += (sender, e) => {
                ButtonStyleHelper.UpdateButtonStyle(btnExport, new ButtonOrange(), lblConnectSuccess);
            };
            // �]�w���s��l�˦�
            ButtonStyleHelper.UpdateButtonStyle(btnImport, new ButtonGreen(), lblConnectSuccess);
            // ���U EnabledChanged �ƥ�
            btnImport.EnabledChanged += (sender, e) => {
                ButtonStyleHelper.UpdateButtonStyle(btnImport, new ButtonGreen(), lblConnectSuccess);
            };
            //��l�ƻy��
            LocalizeComponent();
            ConnectTask = CheckConnectRecoverConnectString();
        }
        #endregion �غc��
        #region DAO�ϥνd��
        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServerAddress.Text)) {
                MsgBoxHelper.ShowError("MessageServerAddressBlankError");
                return;
            }
            try {
                //�إ߳s�u�r��
                StringBuilder sb = new ();
                DbTypeEnum dbType = DbTypeEnum.MSSQL;
                sb.Append($"Server={txtServerAddress.Text}");
                if (!string.IsNullOrEmpty(txtPort.Text))
                    sb.Append($",{txtPort.Text}");
                if (cbAuthentication.SelectedIndex == 0)
                    sb.Append(";Trusted_Connection=True;");// Windows ��������
                else if (cbAuthentication.SelectedIndex == 1)
                    // SQL Server ��������
                    sb.Append($";Trusted_Connection=false;User Id={txtUserId.Text};Password={txtPassword.Text}");
                else {
                    // SQLite �����p
                    sb.Clear();  // �M���e�����c�ءA�]�� SQLite ���ݭn�A�Ⱦ��κݤf
                    sb.Append($"Data Source={txtServerAddress.Text};Mode=ReadWriteCreate");
                    dbType = DbTypeEnum.SQLite;
                }
                /*
                 * ������Ʈw�A�o��i�H�̾ڤ��P��Ʈw�Τ���������
                 * �Y�ϬO��l�N�`�J���A�Ȥ]�i�H������
                 */
                var newCtypto = CryptoHelper.CreateEncryptModel(sb.ToString());
                newCtypto.DatabaseType = dbType;
                _databaseDAO.ChangeDatabaseConnection(newCtypto);
                var testConnect = await _databaseDAO.OpenConnectionAsync();
                //�P�_��_�s���H�άO�_���������v��
                if (!testConnect.IsDbConnected) {
                    MessageBox.Show(LangHelper.T("ConnectFailed"), LangHelper.T("Error")
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GlobalVariables.ConnectionStringFromRegistry = string.Empty;
                    GlobalVariables.ConnectionString = string.Empty;
                    btnExport.Enabled = false;
                    btnImport.Enabled = false;
                }
                else {
                    btnImport.Enabled = true;
                    btnExport.Enabled = true;
                }
            }
            catch {
                MessageBox.Show(LangHelper.T("ConnectFailed"), LangHelper.T("Error")
                  , MessageBoxButtons.OK, MessageBoxIcon.Error);
                GlobalVariables.ConnectionStringFromRegistry = string.Empty;
                GlobalVariables.ConnectionString = string.Empty;
                btnExport.Enabled = false;
                btnImport.Enabled = false;
            }
        }
        #endregion DAO�ϥνd��
        #region �פJ�ץX
        private void BtnImport_Click(object sender, EventArgs e)
        {

        }

        private void BtnExport_Click(object sender, EventArgs e)
        {

        }
        #endregion �פJ�ץX
        #region ��L��k
        private void CbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            btnImport.Enabled = false;
            if (cbAuthentication.SelectedIndex == 1) {
                txtUserId.Enabled = true;
                txtPassword.Enabled = true;
                txtUserId.TabIndex = 4;
                txtPassword.TabIndex = 5;
                btnShowPassword.Visible = true;
            }
            else {
                txtUserId.Enabled = false;
                txtPassword.Enabled = false;
                btnShowPassword.Visible = false;
            }

            if (cbAuthentication.SelectedIndex == 2) {
                OpenFileDialog openFileDialog = new() {
                    Filter = "SQLite Files (*.db;*.sqlite)|*.db;*.sqlite|All files (*.*)|*.*",
                    Title = "��� SQLite �ɮ�",
                    CheckFileExists = false // ���\��ܤ��s�b���ɮסA���{���ۤv�Ы�
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string filePath = openFileDialog.FileName;
                    txtServerAddress.Text = filePath;
                }
            }
        }
        private void BtnShowPassword_Click(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar) {
                txtPassword.UseSystemPasswordChar = false;
                btnShowPassword.Image = ImgHelper.GetImage("eye");
            }
            else {
                txtPassword.UseSystemPasswordChar = true;
                btnShowPassword.Image = ImgHelper.GetImage("eye_off");
            }
        }

        //�H�U���O���b�A�ϥΪ̧��ܿ�J���e���ݭn���s�s���C
        private void TxtServerAddress_TextChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            btnImport.Enabled = false;
        }

        private void TxtPort_TextChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            btnImport.Enabled = false;
        }

        private void TxtUserId_TextChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            btnImport.Enabled = false;
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            btnImport.Enabled = false;
        }
        /// <summary>
        /// �q�n�����٭�s���r��A�ña�J��UI����
        /// </summary>
        public bool CheckConnectRecoverConnectString()
        {
            string conString=GlobalVariables.ConnectionString;
            bool connect;
            try {
                connect = !string.IsNullOrEmpty(conString);
            }
            catch { connect = false; }
            if (!connect) {
                btnExport.Enabled = false;
                btnImport.Enabled = false;
                return connect;
            }
            // �N�s���r����Φ��h�ӳ���
            var parts = conString.Split(';');

            foreach (var part in parts) {
                // �i�@�B�N�C�������Φ���M��
                var keyValue = part.Split('=');
                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                switch (key) {
                    case "Server":
                        // �B�z���A���a�}�M�ݤf�]�p�G���^
                        var serverParts = value.Split(',');
                        txtServerAddress.Text = serverParts[0];
                        if (serverParts.Length > 1)
                            txtPort.Text = serverParts[1];
                        break;
                    case "Trusted_Connection":
                        // �]�w�������Ҥ覡
                        if (value.Equals("True", StringComparison.OrdinalIgnoreCase))
                            cbAuthentication.SelectedIndex = 0; // Windows ��������
                        else
                            cbAuthentication.SelectedIndex = 1; // SQL Server ��������
                        break;
                    case "User Id":
                        txtUserId.Text = value;
                        break;
                    case "Password":
                        txtPassword.Text = value;
                        break;
                }
            }
            btnImport.Enabled = true;
            btnExport.Enabled = true;
            return connect;
        }

        #endregion ��L��k
        #region �y����k
        /// <summary>
        /// ����Ҧ�����A��s������ܪ��y��
        /// </summary>
        public void LocalizeComponent()
        {
            // ��s����W�Ҧ�������A�èϥθ귽����s���̪���ܤ奻
            foreach (Control c in Controls) {
                UpdateControlLang(c);
            }
        }
        /// <summary>
        /// ��s����y���奻
        /// </summary>
        ///  <param name="control">�������ե�</param>
        public void UpdateControlLang(Control control)
        {
            //�M��y�t�奻
            string resourceValue=LangHelper.T(control.Name);
            // �p�G���F�귽�A�h������]�m�奻
            if (!string.IsNullOrEmpty(resourceValue)) {
                if (control is Label)
                    CenterLabelText(control, resourceValue);
                else
                    control.Text = resourceValue;
            }
            // �p�G����O�e�������A�i�滼�j
            if (control.HasChildren) {
                foreach (Control childControl in control.Controls) {
                    UpdateControlLang(childControl);
                }
            }
        }
        /// <summary>
        /// ���ܻy����]�w���Ҧ�m�m��
        /// </summary>
        ///  <param name="label">�ؼм��Ҥ���</param>
        ///  <param name="text">�s����r</param>
        private static void CenterLabelText(Control label, string text)
        {
            // �]�m Label ����r
            label.Text = text;

            // �T�O label ���@�ӫD null �����e��
            if (label.Parent != null) {
                // �ϥ� Label ����e�r��إߤ@�� Graphics ��H
                using Graphics g = label.CreateGraphics();
                // ���q��r���j�p
                SizeF textSize = g.MeasureString(text, label.Font);

                // �p���r�����~�����s X �y��
                int newX = (label.Parent.Width - (int)textSize.Width) / 2;

                // ��s Label ����m
                label.Location = new Point(newX, label.Location.Y);
            }
        }
        #endregion �y����k

    }
}