import Repository from './repository';
import Patch from './patch';

export default interface Version {
  repository: Repository;
  version: string;
  patches: Patch[];
}
