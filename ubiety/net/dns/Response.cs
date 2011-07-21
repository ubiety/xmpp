using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ubiety.net.dns.Records;

namespace ubiety.net.dns
{
	///<summary>
	///</summary>
	public class Response
	{
		/// <summary>
		/// List of AdditionalRR records
		/// </summary>
		public List<AdditionalRR> Additionals;

		/// <summary>
		/// List of AnswerRR records
		/// </summary>
		public List<AnswerRR> Answers;

		/// <summary>
		/// List of AuthorityRR records
		/// </summary>
		public List<AuthorityRR> Authorities;

		/// <summary>
		/// Error message, empty when no error
		/// </summary>
		public string Error;

		/// <summary>
		/// The Size of the message
		/// </summary>
		public int MessageSize;

		/// <summary>
		/// List of Question records
		/// </summary>
		public List<Question> Questions;

		/// <summary>
		/// Server which delivered this response
		/// </summary>
		public IPEndPoint Server;

		/// <summary>
		/// TimeStamp when cached
		/// </summary>
		public DateTime TimeStamp;

		///<summary>
		///</summary>
		public Header Header;

		///<summary>
		///</summary>
		public Response()
		{
			Questions = new List<Question>();
			Answers = new List<AnswerRR>();
			Authorities = new List<AuthorityRR>();
			Additionals = new List<AdditionalRR>();

			Server = new IPEndPoint(0, 0);
			Error = "";
			MessageSize = 0;
			TimeStamp = DateTime.Now;
			Header = new Header();
		}

		///<summary>
		///</summary>
		///<param name="iPEndPoint"></param>
		///<param name="data"></param>
		public Response(IPEndPoint iPEndPoint, byte[] data)
		{
			Error = "";
			Server = iPEndPoint;
			TimeStamp = DateTime.Now;
			MessageSize = data.Length;
			var rr = new RecordReader(data);

			Questions = new List<Question>();
			Answers = new List<AnswerRR>();
			Authorities = new List<AuthorityRR>();
			Additionals = new List<AdditionalRR>();

			Header = new Header(rr);

			for (var intI = 0; intI < Header.QDCOUNT; intI++)
			{
				Questions.Add(new Question(rr));
			}

			for (var intI = 0; intI < Header.ANCOUNT; intI++)
			{
				Answers.Add(new AnswerRR(rr));
			}

			for (var intI = 0; intI < Header.NSCOUNT; intI++)
			{
				Authorities.Add(new AuthorityRR(rr));
			}
			for (var intI = 0; intI < Header.ARCOUNT; intI++)
			{
				Additionals.Add(new AdditionalRR(rr));
			}
		}

		/// <summary>
		/// List of RecordMX in Response.Answers
		/// </summary>
		public RecordMX[] RecordsMX
		{
			get
			{
				var list = Answers.Select(answerRR => answerRR.Record).OfType<RecordMX>().ToList();
				list.Sort();
				return list.ToArray();
			}
		}

		/// <summary>
		/// List of RecordTXT in Response.Answers
		/// </summary>
		public RecordTXT[] RecordsTXT
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordTXT>().ToArray();
			}
		}

		/// <summary>
		/// List of RecordA in Response.Answers
		/// </summary>
		public RecordA[] RecordsA
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordA>().ToArray();
			}
		}

		/// <summary>
		/// List of RecordPTR in Response.Answers
		/// </summary>
		public RecordPTR[] RecordsPTR
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordPTR>().ToArray();
			}
		}

		/// <summary>
		/// List of RecordCNAME in Response.Answers
		/// </summary>
		public RecordCNAME[] RecordsCNAME
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordCNAME>().ToArray();
			}
		}

		/// <summary>
		/// List of RecordAAAA in Response.Answers
		/// </summary>
		public RecordAAAA[] RecordsAAAA
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordAAAA>().ToArray();
			}
		}

		/// <summary>
		/// List of RecordNS in Response.Answers
		/// </summary>
		public RecordNS[] RecordsNS
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordNS>().ToArray();
			}
		}

		/// <summary>
		/// List of RecordSOA in Response.Answers
		/// </summary>
		public RecordSOA[] RecordsSOA
		{
			get
			{
				return Answers.Select(answerRR => answerRR.Record).OfType<RecordSOA>().ToArray();
			}
		}

		///<summary>
		///</summary>
		public RR[] RecordsRR
		{
			get
			{
				var list = new List<RR>();
				foreach (RR rr in Answers)
				{
					list.Add(rr);
				}
				foreach (RR rr in Answers)
				{
					list.Add(rr);
				}
				foreach (RR rr in Authorities)
				{
					list.Add(rr);
				}
				foreach (RR rr in Additionals)
				{
					list.Add(rr);
				}
				return list.ToArray();
			}
		}
	}
}