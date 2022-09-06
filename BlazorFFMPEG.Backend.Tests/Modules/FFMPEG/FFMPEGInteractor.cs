using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Shared.DTO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Tests.Modules.FFMPEG;

[TestClass]
public class FFMPEGInteractor_Test
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
    public async Task FFMPEGInteractor_getAvailableEncoders()
    {
        List<Encoder> availableEncoders = await new Backend.Modules.FFMPEG.FFMPEG().getAvailableEncoders();

        //Cannot check number of encoders => different on every machine
        //But not 0 on any machine
        Assert.IsFalse(availableEncoders.Count == 0);
    }
}