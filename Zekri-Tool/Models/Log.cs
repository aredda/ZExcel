using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Zekri_Tool.Models
{
	[Serializable]
    public class Log
    {
		private string fileName;

		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		private string fullPath;

		public string FullPath
		{
			get { return fullPath; }
			set { 
				fullPath = value;
				FileName = fullPath.Split('\\').Last();
			}
		}

		private DateTime time;

		public DateTime Time
		{
			get { return time; }
			set { time = value; }
		}
	}
}
