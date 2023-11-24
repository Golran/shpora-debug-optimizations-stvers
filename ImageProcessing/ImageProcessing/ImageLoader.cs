using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
	public static class ImageLoader
	{

		/// <summary>
		/// Performs given action on extracted bitmap data. After action is performed, bitmap is unlocked in memory 
		/// (so bitmap data should not be referenced outside of the action)
		/// </summary>
		/// <param name="bmp">The bitmap to process</param>
		/// <param name="bitmapDataAction">Action to perform</param>
		public static T WithBitmapData<T>(this Bitmap bmp, Func<BitmapData, T> bitmapDataAction)
		{
			var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			var bitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
			try
			{
				return bitmapDataAction(bitmapData);
			}
			finally
			{
				bmp.UnlockBits(bitmapData);
			}
		}

		/// <summary>
		/// Performs given action on extracted bitmap data. After action is performed, bitmap is unlocked in memory 
		/// (so bitmap data should not be referenced outside of the action)
		/// </summary>
		/// <param name="bmp">The bitmap to process</param>
		/// <param name="bitmapDataAction">Action to perform</param>
		public static void WithBitmapData(this Bitmap bmp, Action<BitmapData> bitmapDataAction)
		{
			WithBitmapData(bmp, bitmapData =>
			{
				bitmapDataAction(bitmapData);
				return 0;
			});
		}

		/// <summary>
		/// Set colors in bitmap
		/// </summary>
		/// <param name="bmp">Bitmap to set</param>
		/// <param name="colors">Seted colors</param>
		public static void SetColorValues(this Bitmap bmp, byte[] colors)
		{
			bmp.WithBitmapData(bmpData => Marshal.Copy(colors, 0, bmpData.Scan0, colors.Length));
		}

		/// <summary>
		/// Get colors in bitmaps format
		/// </summary>
		/// <param name="bmpData">Bitmap Data</param>
		/// <returns>Colors in bitmaps format</returns>
		public static byte[] CopyRasterData(this BitmapData bmpData)
		{
			var size = bmpData.Stride * bmpData.Height;
			var pointer = bmpData.Scan0;

			var colorValues = RGBImage.AllocateImageData(bmpData.Stride, bmpData.Height);

			Marshal.Copy(pointer, colorValues, 0, size);
			return colorValues;
		}
	}
}