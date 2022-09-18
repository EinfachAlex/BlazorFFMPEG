namespace BlazorFFMPEG.Shared.DTO;

public class EncoderDTO
{
    public EncoderDTO(string name)
    {
        this.name = name;
    }
    
    public string name { get; set;  }
}