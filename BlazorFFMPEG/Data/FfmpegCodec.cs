namespace BlazorFFMPEG.Data;

public class FfmpegCodec
{
    public FfmpegCodec(string name)
    {
        this.name = name;
    }
    
    public string name { get; set;  }
}