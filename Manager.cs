
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace images
{
    public class Manager
    {
        List<Picture> PictureList = new List<Picture>();
        ImageCodecInfo GifEncroder = null;

        public void SetCodecInfo()
        {
            foreach (ImageCodecInfo item in ImageCodecInfo.GetImageEncoders())
            {
                if (item.MimeType == "image/gif")
                {
                    GifEncroder = item;
                    break;
                }
            }
        }

        public ImageCodecInfo GetCodecInfo()
        {
            this.SetCodecInfo();
            return this.GifEncroder;
        }

        public void AddPicture(Picture picture)
        {
            this.PictureList.Add(picture);
        }

        public List<Picture> gifList()
        {
            List<Picture> gifList = new List<Picture>();

            foreach (Picture picture in this.PictureList)
            {
                gifList.Add(picture.ConvertToGif());
            }
   

            return gifList;
        }

        public void ClearPictureList()
        {
            this.PictureList.Clear();
        }
    }       
}
