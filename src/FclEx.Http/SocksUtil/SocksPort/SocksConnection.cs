﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Http.SocksUtil.Http.Extensions;

namespace FclEx.Http.SocksUtil.SocksPort
{
	internal sealed class SocksConnection
	{
	    internal IPEndPoint _endPoint = null;
	    internal Uri _destination;
	    internal Socket _socket;
	    internal Stream _stream;
	    internal int _referenceCount;
		private readonly object _lock = new object();

		private void HandshakeTor()
		{
			var sendBuffer = new byte[] { 5, 1, 0 };
			_socket.Send(sendBuffer, SocketFlags.None);

			var recBuffer = new byte[_socket.ReceiveBufferSize];
			var recCnt = _socket.Receive(recBuffer, SocketFlags.None);

			Util.ValidateHandshakeResponse(recBuffer, recCnt);
		}


		private void ConnectSocket()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
			{
				Blocking = true
			};
			_socket.Connect(_endPoint);
		}

		private async Task ConnectToDestinationAsync(CancellationToken ctsToken = default(CancellationToken))
		{
			var sendBuffer = new ArraySegment<byte>(Util.BuildConnectToUri(_destination).Array);
			await _socket.SendAsync(sendBuffer, SocketFlags.None).ConfigureAwait(false);
			ctsToken.ThrowIfCancellationRequested();

			var recBuffer = new ArraySegment<byte>(new byte[_socket.ReceiveBufferSize]);
			var recCnt = await _socket.ReceiveAsync(recBuffer, SocketFlags.None).ConfigureAwait(false);
			ctsToken.ThrowIfCancellationRequested();

			Util.ValidateConnectToDestinationResponse(recBuffer.Array, recCnt);

			Stream stream = new NetworkStream(_socket, ownsSocket: false);
			if (_destination.Scheme.Equals("https", StringComparison.Ordinal))
			{
				SslStream httpsStream;
				// On Linux and OSX ignore certificate, because of a .NET Core bug
				// This is a security vulnerability, has to be fixed as soon as the bug get fixed
				// Details:
				// https://github.com/dotnet/corefx/issues/21761
				// https://github.com/nopara73/DotNetTor/issues/4
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					httpsStream = new SslStream(
						stream,
						leaveInnerStreamOpen: true);
				}
				else
				{
					httpsStream = new SslStream(
						stream,
						leaveInnerStreamOpen: true,
						userCertificateValidationCallback: (a, b, c, d) => true);
				}

				await httpsStream
					.AuthenticateAsClientAsync(
						_destination.DnsSafeHost,
						new X509CertificateCollection(),
						SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12,
						checkCertificateRevocation: true)
					.ConfigureAwait(false);
				stream = httpsStream;
			}
			_stream = stream;
		}

		private bool IsSocketConnected(bool throws)
		{
			try
			{
				if (_socket == null)
					return false;
				if (!_socket.Connected)
					return false;
				//if (Socket.Available == 0)
				//	return false;
				//if (Socket.Poll(1000, SelectMode.SelectRead))
				//	return false;

				return true;
			}
			catch
			{
				if (throws)
					throw;

				return false;
			}
		}

		public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken ctsToken)
		{
			try
			{
				await EnsureConnectedToTorAsync(ctsToken).ConfigureAwait(false);
				ctsToken.ThrowIfCancellationRequested();

				// https://tools.ietf.org/html/rfc7230#section-3.3.2
				// A user agent SHOULD send a Content - Length in a request message when
				// no Transfer-Encoding is sent and the request method defines a meaning
				// for an enclosed payload body.For example, a Content - Length header
				// field is normally sent in a POST request even when the value is 0
				// (indicating an empty payload body).A user agent SHOULD NOT send a
				// Content - Length header field when the request message does not contain
				// a payload body and the method semantics do not anticipate such a
				// body.
				// TODO implement it fully (altough probably .NET already ensures it)
				if (request.Method == HttpMethod.Post)
				{
					if (request.Headers.TransferEncoding.Count == 0)
					{
						if (request.Content == null)
						{
							request.Content = new ByteArrayContent(new byte[] { }); // dummy empty content
							request.Content.Headers.ContentLength = 0;
						}
						else
						{
							if (request.Content.Headers.ContentLength == null)
							{
								request.Content.Headers.ContentLength = (await request.Content.ReadAsStringAsync().ConfigureAwait(false)).Length;
							}
						}
					}
				}

				var requestString = await request.ToHttpStringAsync(ctsToken).ConfigureAwait(false);
				ctsToken.ThrowIfCancellationRequested();

				await _stream.WriteAsync(Encoding.UTF8.GetBytes(requestString), 0, requestString.Length, ctsToken).ConfigureAwait(false);
				await _stream.FlushAsync(ctsToken).ConfigureAwait(false);
				ctsToken.ThrowIfCancellationRequested();

				return await new HttpResponseMessage().CreateNewAsync(_stream, request.Method, ctsToken).ConfigureAwait(false);
			}
			catch (SocketException)
			{
				DestroySocket();
				throw;
			}
		}

		private async Task EnsureConnectedToTorAsync(CancellationToken ctsToken = default(CancellationToken))
		{
			if (!IsSocketConnected(throws: false)) // Socket.Connected is misleading, don't use that
			{
				DestroySocket();
				ConnectSocket();
				HandshakeTor();
				await ConnectToDestinationAsync(ctsToken).ConfigureAwait(false);
			}
		}

		public void AddReference() => Interlocked.Increment(ref _referenceCount);

		public void RemoveReference(out bool disposed)
		{
			disposed = false;
			var value = Interlocked.Decrement(ref _referenceCount);
			if (value == 0)
			{
				lock (_lock)
				{
					DestroySocket();
					disposed = true;
				}
			}
		}

		private void DestroySocket()
		{
			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
			if (_socket != null)
			{
				try
				{
					_socket.Shutdown(SocketShutdown.Both);
				}
				catch (SocketException) { }
				_socket.Dispose();
				_socket = null;
			}
		}
	}
}
