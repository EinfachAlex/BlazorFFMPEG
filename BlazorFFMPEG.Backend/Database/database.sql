﻿DROP TABLE IF EXISTS encode_jobs;
DROP TABLE IF EXISTS constants_status;

CREATE TABLE constants_status
(
    id          SERIAL PRIMARY KEY,
    description TEXT
);

CREATE TABLE encode_jobs
(
    jobId  SERIAL PRIMARY KEY,
    status SERIAL,
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
