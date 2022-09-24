using System;
using System.Collections.Generic;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class ConstantsQualitymethod
    {
        public ConstantsQualitymethod()
        {
            EncodeJobs = new HashSet<EncodeJob>();
        }

        public int Id { get; set; }
        public string? Description { get; set; }
        public int? Minqualityvalue { get; set; }
        public int? Maxqualityvalue { get; set; }

        public virtual ICollection<EncodeJob> EncodeJobs { get; set; }
    }
}
