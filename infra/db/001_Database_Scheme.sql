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