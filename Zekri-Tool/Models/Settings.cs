using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Zekri_Tool.Models
{
	[Serializable]
    public class Settings
    {
		private List<Log> logs;

		public List<Log> Logs
		{
			get { return logs; }
			set { logs = value; }
		}

		private List<string> langs = new List<string> {"ENG", "FR", "ES", "JPN", "IT", "DE", "PG"};

		public List<string> Languages
		{
			get { return langs; }
			set { langs = value; }
		}

		private List<Description> savedDescriptions;

		public List<Description> SavedDescriptions
		{
			get { return savedDescriptions; }
			set { savedDescriptions = value; }
		}

		public Settings()
		{
			this.logs = new List<Log>();
			this.savedDescriptions = new List<Description>();
		}

		public void Save()
		{
			using (FileStream fs = new FileStream("settings.bnr", FileMode.Create)) 
				new BinaryFormatter().Serialize(fs, this);
		}

		public void Load()
		{
			Settings loaded = new Settings();

			if (File.Exists("settings.bnr"))
				using (FileStream fs = new FileStream("settings.bnr", FileMode.Open))
					loaded = (Settings) new BinaryFormatter().Deserialize(fs);

			Logs = loaded.Logs;
			SavedDescriptions = loaded.SavedDescriptions;
		}
	}
}
