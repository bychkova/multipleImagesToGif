using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;


namespace images
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        Manager manager = new Manager();

        public void CreateGif(Manager manager, string savePath)
        {
            ImageCodecInfo gifEncroder = manager.GetCodecInfo();

            List<Picture> converted = manager.gifList();

            Bitmap animatedGif = converted[0].GetPictureImg();

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                int FrameDelay = 0x5100; //PropertyTagFrameDelay
                int Frameloop = 0x5101; // PropertyTagLoopCount
                int unitBytes = 4;

                PropertyItem frameDelay = animatedGif.GetPropertyItem(FrameDelay);
                PropertyItem loopPropertyItem = animatedGif.GetPropertyItem(Frameloop);

                var encoderParamFirst = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame) }
                };
                
                frameDelay.Len = 3 * unitBytes;
                frameDelay.Value = new byte[3 * unitBytes];
                loopPropertyItem.Value = BitConverter.GetBytes((ushort)0);

                animatedGif.SetPropertyItem(frameDelay);
                animatedGif.SetPropertyItem(loopPropertyItem);

                animatedGif.Save(stream, gifEncroder, encoderParamFirst);

                var encoderParamRest = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime) }
                };

                for (int i = 1; i < converted.Count; i++)
                {
                    animatedGif.SaveAdd(converted[i].GetPictureImg(), encoderParamRest);
                }

                var encoderParamsFlush = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush) }
                };

                animatedGif.SaveAdd(encoderParamsFlush);
                finalPicture.Image = animatedGif;
                stream.Close();
            }

            for (int i = 0; i < converted.Count; i++)
            {
                converted[i].GetPictureImg().Dispose();
                File.Delete(converted[i].GetPicturePath());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() {Multiselect = true, Title = "Choose img"};
            ofd.Filter = "images(*.png; *.jpeg; *.jpg) | *.png; *.jpeg; *.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                btnSave.Enabled = true;
                foreach (string name in ofd.FileNames)
                {
                    Picture picture = new Picture(name);
                    manager.AddPicture(picture);
                    GroupBox pct1 = new GroupBox();
                    pct1.Size = new Size(150, 150);
                    flowLayoutPanel1.Controls.Add(pct1);

                    PictureBox PictureBox1 = new PictureBox();
                    PictureBox1.Location = new Point(10, 13);
                    PictureBox1.Size = new Size(130, 130);
                    PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                    pct1.Controls.Add(PictureBox1);

                    PictureBox1.Image = picture.GetPictureImg();                    
                }                
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "images(*.gif) | *.gif";
            string path = "";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                path = sfd.FileName;
                CreateGif(manager, path);
                finalPicture.Image = new Bitmap(path);
                btnAdd.Enabled = false;
                btnSave.Enabled = false;
                btnClear.Text = "новая gif";
            }                       
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            finalPicture.Image = null;
            manager.ClearPictureList();
            btnClear.Text = "очистить";
            btnAdd.Enabled = true;
            btnSave.Enabled = false;
        }
    }
}
