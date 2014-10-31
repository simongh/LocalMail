using System;
using System.Net.Sockets;
using System.Text;

namespace LocalMail.Services
{
	internal class ClientService
	{
		private TcpClient _client;
		private NetworkStream _stream;
		private byte[] _buffer;
		private Encoding _encoder;

		public Action<ClientService> CloseCallback { get; set; }

		public ClientService(TcpClient client)
		{
			_client = client;

			_buffer = new byte[1024];
			_stream = client.GetStream();
			_encoder = Encoding.UTF8;
		}

		public void Start()
		{
			_stream.BeginRead(_buffer, 0, _buffer.Length, (ar) =>
			{
				var stream = ar.AsyncState as NetworkStream;
				var read = stream.EndRead(ar);
				if (read == 0)
				{
					End();
					return;
				}

				var msg = _encoder.GetString(_buffer, 0, read);
				Console.WriteLine(msg);
				Write(msg);

				if (msg == "END")
					End();
				else
					Start();
			}, _stream);
		}

		public void End()
		{
			if (_stream == null)
				return;

			if (_stream.CanRead)
				_client.Client.Disconnect(false);

			_stream = null;

			CloseCallback(this);
		}

		public void Write(string message)
		{
			var buffer = _encoder.GetBytes(message);
			_stream.BeginWrite(buffer, 0, buffer.Length, (ar) =>
			{
				var stream = ar.AsyncState as NetworkStream;
				stream.EndWrite(ar);
			}, _stream);
		}
	}
}