CREATE TABLE IF NOT EXISTS "short_urls" (
    "Hash"      VARCHAR(15)   NOT NULL,
    "Target"    VARCHAR(2048) NOT NULL,
    "Temporary" BOOL          NOT NULL DEFAULT TRUE,
    "IsActive"  BOOL          NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMPTZ   NOT NULL,
    "UpdatedAt" TIMESTAMPTZ,
    "ExpiresAt" TIMESTAMPTZ,
    "MaxClicks" INT,
    PRIMARY KEY ("Hash")
);

CREATE TABLE IF NOT EXISTS "url_clicks" (
    "Id"        BIGSERIAL     NOT NULL,
    "Hash"      VARCHAR(15)   NOT NULL,
    "ClickedAt" TIMESTAMPTZ   NOT NULL,
    "IpAddress" VARCHAR(45),
    "UserAgent" VARCHAR(512),
    "Referer"   VARCHAR(2048),
    "Country"   VARCHAR(2),
    PRIMARY KEY ("Id")
);

CREATE INDEX IF NOT EXISTS "ix_url_clicks_hash"       ON "url_clicks" ("Hash");
CREATE INDEX IF NOT EXISTS "ix_url_clicks_clicked_at" ON "url_clicks" ("ClickedAt");