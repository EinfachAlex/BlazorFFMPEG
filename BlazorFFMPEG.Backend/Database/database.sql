DROP TABLE IF EXISTS encode_jobs;
DROP TABLE IF EXISTS constants_status;
DROP TABLE IF EXISTS constants_qualityMethod;

CREATE TABLE constants_status
(
    id          SERIAL PRIMARY KEY,
    description TEXT NOT NULL
);

CREATE TABLE encode_jobs
(
    jobId  SERIAL PRIMARY KEY,
    status INT DEFAULT 0,
    codec TEXT NOT NULL,
    qualityMethod TEXT NOT NULL,
    qualityValue INTEGER NOT NULL,
    path TEXT NOT NULL,
    isAutoEncodeJob BOOLEAN DEFAULT FALSE,

    CONSTRAINT fk_status FOREIGN KEY (status) REFERENCES constants_status (id)
);

--List of folders that should be scanned, if video file is found it gets encoded
CREATE TABLE auto_encode_folder
(
    folderId SERIAL PRIMARY KEY,
    inputPath TEXT NOT NULL,
    outputPath TEXT NOT NULL
);

INSERT INTO constants_status(description)
VALUES ('New');
INSERT INTO constants_status(description)
VALUES ('Working');
INSERT INTO constants_status(description)
VALUES ('Finished');

