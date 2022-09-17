using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;

public class HEVC_NVENC : Encoder
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

    public override void checkQualityMethodValue(string qualityMethod, string qualityValue)
    {
        //QualityMethod qualityMethodObject = QualityMethod.construct(qualityValue, );
        throw new NotImplementedException();
    }
}