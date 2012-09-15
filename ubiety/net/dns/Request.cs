using System.Collections.Generic;

namespace ubiety.net.dns
{
	///<summary>
	///</summary>
	public class Request
	{
		private readonly List<Question> _questions;

		///<summary>
		///</summary>
		public Header Header;

		///<summary>
		///</summary>
		public Request()
		{
			Header = new Header {OPCODE = OPCode.Query, QDCOUNT = 0};

			_questions = new List<Question>();
		}

		///<summary>
		///</summary>
		public byte[] Data
		{
			get
			{
				var data = new List<byte>();
				Header.QDCOUNT = (ushort) _questions.Count;
				data.AddRange(Header.Data);
				foreach (var q in _questions)
					data.AddRange(q.Data);
				return data.ToArray();
			}
		}

		///<summary>
		///</summary>
		///<param name="question"></param>
		public void AddQuestion(Question question)
		{
			_questions.Add(question);
		}
	}
}