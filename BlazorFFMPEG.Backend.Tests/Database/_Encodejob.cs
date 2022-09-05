using BlazorFFMPEG.Backend.Database;
using EinfachAlex.Utils.HashGenerator;

namespace BlazorFFMPEG.Backend.Tests.Database;

[TestClass]
public class EncodeJob_Test
{
    public EncodeJob_Test()
    {
    }

    [TestMethod]
    public void EncodeJob_generateId()
    {
        const string codec = "HEVC_NVENC";
        const string file = @"C:\testfile.mp4";
        const string result = "01f41eb5df083ff59f0e6f72dec60293a0cb6469796eb11494ac5db698c86761";
        
        Hash id = EncodeJob.generateId(codec, file);
        
        Assert.AreEqual(id.ToString(), result);
    }
}