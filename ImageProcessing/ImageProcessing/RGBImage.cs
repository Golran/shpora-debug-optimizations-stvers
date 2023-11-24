using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
	public class RGBImage
	{
		public int Width { get; }

		public int Height { get; }

		public byte[] ImageData { get; }

		public int BytesPerLine { get; }

		public const int BytesPerPixel = 3;

		public RGBImage(int width, int height, int bytesPerLine, byte[] imageData)
		{
			Width = width;
			Height = height;
			BytesPerLine = bytesPerLine;
			ImageData = imageData;
		}

		/// <summary>
		/// Creates a new image object from file
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static RGBImage FromFile(string path)
		{
			using (var image = Image.FromFile(path))
			{
				using (var sourceBitmap = image as Bitmap ?? new Bitmap(image))
				{
					return FromBitmap(sourceBitmap);
				}
			}
		}

		/// <summary>
		/// Creates a new image object from the specified file
		/// </summary>
		/// <param name="data">Stream with an image content</param>
		/// <returns></returns>
		public static RGBImage FromStream(Stream data)
		{
			using (var image = Image.FromStream(data))
			{
				using (var sourceBitmap = image as Bitmap ?? new Bitmap(image))
				{
					return FromBitmap(sourceBitmap);
				}
			}
		}


		/// <summary>
		/// Creates a new image object from the given bitmap object
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static RGBImage FromBitmap(Bitmap bitmap)
		{
			return bitmap.WithBitmapData(GetBitmapDataAsIs);
		}

		/// <summary>
		/// Stores this image to a file
		/// </summary>
		/// <param name="destPath">The path where the image has to be stored</param>
		public void SaveToFile(string destPath)
		{
			using (var bitmap = ToBitmap())
			{
				bitmap.Save(destPath);
			}
		}

		public static byte[] AllocateImageData(int stride, int height)
		{
			var dataSize = height * stride;
			return new byte[dataSize];
		}

		private static RGBImage GetBitmapDataAsIs(BitmapData data)
		{
			var imageData = data.CopyRasterData();
			return new RGBImage(data.Width, data.Height, data.Stride,imageData);
		}

		/// <summary>
		/// Converts this image to Bitmap representation
		/// </summary>
		/// <returns></returns>
		public Bitmap ToBitmap()
		{
			var pixelFormat = PixelFormat.Format24bppRgb;
			var bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
			var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, pixelFormat);
			try
			{
				var newStride = bitmapData.Stride;
				var oldStride = BytesPerLine;
				if (newStride != oldStride)
				{
					var currentSourceOffset = 0;
					var currentTargetLinePtr = bitmapData.Scan0;
					for (var scanLineIdx = 0; scanLineIdx < Height; scanLineIdx++)
					{
						Marshal.Copy(ImageData, currentSourceOffset, currentTargetLinePtr, newStride);
						currentTargetLinePtr += newStride;
						currentSourceOffset += oldStride;
					}
				}
				else
				{
					Marshal.Copy(ImageData, 0, bitmapData.Scan0, bitmapData.Height * newStride);
				}
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}

			return bitmap;
		}

	}
}
