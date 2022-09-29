using System;
using System.Collections.Generic;

namespace BlazorFFMPEG.Backend.Database
{
    public partial class AutoEncodeFolder
    {
        public int Folderid { get; set; }
        public string Inputpath { get; set; } = null!;
        public string Outputpath { get; set; } = null!;
    }
}
