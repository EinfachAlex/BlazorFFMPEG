DROP TABLE IF EXISTS encode_jobs;
DROP TABLE IF EXISTS constants_status;
DROP TABLE IF EXISTS constants_qualityMethod;

CREATE TABLE constants_status
(
    id          SERIAL PRIMARY KEY,
    description TEXT
);

CREATE TABLE constants_qualityMethod (
    id SERIAL PRIMARY KEY,
    description TEXT,
    minQualityValue INT,
    maxQualityValue INT
);

INSERT INTO constants_qualityMethod(description, minQualityValue, maxQualityValue)
VALUES ('CRF / CQP', 0, 1000000);
INSERT INTO constants_qualityMethod(description, minQualityValue, maxQualityValue)
VALUES ('Bitrate', 0, 1000000);

CREATE TABLE encode_jobs
(
    jobId  SERIAL PRIMARY KEY,
    status SERIAL,
    codec TEXT,
    qualityMethod INTEGER,
    qualityValue INTEGER,
    path TEXT,

    CONSTRAINT fk_status FOREIGN KEY (status) REFERENCES constants_status (id),
    CONSTRAINT fk_qualityMethod FOREIGN KEY (qualityMethod) REFERENCES constants_qualityMethod (id)
);

INSERT INTO constants_status(description)
VALUES ('New');
INSERT INTO constants_status(description)
VALUES ('Working');
INSERT INTO constants_status(description)
VALUES ('Finished');

