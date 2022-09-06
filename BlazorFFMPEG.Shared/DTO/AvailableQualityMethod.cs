namespace BlazorFFMPEG.Shared.DTO;

public class AvailableQualityMethod
{
    public AvailableQualityMethod(string name)
    {
        this.name = name;
    }
    
    public string name { get; set; }
}