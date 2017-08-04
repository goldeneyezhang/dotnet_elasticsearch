using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchTest
{
	public class Setting
	{
		public static string strConnectionString = @"http://localhost:9200";
		public static Uri Node
		{
			get
			{
				return new Uri(strConnectionString);
			}
		}
		public static ConnectionSettings ConnectionSettings
		{
			get
			{
				return new ConnectionSettings(Node).DefaultIndex("default");
			}
		}
	}
}
