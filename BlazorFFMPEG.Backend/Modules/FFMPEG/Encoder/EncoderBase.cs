using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;
using BlazorFFMPEG.Shared.Constants;
using Microsoft.OpenApi.Extensions;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;

public abstract class EncoderBase
{
    public abstract List<QualityMethod> getCompatibleQualityMethods();

    public abstract EEncoders getAsEnum();
    
    public static EncoderBase constructByString(string encoder)
    {
        Enum.TryParse(encoder.ToUpper(), out EEncoders value);

        switch (value)
        {
            case EEncoders.LIBX264:
                throw new NotImplementedException();
                
            case EEncoders.LIBSVTAV1:
                return new LIBSVTAV1();

            case EEncoders.HEVC_NVENC:
                return new HEVC_NVENC();
        }

        throw new NotImplementedException();
    }

    /**
     * Checks if given quality value and quality method combination is valid for the current encoder instance
     */
    public virtual void checkQualityMethodValue(QualityMethod qualityMethod, string qualityValue)
    {
        QualityMethod compatibleQualityMethodSettings = this.getCompatibleQualityMethods().Find(qm => qm.getQualityMethodAsEnum() == qualityMethod.getQualityMethodAsEnum()) ?? throw new NullReferenceException();

        long qualityValueLong = Convert.ToInt64(qualityValue);

        if (compatibleQualityMethodSettings.minQualityValue > qualityValueLong
            || compatibleQualityMethodSettings.maxQualityValue < qualityValueLong)
        {
            throw new Exception("Not compatible!");
        }
    }
    
    /**
     * Checks if given quality value and quality method combination is valid for the current encoder instance
     * uses strings for parameters
     */
    public abstract void checkQualityMethodValue(string qualityMethod, string qualityValue);
    
    /**
     * Checks if given qualityMethod-Name is compatible with the current encoder
     */
    public void checkQualityMethodIsCompatibleWithEncoder(string qualityMethodName, out QualityMethod qualityMethod)
    {
        Enum.TryParse(qualityMethodName, out EQualityMethods EQualityMethod);

        qualityMethod = this.getCompatibleQualityMethods().Find(qm => qm.getQualityMethodAsEnum() == EQualityMethod)!;

        if (qualityMethod == null)
        {
            throw new Exception("Not compatible!");
        }
    }

    public override string ToString()
    {
        return this.GetType().Name;
    }
}