# Bank-statement evidence foundation

UX-017B stores tenant-scoped bank-account identities, immutable CSV import batches and immutable statement lines. Raw files are not retained; SHA-256 file and normalized transaction fingerprints prevent repeat imports and duplicate bank references. This slice intentionally does not match, post, adjust, charge fees, or affect period close readiness.
