namespace BlazorFFMPEG.Shared.DTO;

public class Encoder
{
    public Encoder(string name)
    {
        this.name = name;
    }
    
    public string name { get; set;  }
}