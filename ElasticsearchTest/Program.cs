using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchTest
{
	class Program
	{
		static void Main(string[] args)
		{
			MeetupEvents events = new MeetupEvents();
			events.description = "我是erwerwerw";
			events.eventid = 2;
			events.eventname = "我是xxxxdfsd";
			events.orignalid = "2";
			ESProvider provider = new ESProvider();
			provider.PopulateIndex(events);
			var result = provider.GetResult_TermQuery("我是事件");
			Console.WriteLine(result.ToString());
			Console.WriteLine(result.Count());
			Console.ReadLine();
		}
	}
}
