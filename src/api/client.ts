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

  public async getRespositories(): Promise<Repository[]> {
    return await this.get<Repository[]>('repositories');
  }

  public async getLatestVersions(repository?: Repository): Promise<Version[]> {
    return await this.get<Version[]>(`versions/${repository?.name ?? 'all'}/latest`);
  }
}

const Api = new ApiClient(process.env.REACT_APP_API_URL ?? 'https://thaliak.xiv.dev/api');
export default Api;

