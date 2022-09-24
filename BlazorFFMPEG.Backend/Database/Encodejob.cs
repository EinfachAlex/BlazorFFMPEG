using System;
using System.Collections.Generic;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class EncodeJob
    {
        public int Jobid { get; set; }
        public int Status { get; set; }
        public string? Codec { get; set; }
        public int? Qualitymethod { get; set; }
        public int? Qualityvalue { get; set; }
        public string? Path { get; set; }

        public virtual ConstantsQualitymethod? QualitymethodNavigation { get; set; }
        public virtual ConstantsStatus StatusNavigation { get; set; } = null!;
    }
}
