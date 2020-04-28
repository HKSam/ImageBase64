using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Base64Image
{
    public partial class Form1 : Form
    {
        private string curFileName;
        public Form1()
        {
            InitializeComponent();
            this.richTextBox.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToBase64(this.pictureBox1.Image, e);
            this.pictureBox1.Hide();
            this.richTextBox.Show();
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Multiselect = true;
            //dlg.Title = "选择要转换的图片";
            //dlg.Filter = "Image files (*.jpg;*.bmp;*.gif;*.png)|*.jpg*.jpeg;*.gif;*.bmp|AllFiles (*.*)|*.*";
            //if (DialogResult.OK == dlg.ShowDialog())
            //{
            //    for (int i = 0; i < dlg.FileNames.Length; i++)
            //    {
            //        ImgToBase64String(dlg.FileNames[i].ToString());
            //    }
            //}
        }

        //图片 转为    base64编码的文本
        private void ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                this.pictureBox1.Image = bmp;
                FileStream fs = new FileStream(Imagefilename + ".txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                sw.Write(strbaser64);

                sw.Close();
                fs.Close();
                // MessageBox.Show("转换成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImgToBase64String 转换失败\nException:" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ToImage(this.richTextBox.Text, e);
            this.pictureBox1.Show();
            this.richTextBox.Hide();
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Multiselect = true;
            //dlg.Title = "选择要转换的base64编码的文本";
            //dlg.Filter = "txt files|*.txt";
            //if (DialogResult.OK == dlg.ShowDialog())
            //{
            //    for (int i = 0; i < dlg.FileNames.Length; i++)
            //    {
            //        Base64StringToImage(dlg.FileNames[i].ToString());
            //    }

            //}
        }

        //base64编码的文本 转为    图片
        private void Base64StringToImage(string txtFileName)
        {
            try
            {
                FileStream ifs = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(ifs);

                String inputStr = sr.ReadToEnd();
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                //bmp.Save(txtFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(txtFileName + ".bmp", ImageFormat.Bmp);
                //bmp.Save(txtFileName + ".gif", ImageFormat.Gif);
                //bmp.Save(txtFileName + ".png", ImageFormat.Png);
                ms.Close();
                sr.Close();
                ifs.Close();
                this.pictureBox1.Image = bmp;
                if (File.Exists(txtFileName))
                {
                    File.Delete(txtFileName);
                }
                //MessageBox.Show("转换成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Base64StringToImage 转换失败\nException：" + ex.Message);
            }
        }

        private void ToBase64(object sender, EventArgs e)
        {
            Image img = this.pictureBox1.Image;
            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            binFormatter.Serialize(memStream, img);
            byte[] bytes = memStream.GetBuffer();
            string base64 = Convert.ToBase64String(bytes);
            this.richTextBox.Text = base64;
        }

        /// <summary>
        /// 将Base64字符串转换为图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToImage(object sender, EventArgs e)
        {
            string base64 = this.richTextBox.Text;
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            BinaryFormatter binFormatter = new BinaryFormatter();
            Image img = (Image)binFormatter.Deserialize(memStream);
            this.pictureBox1.Image = img;
        }

        /// <summary>
        /// 修改图片的大小
        /// </summary>
        /// <param name="b"></param>
        /// <param name="destHeight"></param>
        /// <param name="destWidth"></param>
        /// <returns></returns>
        public Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
        {
            System.Drawing.Image imgSource = b;
            System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
            int sW = 0, sH = 0;
            // 按比例缩放           
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            if (sHeight > destHeight || sWidth > destWidth)
            {
                if ((sWidth * destHeight) > (sHeight * destWidth))
                {
                    sW = destWidth;
                    sH = (destWidth * sHeight) / sWidth;
                }
                else
                {
                    sH = destHeight;
                    sW = (sWidth * destHeight) / sHeight;
                }
            }
            else
            {
                sW = sWidth;
                sH = sHeight;
            }
            Bitmap outBmp = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量         
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            imgSource.Dispose();
            return outBmp;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opndlg = new OpenFileDialog();
            opndlg.Filter = "所有图像文件|*.bmp;*.pcx;*.png;*.jpg;*.gif";
            opndlg.Title = "打开图像文件";
            if (opndlg.ShowDialog() == DialogResult.OK)
            {
                curFileName = opndlg.FileName;
                try
                {
                    Bitmap bm = (Bitmap)Image.FromFile(curFileName);
                    bm = GetThumbnail(bm, 400, 400);
                    this.pictureBox1.Image = bm;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        
    }
}
