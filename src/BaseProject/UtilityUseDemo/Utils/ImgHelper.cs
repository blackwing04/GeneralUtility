using System.Reflection;
using System.Resources;

namespace UtilityUseDemo.Utils
{
    public class ImgHelper
    {
        /// <summary>
        /// 回傳對應的資源圖檔
        /// </summary>
        ///  <param name="imageName">圖片的名稱</param>
        /// <returns>如果找到對應圖檔，則回傳圖檔；否則回傳Null。</returns>
        public static Image? GetImage(string imageName)
        {
            try {
                string resourcePath = $"DBTemplateTool.Resources.Img";
                var resourceManager = new ResourceManager(resourcePath, Assembly.GetExecutingAssembly());
                var imageObj = resourceManager.GetObject(imageName);
                return imageObj as Image;
            }
            catch {
                return null;
            }
        }
        /// <summary>
        /// 回傳對應的資源圖示
        /// </summary>
        ///  <param name="iconName">圖示的名稱</param>
        /// <returns>如果找到對應圖檔，則回傳圖檔；否則回傳Null。</returns>
        public static Icon? GetIcon(string iconName)
        {
            try {
                string resourcePath = $"DBTemplateTool.Resources.Img";
                var resourceManager = new ResourceManager(resourcePath, Assembly.GetExecutingAssembly());
                var iconObj = resourceManager.GetObject(iconName);
                return iconObj as Icon;
            }
            catch {
                return null;
            }
        }
    }
}
