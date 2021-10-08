# Denoise-Bitmap


Example:
```cs
    Bitmap InputBitmap = (Bitmap)Bitmap.FromFile("noise.bmp");    
    denoise.DeNoise(InputBitmap,15,40);
    denoise.DeNoise(InputBitmap, 10,30);
    pictureBox1.Image = InputBitmap;
    pictureBox1.Refresh();
    pictureBox1.Image.Save("outp.bmp");
 ```

![alt text](https://github.com/PieterBoots/Denoise-Bitmap/blob/main/noise.bmp?raw=true)
![alt text](https://github.com/PieterBoots/Denoise-Bitmap/blob/main/outp.bmp?raw=true)
