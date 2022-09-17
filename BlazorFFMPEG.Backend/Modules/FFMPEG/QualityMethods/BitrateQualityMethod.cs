using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;

public class BitrateQualityMethod : QualityMethod
{
    public BitrateQualityMethod(long minQualityValue, long maxQualityValue) : base(minQualityValue, maxQualityValue)
    {
    }
    
    public override EQualityMethods getQualityMethodAsEnum()
    {
        return EQualityMethods.Bitrate;
    }
}