using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LocalMail.Services
{
	internal class ListenerService : IDisposable
	{
		private TcpListener _listener;
		private bool _disposed;
		private List<ClientService> _clients;

		public bool IsListening { get; private set; }

		public ListenerService()
		{
			_clients = new List<ClientService>();
		}

		public void Start()
		{
			_listener = TcpListener.Create(25);
			_listener.ExclusiveAddressUse = false;

			_listener.Start();
			IsListening = true;

			_listener.BeginAcceptTcpClient(new AsyncCallback(Callback), null);
		}

		public void Stop()
		{
			IsListening = false;
			_listener.Stop();

			lock (_clients)
			{
				foreach (var item in _clients)
				{
					item.End();
				}
			}
		}

		private void Callback(IAsyncResult ar)
		{
			if (!IsListening)
				return;

			var client = _listener.EndAcceptTcpClient(ar);

			Console.WriteLine("Client connection done");
			var c = new ClientService(client);
			c.CloseCallback = Closing;
			lock (client)
			{
				_clients.Add(c);
			}
			c.Start();

			_listener.BeginAcceptTcpClient(new AsyncCallback(Callback), null);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (IsListening)
				Stop();

			_listener = null;
			_disposed = true;
		}

		private void Closing(ClientService client)
		{
			lock (_clients)
			{
				_clients.Remove(client);
			}
		}
	}
}