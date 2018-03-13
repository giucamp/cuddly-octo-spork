using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using fnc.fnc;

namespace fnc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var vz = Utils.Spherical(0, 0);
            var vx = Utils.Spherical(.75f, 0);
            var vy = Utils.Spherical(0, .75f);
            var l1 = vz.Length();
            var l2 = vx.Length();
            var l3 = vy.Length();

            for(float x = -4; x <= 4; x+=0.1f)
            {
                float g = (Utils.TriangleWave(x, 1) + 1) * 8;
                
                System.Diagnostics.Debug.WriteLine(new string(' ', (int)g) + '*' );
            }

            Form1 fmr = new Form1();

            fmr.Show();
            while(fmr.Visible)
            {
                fmr.UpdateBitmap();
                Application.DoEvents();
                //fmr.Refresh();
                //System.Threading.Thread.Sleep(10);
            }

            //Application.Run(new Form1());
        }

    }
}
