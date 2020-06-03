using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zekri_Tool.Models.Abstract;
using Zekri_Tool.Models.Attributes;
using Zekri_Tool.Models.Interfaces;

namespace Zekri_Tool.Models
{
    public class Product
		: Parsable
    {
		private string title;

		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		private string subTitle;

		public string SubTitle
		{
			get { return subTitle; }
			set { subTitle = value; }
		}

		private Description description;

		[Column(Name="Desc")]
		public Description Description
		{
			get { return description; }
			set { description = value; }
		}

		private string keywords;
		[IgnoreProperty]
		public string Keywords
		{
			get { return keywords; }
			set { keywords = value; }
		}

		public override string ToString()
		{
			return Title + ", " + SubTitle + ", " + Description.PreviewContent + ", " + Keywords;
		}

		public override List<string> Parse()
		{
			List<string> data = new List<string> { Title, SubTitle, Description.Content };

			if (Keywords != null)
				foreach (string key in Keywords.Split(','))
					data.Add(key.Trim());

			return data;
		}

		public override List<string> GetColumns()
		{
			List<string> cols = base.GetColumns();

			for (int i = 0; i < 7; i++) cols.Add("key" + (i + 1));

			return cols;
		}

		public override void Configure(List<string> data)
		{
			this.Title = data[0];
			this.SubTitle = data[1];
			this.Description = new Description { Content = data[2] };
			this.Keywords = "";

			for (int i = 3; i < data.Count; i++)
			{
				this.Keywords += data[i];

				if (i != data.Count - 1)
					this.Keywords += ", ";
			}
		}
	}
}
