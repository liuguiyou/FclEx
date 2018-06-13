﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FclEx.Http.SocksUtil.Http.Extensions
{
    public static class StreamExtensions
    {
		public static async Task<int> ReadByteAsync(this Stream stream, CancellationToken ctsToken = default(CancellationToken))
		{
			var buf = new byte[1];
			var len = await stream.ReadAsync(buf, 0, 1, ctsToken).DonotCapture();
			if (len == 0) return -1;
			else return buf[0];
		}
		public static async Task<int> ReadBlockAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken ctsToken = default(CancellationToken))
		{
			var left = count;
			while(left != 0)
			{
				var read = await stream.ReadAsync(buffer, count - left, left, ctsToken).DonotCapture();
				left -= read;
			}
			return count - left;
		}
	}
}
