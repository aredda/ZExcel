using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zekri_Tool.Models.Interfaces
{
    public interface IParsable
    {
        List<string> Parse();
        List<string> GetColumns();
        void Configure(List<string> data);
    }
}
