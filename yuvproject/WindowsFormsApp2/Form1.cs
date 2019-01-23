
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        FileStream fs;
        string filename;
        int yy = 0;
        double framesay;
        int oynt = 0;
        float katsayi;
        float uvsayi;

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();





            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filename = dlg.FileName;
                textBox1.Text = filename;

                textBox1.Enabled = false;

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text.Equals("") || textBox2.Text.Equals("") || textBox3.Text.Equals(""))
            {
                MessageBox.Show("Bos birakilamaz !!");
            }
            else
            {
                FileInfo fi = new FileInfo(filename);
                long dosyaBoyutu = fi.Length;

                int w = Int32.Parse(textBox2.Text);
                int h = Int32.Parse(textBox3.Text);
                int len = w * h;

                if (comboBox1.Text.Equals("4-2-0"))
                {

                    katsayi = 4;
                    uvsayi = 0.25f;
                    framesay = dosyaBoyutu / (w * h * 1.5);

                }
                else if (comboBox1.Text.Equals("4-2-2"))
                {
                    katsayi = 2;
                    uvsayi = 0.5f;
                   framesay= dosyaBoyutu / (w * h * 2);
                }
                else
                {

                    katsayi = 1;
                    uvsayi = 1;
                    framesay = dosyaBoyutu / (w * h * 3);

                }
                //MessageBox.Show(framesay.ToString()); 

                fs = new FileStream(filename, FileMode.Open);
                BinaryReader readBinary = new BinaryReader(fs);
                int size = Int32.Parse(textBox2.Text) * Int32.Parse(textBox3.Text);
                byte[] ydizi = new byte[size];
                int size1 = (int)(size / katsayi);
                byte[] udizi = new byte[size];
                byte[] vdizi = new byte[size];

                for (int jj = 0; jj < framesay; jj++)
                {

                    for (int k = 0; k < size; k++)
                    {
                        udizi[k] = 0; vdizi[k] = 0;

                    }

                    try
                    {
                        for (int i = 0; i < size; i++)
                        {
                            ydizi[i] = readBinary.ReadByte();
                            // MessageBox.Show(ydizi[i].ToString());        //for try
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    try
                    {
                        for (int i = 0; i < size1; i++)
                        {
                            udizi[i] = readBinary.ReadByte();
                            //MessageBox.Show(udizi[i].ToString());     //for try
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    try
                    {
                        for (int i = 0; i < size1; i++)
                        {
                            vdizi[i] = readBinary.ReadByte();
                            //MessageBox.Show(vdizi[i].ToString());         //for try
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    byte[] r = new byte[(int)size];
                    byte[] g = new byte[(int)size];
                    byte[] b = new byte[(int)size];
                    byte[] rgb1 = new byte[(int)size];
                    Color[] palette = new Color[(int)size];
                    for (int i = 0; i < size; i++)
                    {

                        r[i] = clamp((byte)(ydizi[i]));

                        /*r[i] = clamp((byte)(ydizi[i] + 1.4075 * (vdizi[i] - 128)));
                        g[i] = clamp((byte)(ydizi[i] - 0.3455 * (udizi[i] - 128) - (0.7169 * (vdizi[i] - 128)))); //bu 3 satır kod renkli olması için gerekliydi 
                       b[i] = clamp((byte)(ydizi[i] + 1.7790 * (udizi[i] - 128))); */

                    }

                    Bitmap pic = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Rectangle rect = new Rectangle(0, 0, w, h);
                    BitmapData picd = pic.LockBits(rect, ImageLockMode.WriteOnly, pic.PixelFormat);
                    int padding = picd.Stride - (3 * w);
                    unsafe
                    {
                        byte* ptr = (byte*)picd.Scan0;


                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                ptr[2] = r[y * w + x];
                                ptr[1] = r[y * w + x];
                                ptr[0] = r[y * w + x];
                                ptr += 3;
                            }
                            ptr += padding;
                        }
                    };
                    pic.UnlockBits(picd);
                    pic.Save("frames/test_bmp" + jj, ImageFormat.Bmp);

                }

                fs.Close();

            }

            textBox4.Text = framesay.ToString();
            textBox4.Enabled = false;

        }
        public static byte clamp(int val)
        {
            if (val < 0) { return 0; }
            else if (val > 255) { return 255; }
            else { return (byte)val; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (oynt < framesay)
            {
                pictureBox1.Image = null;
                Image image = Image.FromFile("frames/test_bmp" + oynt);
                pictureBox1.Image = image;
                pictureBox1.Height = image.Height;
                pictureBox1.Width = image.Width;
                pictureBox1.ImageLocation = ("frames/test_bmp" + oynt);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;   
                oynt++;
            }
            else
            {
                timer1.Enabled = false;
                oynt = 0;
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            oynt = 0;
            timer1.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            oynt = oynt + 25;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            oynt = oynt - 25;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

       
    }

}

