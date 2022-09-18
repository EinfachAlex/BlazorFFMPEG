using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;

public class LIBSVTAV1 : EncoderBase
{
    public override List<QualityMethod> getCompatibleQualityMethods()
    {
        List<QualityMethod> compatibleQualityMethods = new List<QualityMethod>()
        {
            //TODO update values
            new BitrateQualityMethod(0, 800000),
            new CRFQualityMethod(0, 51)
        };

        return compatibleQualityMethods;
    }
    public override EEncoders getAsEnum()
    {
        return EEncoders.LIBSVTAV1;
    }

    public override void checkQualityMethodValue(string qualityMethod, string qualityValue)
    {
        throw new NotImplementedException();
    }
}