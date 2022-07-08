import { atom, RecoilState } from 'recoil';
import Repository from './api/types/repository';
import Version from './api/types/version';

export const LAST_UPDATED: RecoilState<Date | undefined> = atom<Date | undefined>({
  key: 'last_updated',
  default: undefined
});

export const REPOSITORIES: RecoilState<Repository[]> = atom<Repository[]>({
  key: 'repositories',
  default: []
});

export const VERSIONS: RecoilState<Version[]> = atom<Version[]>({
  key: 'versions',
  default: []
});

export const LATEST_VERSIONS: RecoilState<Version[]> = atom<Version[]>({
  key: 'latest_versions',
  default: []
});