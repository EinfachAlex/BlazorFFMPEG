using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Database;

public partial class ConstantsQualitymethod
{
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