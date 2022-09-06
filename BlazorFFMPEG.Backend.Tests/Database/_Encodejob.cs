using BlazorFFMPEG.Backend.Database;
using EinfachAlex.Utils.HashGenerator;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Tests.Database;

[TestClass]
public class EncodeJob_Test
{
    private databaseContext databaseContext;

    [TestInitialize]
    public void init_db()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var _contextOptions = new DbContextOptionsBuilder<databaseContext>().UseSqlite(_connection).UseLazyLoadingProxies().Options;

        // Create the schema and seed some data
        databaseContext = new databaseContext(_contextOptions);


        if (databaseContext.Database.EnsureCreated())
        {
            using var viewCommand = databaseContext.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText = @"DROP TABLE IF EXISTS encode_jobs;
DROP TABLE IF EXISTS constants_status;

CREATE TABLE constants_status
(
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    description TEXT
);

CREATE TABLE encode_jobs
(
    jobId  INTEGER PRIMARY KEY AUTOINCREMENT,
    status INTEGER,
    codec TEXT,
    path TEXT,

    CONSTRAINT fk_status FOREIGN KEY (status) REFERENCES constants_status (id)
);

INSERT INTO constants_status(description)
VALUES ('New');
INSERT INTO constants_status(description)
VALUES ('Working');
INSERT INTO constants_status(description)
VALUES ('Finished');

";
            viewCommand.ExecuteNonQuery();
        }

        databaseContext.SaveChanges();
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

    [TestMethod]
    public void EncodeJob_constructNew_noCommit()
    {
        const string codec = "HEVC_NVENC";
        const string file = @"C:\testfile.mp4";

        EncodeJob encodeJob = EncodeJob.constructNew(databaseContext, false, codec, file);

        Assert.AreEqual(codec, encodeJob.Codec);
        Assert.AreEqual(file, encodeJob.Path);
    }

    [TestMethod]
    public void EncodeJob_constructNew_Commit()
    {
        const string codec = "HEVC_NVENC";
        const string file = @"C:\testfile.mp4";

        EncodeJob encodeJob = EncodeJob.constructNew(databaseContext, true, codec, file);

        Assert.AreEqual(codec, encodeJob.Codec);
        Assert.AreEqual(file, encodeJob.Path);

        EncodeJob encodeJobReadFromDB = databaseContext.EncodeJobs.Single(e => e.Jobid == encodeJob.Jobid);

        Assert.AreEqual(codec, encodeJobReadFromDB.Codec);
        Assert.AreEqual(file, encodeJobReadFromDB.Path);
    }
}