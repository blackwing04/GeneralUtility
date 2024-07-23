using UtilityUseDemo.Models;

namespace UtilityUseDemo.Utils
{
    public static class ButtonStyleHelper
    {
        public static void UpdateButtonStyle(Button button, IButtonStyle buttonStyle, Label? label = null)
        {
            if (!button.Enabled) {
                // 不啟用狀態的樣式
                button.BackColor = SystemColors.Control;
                button.FlatAppearance.BorderColor = Color.FromArgb(169, 169, 169);
                button.ForeColor = SystemColors.GrayText; // 更改為灰色以增加可讀性
                if (label != null)
                    label.Visible = false;
            }
            else {
                button.BackColor = buttonStyle.BackColor;
                button.FlatAppearance.BorderColor = buttonStyle.BorderColor;
                button.ForeColor = buttonStyle.ForeColor;
                if (label != null)
                    label.Visible = true;
            }
        }
    }
}
