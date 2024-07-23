
namespace UtilityUseDemo.Models
{
    public interface IButtonStyle
    {
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Color ForeColor { get; set; }
    }
    public class ButtonGreen : IButtonStyle
    {
        public Color BackColor { get; set; } = Color.FromArgb(102, 204, 153);
        public Color BorderColor { get; set; }= Color.Black;
        public Color ForeColor { get; set; } = Color.White;
    }
    public class ButtonOrange : IButtonStyle
    {
        public Color BackColor { get; set; } = Color.Orange;
        public Color BorderColor { get; set; } = Color.Black;
        public Color ForeColor { get; set; } = Color.White;
    }
}
