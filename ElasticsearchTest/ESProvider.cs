using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchTest
{
	public class ESProvider
	{
		public static ElasticClient client = new ElasticClient(Setting.ConnectionSettings);
		public static string strIndexName = @"meetup".ToLower();
		public static string strDocType = "events".ToLower();

		public bool PopulateIndex(MeetupEvents meetupevent)
		{
			var index = client.Index(meetupevent, i => i.Index(strIndexName).Type(strDocType).Id(meetupevent.eventid));
			return index.Created;
		}
		public bool BulkPopulateIndex(List<MeetupEvents> posts)
		{
			var bulkRequest = new BulkRequest(strIndexName, strDocType) { Operations = new List<IBulkOperation>() };
			var idxops = posts.Select(o => new BulkIndexOperation<MeetupEvents>(o) { Id = o.eventid }).Cast<IBulkOperation>().ToList();
			bulkRequest.Operations = idxops;
			var response = client.Bulk(bulkRequest);
			return response.IsValid;
		}
		/// <summary>
		/// 词条查询
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		public List<MeetupEvents> GetResult_TermQuery(string keyword)
		{
			//create term query
			TermQuery tq = new TermQuery();
			tq.Field = "eventname.keyword";
			tq.Value = keyword;

			//create search request
			SearchRequest sr = new SearchRequest("meetup", "events");
			sr.Query = tq;

			//windows
			sr.From = 0;
			sr.Size = 100;

			//sort
			ISort sort = new SortField { Field = "eventid", Order = SortOrder.Ascending };
			sr.Sort = new List<ISort>();
			sr.Sort.Add(sort);

			//source filter
			sr.Source = new SourceFilter()
			{
				Includes = new string[] { "eventid", "eventname" },
				Excludes = new string[] { "roginalid", "description" }
			};

			var result = client.Search<MeetupEvents>(sr);
			return result.Documents.ToList<MeetupEvents>();
		}
		/// <summary>
		/// 匹配查询
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		public List<MeetupEvents> GetResult_MatchQuery(string keyword)
		{
			SearchRequest sr = new SearchRequest("meetup", "events");
			MatchQuery mq = new MatchQuery();
			mq.Field = new Field("eventname");
			mq.Query = keyword;
			mq.MinimumShouldMatch = 2;
			mq.Operator = Operator.Or;

			sr.Query = mq;
			sr.From = 0;
			sr.Size = 100;
			sr.Sort = new List<ISort>();
			sr.Sort.Add(new SortField { Field = "eventid", Order = SortOrder.Ascending });

			ISearchResponse<MeetupEvents> result = client.Search<MeetupEvents>(sr);

			return result.Documents.ToList<MeetupEvents>();
		}
		/// <summary>
		/// 正则表达式查询
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		public List<MeetupEvents> GetResult_RegexpQuery(string keyword)
		{
			SearchRequest sr = new SearchRequest();

			RegexpQuery rq = new RegexpQuery();
			rq.Field = "description";
			rq.Value = keyword+"*";
			rq.MaximumDeterminizedStates = 20000;

			sr.Query = rq;

			var result = client.Search<MeetupEvents>(sr);
			return result.Documents.ToList<MeetupEvents>();
		}
		/// <summary>
		/// 布尔查询
		/// </summary>
		/// <param name="keyword"></param>
		/// <returns></returns>
		public List<MeetupEvents> GetResult_BoolQuery()
		{
			SearchRequest sr = new SearchRequest("meetup", "events");

			BoolQuery bq = new BoolQuery();
			bq.Filter = new QueryContainer[]
			{
		new MatchQuery()
		{
			Field="eventname",
			Query="azure cloud",
			Operator=Operator.Or,
			MinimumShouldMatch=1
		},
		new MatchQuery()
		{
			Field ="eventname",
			Query="aws google",
			Operator=Operator.Or,
			MinimumShouldMatch=1
		 }
			};
			bq.Should = new QueryContainer[]
			{
		new TermQuery()
		{
			Field="description",
			Value="azure"
		},
		new TermQuery()
		{
			Field="description",
			Value="cloud"
		}

			};
			bq.MinimumShouldMatch = 1;

			sr.Query = bq;

			var result = client.Search<MeetupEvents>(sr);
			return result.Documents.ToList<MeetupEvents>();
		}
	}
}
