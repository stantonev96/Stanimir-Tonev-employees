using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    public class Info
    {
        public Info()
        {
            EmployeeIds = new int[2];
        }

        public int ProjectId { get; set; }

        public int[] EmployeeIds { get; set; }

        public int LongestTimeInDays { get; set; }
    }
}
