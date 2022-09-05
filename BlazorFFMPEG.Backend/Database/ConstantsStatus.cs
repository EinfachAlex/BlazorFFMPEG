using System;
using System.Collections.Generic;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class ConstantsStatus
    {
        public ConstantsStatus()
        {
            Encodejobs = new HashSet<Encodejob>();
        }

        public int Id { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Encodejob> Encodejobs { get; set; }
    }
}
