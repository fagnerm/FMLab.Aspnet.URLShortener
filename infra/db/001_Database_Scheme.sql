CREATE TABLE IF NOT EXISTS "url_redirections" (
    "Hash"      VARCHAR(15) NOT NULL,
    "Target"    VARCHAR(2048) NOT NULL,
    "Temporary" BOOL NOT NULL DEFAULT TRUE
);