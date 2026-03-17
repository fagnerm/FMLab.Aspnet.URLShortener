CREATE TABLE IF NOT EXISTS "short_urls" (
    "hash"      VARCHAR(15)   NOT NULL,
    "target"    VARCHAR(2048) NOT NULL,
    "temporary" BOOL          NOT NULL DEFAULT TRUE,
    "is_active"  BOOL          NOT NULL DEFAULT TRUE,
    "created_at" TIMESTAMPTZ   NOT NULL,
    "updated_at" TIMESTAMPTZ,
    "expires_at" TIMESTAMPTZ,
    "max_clicks" INT,
    PRIMARY KEY ("hash")
);

CREATE TABLE IF NOT EXISTS "url_clicks" (
    "id"        BIGSERIAL     NOT NULL,
    "hash"      VARCHAR(15)   NOT NULL,
    "clicked_at" TIMESTAMPTZ   NOT NULL,
    "ip_address" VARCHAR(45),
    "user_agent" VARCHAR(512),
    "referer"   VARCHAR(2048),
    "country"   VARCHAR(2),
    PRIMARY KEY ("id")
);

CREATE INDEX IF NOT EXISTS "ix_url_clicks_hash"       ON "url_clicks" ("hash");
CREATE INDEX IF NOT EXISTS "ix_url_clicks_clicked_at" ON "url_clicks" ("clicked_at");