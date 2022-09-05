using System;
using System.Collections.Generic;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class Encodejob
    {
        public int Jobid { get; set; }
        public int Status { get; set; }

        public virtual ConstantsStatus StatusNavigation { get; set; } = null!;
    }
}
