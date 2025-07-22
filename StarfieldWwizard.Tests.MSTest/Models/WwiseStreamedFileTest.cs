using JetBrains.Annotations;
using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Tests.MSTest.Models;

[TestClass]
[TestSubject(typeof(WwiseStreamedFile))]
public class WwiseStreamedFileTest
{

    [TestMethod]
    public void testGettingSfxName()
    {
        var test = new WwiseStreamedFile
        {
            Id = 29651,
            Language = "SFX",
            ShortName = "AMB\\Artifact\\Puzzle\\Temple\\AMB_ArtifactPuzzle_TempleStart_01.wav"
        };
        
        Assert.AreEqual("AMB_ArtifactPuzzle_TempleStart_01", test.SfxName);
    }
}