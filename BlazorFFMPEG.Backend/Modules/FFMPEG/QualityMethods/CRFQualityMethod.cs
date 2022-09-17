using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;

public class CRFQualityMethod : QualityMethod
{
    public CRFQualityMethod(long minQualityValue, long maxQualityValue) : base(minQualityValue, maxQualityValue)
    {
        
    }
    
    public override EQualityMethods getQualityMethodAsEnum()
    {
        return EQualityMethods.CQ;
    }
}