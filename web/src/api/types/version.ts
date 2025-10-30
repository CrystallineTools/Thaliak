import Patch from './patch';

export default interface Version {
  versionString: string;
  versionId: number;
  isActive?: boolean;
  firstOffered?: string;
  lastOffered?: string;
  patches?: Patch[];
  prerequisiteVersions?: Version[];
  dependentVersions?: Version[];
}
