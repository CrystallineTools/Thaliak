/**
 * REST API Client
 */

import { API_BASE_URL } from './config';

/**
 * Base fetch wrapper with error handling
 */
async function apiFetch<T>(endpoint: string): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  try {
    const response = await fetch(url);

    if (!response.ok) {
      throw new Error(`API request failed: ${response.status} ${response.statusText}`);
    }

    return await response.json();
  } catch (error) {
    console.error(`Error fetching ${url}:`, error);
    throw error;
  }
}

/**
 * API Response Types
 */

export interface LatestPatchInfo {
  version_string: string;
  first_offered?: string;
  last_offered?: string;
}

export interface Repository {
  service_id: string;
  slug: string;
  name: string;
  description?: string;
  latest_patch?: LatestPatchInfo;
}

export interface RepositoriesResponse {
  repositories: Repository[];
  total: number;
}

export interface Hash {
  type: 'none' | 'sha1';
  block_size?: number;
  hashes?: string[];
}

export interface Patch {
  repository_slug: string;
  version_string: string;
  remote_url: string;
  local_path: string;
  first_seen?: string;
  last_seen?: string;
  size: number;
  hash: Hash;
  first_offered?: string;
  last_offered?: string;
  is_active: boolean;
}

export interface PatchesResponse {
  patches: Patch[];
  total: number;
  total_size: number;
}

export interface ComponentStatus {
  component: string;
  commit: string;
  started_at: string;
  uptime_seconds: number;
}

export interface StatusResponse {
  status: string;
  database: string;
  components: ComponentStatus[];
}

/**
 * API Client Functions
 */

/**
 * Get all repositories
 */
export async function getRepositories(): Promise<RepositoriesResponse> {
  return apiFetch<RepositoriesResponse>('/repositories');
}

/**
 * Get a specific repository by slug
 */
export async function getRepository(slug: string): Promise<Repository> {
  return apiFetch<Repository>(`/repositories/${slug}`);
}

/**
 * Get patches for a repository
 * @param slug - Repository slug
 * @param options - Query parameters
 */
export async function getRepositoryPatches(
  slug: string,
  options: {
    from?: string;
    to?: string;
    all?: boolean;
    active?: boolean;
  } = {}
): Promise<PatchesResponse> {
  const params = new URLSearchParams();

  if (options.from) params.append('from', options.from);
  if (options.to) params.append('to', options.to);
  if (options.all !== undefined) params.append('all', String(options.all));
  if (options.active !== undefined) params.append('active', String(options.active));

  const query = params.toString();
  const endpoint = `/repositories/${slug}/patches${query ? `?${query}` : ''}`;

  return apiFetch<PatchesResponse>(endpoint);
}

/**
 * Get a specific patch/version
 */
export async function getPatch(slug: string, version: string): Promise<Patch> {
  return apiFetch<Patch>(`/repositories/${slug}/patches/${version}`);
}

/**
 * Get API and poller status
 */
export async function getStatus(): Promise<StatusResponse> {
  return apiFetch<StatusResponse>('/status');
}
