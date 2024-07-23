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
        #region 建構式
        private readonly IDatabaseDAO _databaseDAO;
        /// <summary>
        /// 表示是否連接成功
        /// </summary>
        public bool ConnectTask { get; private set; }
        public Main(IDatabaseDAO databaseDAO)
        {
            _databaseDAO = databaseDAO;
            InitializeComponent();
            // 設定按鈕初始樣式
            ButtonStyleHelper.UpdateButtonStyle(btnExport, new ButtonOrange(), lblConnectSuccess);
            // 註冊 EnabledChanged 事件
            btnExport.EnabledChanged += (sender, e) => {
                ButtonStyleHelper.UpdateButtonStyle(btnExport, new ButtonOrange(), lblConnectSuccess);
            };
            // 設定按鈕初始樣式
            ButtonStyleHelper.UpdateButtonStyle(btnImport, new ButtonGreen(), lblConnectSuccess);
            // 註冊 EnabledChanged 事件
            btnImport.EnabledChanged += (sender, e) => {
                ButtonStyleHelper.UpdateButtonStyle(btnImport, new ButtonGreen(), lblConnectSuccess);
            };
            //初始化語言
            LocalizeComponent();
            ConnectTask = CheckConnectRecoverConnectString();
        }
        #endregion 建構式
        #region DAO使用範例
        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServerAddress.Text)) {
                MsgBoxHelper.ShowError("MessageServerAddressBlankError");
                return;
            }
            try {
                //建立連線字串
                StringBuilder sb = new ();
                DbTypeEnum dbType = DbTypeEnum.MSSQL;
                sb.Append($"Server={txtServerAddress.Text}");
                if (!string.IsNullOrEmpty(txtPort.Text))
                    sb.Append($",{txtPort.Text}");
                if (cbAuthentication.SelectedIndex == 0)
                    sb.Append(";Trusted_Connection=True;");// Windows 身份驗證
                else if (cbAuthentication.SelectedIndex == 1)
                    // SQL Server 身份驗證
                    sb.Append($";Trusted_Connection=false;User Id={txtUserId.Text};Password={txtPassword.Text}");
                else {
                    // SQLite 的情況
                    sb.Clear();  // 清除前面的構建，因為 SQLite 不需要服務器或端口
                    sb.Append($"Data Source={txtServerAddress.Text};Mode=ReadWriteCreate");
                    dbType = DbTypeEnum.SQLite;
                }
                /*
                 * 切換資料庫，這邊可以依據不同資料庫或引擎做切換
                 * 即使是初始就注入的服務也可以做切換
                 */
                var newCtypto = CryptoHelper.CreateEncryptModel(sb.ToString());
                newCtypto.DatabaseType = dbType;
                _databaseDAO.ChangeDatabaseConnection(newCtypto);
                var testConnect = await _databaseDAO.OpenConnectionAsync();
                //判斷能否連接以及是否有足夠的權限
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
        #endregion DAO使用範例
        #region 匯入匯出
        private void BtnImport_Click(object sender, EventArgs e)
        {

        }

        private void BtnExport_Click(object sender, EventArgs e)
        {

        }
        #endregion 匯入匯出
        #region 其他方法
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
                    Title = "選擇 SQLite 檔案",
                    CheckFileExists = false // 允許選擇不存在的檔案，讓程式自己創建
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

        //以下都是防呆，使用者改變輸入內容都需要重新連接。
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
        /// 從登錄檔還原連接字串，並帶入到UI介面
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
            // 將連接字串分割成多個部分
            var parts = conString.Split(';');

            foreach (var part in parts) {
                // 進一步將每部分分割成鍵和值
                var keyValue = part.Split('=');
                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                switch (key) {
                    case "Server":
                        // 處理伺服器地址和端口（如果有）
                        var serverParts = value.Split(',');
                        txtServerAddress.Text = serverParts[0];
                        if (serverParts.Length > 1)
                            txtPort.Text = serverParts[1];
                        break;
                    case "Trusted_Connection":
                        // 設定身份驗證方式
                        if (value.Equals("True", StringComparison.OrdinalIgnoreCase))
                            cbAuthentication.SelectedIndex = 0; // Windows 身份驗證
                        else
                            cbAuthentication.SelectedIndex = 1; // SQL Server 身份驗證
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

        #endregion 其他方法
        #region 語言方法
        /// <summary>
        /// 獲取所有元件，更新元件顯示的語言
        /// </summary>
        public void LocalizeComponent()
        {
            // 更新窗體上所有的控件，並使用資源文件更新它們的顯示文本
            foreach (Control c in Controls) {
                UpdateControlLang(c);
            }
        }
        /// <summary>
        /// 更新元件語言文本
        /// </summary>
        ///  <param name="control">頁面內組件</param>
        public void UpdateControlLang(Control control)
        {
            //尋找語系文本
            string resourceValue=LangHelper.T(control.Name);
            // 如果找到了資源，則為控件設置文本
            if (!string.IsNullOrEmpty(resourceValue)) {
                if (control is Label)
                    CenterLabelText(control, resourceValue);
                else
                    control.Text = resourceValue;
            }
            // 如果控件是容器類型，進行遞迴
            if (control.HasChildren) {
                foreach (Control childControl in control.Controls) {
                    UpdateControlLang(childControl);
                }
            }
        }
        /// <summary>
        /// 改變語言後設定標籤位置置中
        /// </summary>
        ///  <param name="label">目標標籤元件</param>
        ///  <param name="text">新的文字</param>
        private static void CenterLabelText(Control label, string text)
        {
            // 設置 Label 的文字
            label.Text = text;

            // 確保 label 有一個非 null 的父容器
            if (label.Parent != null) {
                // 使用 Label 的當前字體建立一個 Graphics 對象
                using Graphics g = label.CreateGraphics();
                // 測量文字的大小
                SizeF textSize = g.MeasureString(text, label.Font);

                // 計算文字水平居中的新 X 座標
                int newX = (label.Parent.Width - (int)textSize.Width) / 2;

                // 更新 Label 的位置
                label.Location = new Point(newX, label.Location.Y);
            }
        }
        #endregion 語言方法

    }
}