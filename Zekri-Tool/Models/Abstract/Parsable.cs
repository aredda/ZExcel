using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zekri_Tool.Models.Attributes;
using Zekri_Tool.Models.Interfaces;

namespace Zekri_Tool.Models.Abstract
{
    public abstract class Parsable
        : IParsable
    {
        public abstract void Configure(List<string> data);
        
        public virtual List<string> GetColumns()
        {
            List<string> cols = new List<string>();

            foreach (PropertyInfo p in GetType().GetProperties())
                if (p.GetCustomAttribute(typeof(IgnoreProperty)) == null)
                {
                    string name = p.Name;

                    if (p.GetCustomAttribute(typeof(Column)) != null)
                        name = ((Column)p.GetCustomAttribute(typeof(Column))).Name;

                    cols.Add(name);
                }

            return cols;
        }

        public abstract List<string> Parse();
    }
}
