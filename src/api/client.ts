import Repository from './types/repository';
import Version from './types/version';

class ApiClient {

  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public async get<T>(url: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}/${url}`);
    return await response.json();
  }

  public async getRepositories(): Promise<Repository[]> {
    return await this.get<Repository[]>('repositories');
  }

  public async getLatestVersions(repository?: Repository | string): Promise<Version[]> {
    const repoName = typeof repository === 'string' ? repository : repository?.name ?? 'all';
    return await this.get<Version[]>(`versions/${repoName}/latest`);
  }

  public async getAllVersions(repository?: Repository | string): Promise<Version[]> {
    const repoName = typeof repository === 'string' ? repository : repository?.name ?? 'all';
    return await this.get<Version[]>(`versions/${repoName}`);
  }
}

const Api = new ApiClient(process.env.REACT_APP_API_URL ?? 'https://thaliak.xiv.dev/api');
export default Api;

