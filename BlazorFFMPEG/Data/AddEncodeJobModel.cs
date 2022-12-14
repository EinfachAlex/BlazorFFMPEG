using BlazorFFMPEG.Shared.DTO;

namespace BlazorFFMPEG.Data;

public class AddEncodeJobModel
{
    public AvailableQualityMethod? selectedQualityMethod { get; private set; }
    public EncoderDTO? encoder { get; private set; }
    public int qualityValue { get; private set; }
    public string filePath { get; set; }

    public void setSelectedQualityMethod(AvailableQualityMethod availableQualityMethod)
    {
        this.selectedQualityMethod = availableQualityMethod;
    } 

    public void setEncoder(EncoderDTO newEncoderDto)
    {
        this.encoder = newEncoderDto;
    }
    
    public void setQualityValue(int qualityValue)
    {
        this.qualityValue = qualityValue;
    }

    public void setFilePath(string filePath)
    {
        this.filePath = filePath;
    }
}