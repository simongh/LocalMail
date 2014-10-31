using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalMail
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var o = new Services.ListenerService();

			o.Start();

			Console.ReadLine();

			o.Stop();
		}
	}
}