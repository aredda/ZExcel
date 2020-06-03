using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zekri_Tool.Models.Interfaces;

namespace Zekri_Tool.Models
{
	[Serializable]
    public class Description
    {
		private string language;

		public string Language
		{
			get { return language; }
			set { language = value; }
		}

		private string content;

		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		// for previewing purposes
		public string PreviewContent
		{
			get 
			{
				if (Content.Length > 32)
					return Content.Substring(0, 32).Trim() + "...";

				return Content;
			}
		}
	}
}
