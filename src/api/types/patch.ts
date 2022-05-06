export default interface Patch {
  remoteOriginPath: string;
  firstSeen: string;
  lastSeen: string;
  size: number;
  hashType?: string;
  hashBlockSize?: number;
  hashes?: string[];
}
