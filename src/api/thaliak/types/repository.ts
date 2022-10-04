export interface LatestVersion {
  versionString: string;
  firstOffered: string;
  lastOffered: string;
}

export interface Repository {
  name: string;
  description?: string;
  slug: string;
  latestVersion: LatestVersion;
}
