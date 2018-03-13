using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Windows.Forms;
using fnc.fnc;

namespace fnc
{
    public partial class Form1 : Form
    {
        private Bitmap[] m_bitmaps = new Bitmap[2];
        private int m_currBitmap = 0;

        private List<Channel> m_channels = new List<Channel>();
        private float m_time = 0;
        private float m_period = 2.8f;
        private float m_fadeTime = 0.2f;
        private Random m_rand = new Random();

        public Form1()
        {
            int width = 1280 / 2;
            int height = 720 / 2;
            InitializeComponent();
            m_bitmaps[0] = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            m_bitmaps[1] = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            m_channels.Add(new Channel(m_period, m_fadeTime, true));
            m_channels.Add(new Channel(m_period, m_fadeTime, false));
            m_channels.Add(new Channel(m_period, m_fadeTime, false));
            
            Reset();
        }

        private static byte ToByte(float i_val)
        {
            int ival = (int)(i_val * 255);
            if (ival < 0)
                return 0;
            if (ival > 255)
                return 255;
            return (byte)ival;
        }
        
        public void UpdateBitmap()
        {
            float timeStep = 0.02f;

            int newBitmap = m_currBitmap ^ 1;
            int w = m_bitmaps[newBitmap].Width;
            int h = m_bitmaps[newBitmap].Height;

            float wm = 2.0f / w;
            float hm = 2.0f / h;

            foreach(var channel in m_channels)
                channel.Update(m_rand, timeStep);
            
            var data = m_bitmaps[newBitmap].LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, m_bitmaps[newBitmap].PixelFormat);

            m_time += timeStep;
            float t = (float)(Math.Sin(m_time) * 0.2 + 0.2);

            Text = t.ToString();

            unsafe
            {
                var ptr = (byte*)data.Scan0.ToPointer();
                var stride = data.Stride;

                Parallel.For(0, h, (int y) =>
                {
                    for (int x = 0; x < w; x++)
                    {
                        var pixel = ptr + y * stride + x * 4;

                        float fx = (x * wm) - 1.0f;
                        float fy = -(y * wm) + 1.0f;

                        Vector3 value = Vector3.Zero;
                        foreach (var channel in m_channels)
                            value += channel.Value(fx, fy, t);
                        value /= m_channels.Count;

                        pixel[0] = ToByte(value.X);
                        pixel[1] = ToByte(value.Y);
                        pixel[2] = ToByte(value.Z);
                        pixel[3] = 0;
                    }
                });
            }

            m_bitmaps[newBitmap].UnlockBits(data);

            pictureBox1.Image = m_bitmaps[newBitmap];
            m_currBitmap = newBitmap;
        }

        private void Reset()
        {
            for(int index = 0; index < m_channels.Count; index++)
            {
                m_channels[index].Reset();
                m_channels[index].Update(m_rand, index * (m_period / m_channels.Count));
            }

            m_time = 0;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                Reset();
            }
        }
    }
}
