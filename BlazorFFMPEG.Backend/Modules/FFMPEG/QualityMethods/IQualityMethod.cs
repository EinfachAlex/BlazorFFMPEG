using BlazorFFMPEG.Shared.Constants;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;

public interface IQualityMethod
{
    public abstract EQualityMethods getQualityMethodAsEnum();
}