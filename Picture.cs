using System.Drawing;
using System.Drawing.Imaging;

namespace images
{
    public class Picture
    {
        private Bitmap Img;
        private string FilePath;

        public Picture(string filePath)
        { 
            FilePath = filePath;
            Img = new Bitmap(filePath);
        }

        public string GetPicturePath()
        {
            return this.FilePath;
        }
        public Bitmap GetPictureImg()
        {
            return this.Img;
        }

        public Picture ConvertToGif()
        {
            string[] splitPath = this.FilePath.Split('.');
            string newPath = splitPath[0] + "1.gif";
            this.Img.Save(newPath, ImageFormat.Gif);
            Picture gifPicture = new Picture(newPath);
            return gifPicture; 
        }       
    }
}
