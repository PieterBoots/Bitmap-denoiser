using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


    class denoise
    {

    public static Color YUVToColor(int Y, int U, int V)
    {
        int[] rgb = new int[3];
        int C = Y - 16;
        int D = U - 128;
        int E = V - 128;

        rgb[0] = clip((298 * C + 409 * E + 128) >> 8);
        rgb[1] = clip((298 * C - 100 * D - 208 * E + 128) >> 8);
        rgb[2] = clip((298 * C + 516 * D + 128) >> 8);

        return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]); ;
    }

    public static int[] ColorToYUV(Color c)
    {
        int[] yuv = new int[3];
        yuv[0] = ((66 * c.R + 129 * c.G + 25 * c.B + 128) >> 8) + 16;
        yuv[1] = ((-38 * c.R - 74 * c.G + 112 * c.B + 128) >> 8) + 128;
        yuv[2] = ((112 * c.R - 94 * c.G - 18 * c.B + 128) >> 8) + 128;
        return yuv;
    }

    public static int clip(int inp)
    {
        if (inp > 255) { return 255; }
        if (inp < 0) { return 0; }
        return inp;
    }

    public static void DeNoise(Bitmap InputBitmap, double Y_quality, double uv_quality)
    {
        double[,] memory_y = new double[320, 240];
        double[,] memory_u = new double[320, 240];
        double[,] memory_v = new double[320, 240];
        for (int y = 0; y < InputBitmap.Height; y++)
            for (int x = 0; x < InputBitmap.Width; x++)
            {
                int[] yuv = ColorToYUV(InputBitmap.GetPixel(x, y));
                memory_y[x, y] = yuv[0];
                memory_u[x, y] = yuv[1];
                memory_v[x, y] = yuv[2];
            }
        for (int y = 0; y < InputBitmap.Height; y++)
            for (int x = 0; x < InputBitmap.Width; x++)
            {
                int N = 5;
                int l = (N - 1) / 2;
                double avg_y = kernel(InputBitmap, memory_y, x, y, l, Y_quality);
                N = 7;
                l = (N - 1) / 2;
                Double avg_u = kernel(InputBitmap, memory_u, x, y, l, uv_quality);
                Double avg_v = kernel(InputBitmap, memory_v, x, y, l, uv_quality);
                int cy = System.Convert.ToInt32(avg_y);
                int cu = System.Convert.ToInt32(avg_u);
                int cv = System.Convert.ToInt32(avg_v);
                InputBitmap.SetPixel(x, y, YUVToColor(cy, cu, cv));
            }
    }

    private static double kernel(Bitmap InputBitmap, double[,] memory_y, int x, int y, int l, double quality)
    {
        Double rf = memory_y[x, y];
        double avg = 0;
        double cnt = 0;
        for (int i = -l; i <= l; i++)
            for (int j = -l; j <= l; j++)
            {
                int xt = x + i;
                int yt = y + j;
                if (xt < 0)
                { xt = 0; }
                if (xt > InputBitmap.Width - 1)
                    xt = InputBitmap.Width - 1;
                if (yt < 0)
                { yt = 0; }
                if (yt > InputBitmap.Height - 1)
                    yt = InputBitmap.Height - 1;

                double diff = Math.Abs(rf - memory_y[xt, yt]);

                if (diff >= 0 && diff < quality)
                {
                    avg= avg+ memory_y[xt, yt];
                    cnt = cnt + 1.0;
                }
                if (diff >= quality && diff < quality + 20)
                {
                    avg = avg + memory_y[xt, yt] * 0.1;
                    cnt = cnt + 0.1;
                }
                if (diff >= quality + 20 && diff < quality + 60)
                {
                    avg = avg + memory_y[xt, yt] * 0.05;
                    cnt = cnt + 0.05;
                }
            }        
        return avg / cnt;
    }
}

