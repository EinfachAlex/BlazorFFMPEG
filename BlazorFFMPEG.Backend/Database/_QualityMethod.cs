using BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;
using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Database;

public partial class QualityMethod : IQualityMethod
{
    public long minQualityValue { get; }
    public long maxQualityValue { get; }

    protected QualityMethod(long minQualityValue, long maxQualityValue)
    {
        this.minQualityValue = minQualityValue;
        this.maxQualityValue = maxQualityValue;
    }
    
    public static QualityMethod construct(string qualityMethodName, long minQualityValue, long maxQualityValue)
    {
        throw new NotImplementedException();
    }

    // public virtual void checkValueIsCompatibleWithQualityMethod(string qualityValue)
    // {
    //     long qualityValueLong = Convert.ToInt64(qualityValue);
    //
    //     if (this.minQualityValue > qualityValueLong
    //         || this.maxQualityValue < qualityValueLong)
    //     {
    //         throw new Exception("Not compatible!");
    //     }   
    // }

    /*public abstract long getMinQualityValue();
    public abstract long getMaxQualityValue();*/
    
    public virtual EQualityMethods getQualityMethodAsEnum()
    {
        throw new NotImplementedException();
    }
}