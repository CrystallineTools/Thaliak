import Version from './version';

export default interface Patch {
  firstOffered?: Date;
  lastOffered?: Date;
  prerequisiteVersions?: Version[];
  dependentVersions?: Version[];
}
