using System;
using System.Collections.Generic;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class ConstantsStatus
    {
        public ConstantsStatus()
        {
            EncodeJobs = new HashSet<EncodeJob>();
        }

        public int Id { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<EncodeJob> EncodeJobs { get; set; }
    }
}
