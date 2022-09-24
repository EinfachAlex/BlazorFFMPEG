using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;

public class LIBSVTAV1 : EncoderBase
{
    public override List<ConstantsQualitymethod> getCompatibleQualityMethods(databaseContext databaseContext)
    {
        List<ConstantsQualitymethod> compatibleQualityMethods = databaseContext.ConstantsQualitymethods.ToList();

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