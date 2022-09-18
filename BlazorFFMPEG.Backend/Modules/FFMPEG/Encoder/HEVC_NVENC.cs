using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;

public class HEVC_NVENC : EncoderBase
{
    public override List<QualityMethod> getCompatibleQualityMethods()
    {
        List<QualityMethod> compatibleQualityMethods = new List<QualityMethod>()
        {
            new BitrateQualityMethod(0, 800000),
            new CRFQualityMethod(0, 51)
        };

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