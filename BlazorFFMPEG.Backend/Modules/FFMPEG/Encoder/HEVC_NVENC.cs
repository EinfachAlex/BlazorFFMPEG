using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;

public class HEVC_NVENC : EncoderBase
{
    public override List<ConstantsQualitymethod> getCompatibleQualityMethods(databaseContext databaseContext)
    {
        List<ConstantsQualitymethod> compatibleQualityMethods = databaseContext.ConstantsQualitymethods.ToList();

        return compatibleQualityMethods;
    }
    
    public override EEncoders getAsEnum()
    {
        return EEncoders.HEVC_NVENC;
    }

    public override void checkQualityMethodValue(string qualityMethod, string qualityValue)
    {
        //QualityMethod qualityMethodObject = QualityMethod.construct(qualityValue, );
        throw new NotImplementedException();
    }
}