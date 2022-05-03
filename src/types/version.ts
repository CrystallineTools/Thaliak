import Repository from './repository';
import Patch from './patch';

export default interface Version {
  id: number;
  repository: Repository;
  version: string;
  patches: Patch[];
}
